using System;
using System.Data;
using BP.DA;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_Cond2020 : DirectoryPageBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_Cond2020()
        {
        }
        /// <summary>
        /// 初始化列表
        /// </summary>
        /// <returns></returns>
        public string List_Init()
        {
            // BP.WF.Template.CondAttr.ToNodeID
            // Conds condes = new Conds();
            // condes.RetrieveAll();
            return "";
        }
        public string List_Move()
        {
            string[] ens = this.GetRequestVal("MyPKs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                var enNo = ens[i];
                string sql = "UPDATE WF_Cond SET Idx=" + i + " WHERE MyPK='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "顺序移动成功..";
        }
        public string List_DoCheck()
        {
            int toNodeID = 0;
            string mystr = this.GetRequestVal("ToNodeID");
            if (DataType.IsNullOrEmpty(mystr) == false)
                toNodeID = int.Parse(mystr);

            int condType = this.GetRequestValInt("CondType");
            return List_DoCheckExt(condType,this.FK_Node, toNodeID);
        }
        /// <summary>
        /// 校验是否正确
        /// </summary>
        /// <returns></returns>
        public static string List_DoCheckExt( int condType,int nodeID, int toNodeID)
        {
            if (toNodeID == 0)
                toNodeID = nodeID;

            //集合.
            Conds conds = new Conds();
            conds.Retrieve(CondAttr.FK_Node, nodeID, CondAttr.ToNodeID,
                toNodeID, CondAttr.CondType, condType, CondAttr.Idx);

            if (conds.Count == 0)
                return " 没有条件. ";

            if (conds.Count == 1)
            {
                foreach (Cond item in conds)
                {
                    if (item.HisDataFrom == ConnDataFrom.CondOperator)
                        return "info@请继续增加条件";
                    else
                        return "条件成立.";
                }
            }
            string str = "";
            //遍历方向条件.
            foreach (Cond item in conds)
            {
                if (item.HisDataFrom == ConnDataFrom.CondOperator)
                    str += " " + item.OperatorValue;
                else
                    str += " 1=1 "; // + item.AttrKey + item.FK_Operator + item.OperatorValue;
            }

            string sql = "";
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP 1 No FROM Port_Emp WHERE " + str;
                    break;
                case DBType.MySQL:
                    sql = " SELECT No FROM Port_Emp WHERE " + str + "   limit 1 ";
                    break;
                case DBType.Oracle:
                case DBType.DM:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = " SELECT No FROM Port_Emp WHERE (" + str + ") AND  rownum <=1 ";
                    break;
                default:
                    return "err@没有做的数据库类型判断.";
            }

            try
            {
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return "格式正确:" + str;
            }
            catch (Exception ex)
            {
                return "不符合规范:" + str;
            }
        }
        /// <summary>
        /// 初始化岗位
        /// </summary>
        /// <returns></returns>

        public string SelectStation_StationTypes()
        {
            string sql = "select No,Name FROM port_StationType WHERE No in (SELECT Fk_StationType from Port_Station WHERE OrgNo ='" + this.GetRequestVal("OrgNo") + "')";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
	}
}
