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
        /// <summary>
        /// 校验是否正确
        /// </summary>
        /// <returns></returns>
        public string List_DoCheck()
        {
            string str = "";

            string mystr = this.GetRequestVal("ToNodeID");
            int toNodeID = this.FK_Node;
            if (DataType.IsNullOrEmpty(mystr) == false)
                toNodeID = int.Parse(mystr);

            //集合.
            Conds conds = new Conds();
            conds.Retrieve(CondAttr.FK_Node, this.FK_Node, CondAttr.ToNodeID,
                toNodeID, CondAttr.CondType, this.GetRequestValInt("CondType"), CondAttr.Idx);

            if (conds.Count == 0)
                return "";
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

            //遍历方向条件.
            foreach (Cond item in conds)
            {
                if (item.HisDataFrom == ConnDataFrom.CondOperator)
                    str += " " + item.OperatorValue;
                else
                    str += " 1=1 "; // + item.AttrKey + item.FK_Operator + item.OperatorValue;
            }

            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = " SELECT TOP 1 No FROM Port_Emp WHERE " + str;
                    break;
                case DBType.MySQL:
                    sql = " SELECT No FROM Port_Emp WHERE " + str + " and  limit 1 ";
                    break;
                case DBType.Oracle:
                case DBType.DM:
                    sql = " SELECT No FROM Port_Emp WHERE " + str + " and rownum <=1 ";
                    break;
                default:
                    return "err@没有做的数据库类型判断.";
            }

            try
            {
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                return "格式正确:<font color=blue>" + str + "</blue>";
            }
            catch (Exception ex)
            {
                return "err@不符合规范. <font color=blue>" + str + "</font>";
            }
        }

    }
}
