using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.DA;

namespace BP.Port
{
    public class Glo
    {
        /// <summary>
        /// 根据部门编号s，获得该部门下的人员编号s
        /// </summary>
        /// <param name="depts">部门编号s</param>
        /// <returns>人员编号,格式为:zhangsan,lisi,wangwu</returns>
        public static string GenerEmpNosByDeptNos(string depts)
        {
            if (BP.DA.DataType.IsNullOrEmpty(depts) == true)
                return "";

            string sql = "SELECT No FROM Port_Emp WHERE FK_Dept IN ("+ GenerWhereInSQL(depts) + ")";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += dr[0] + ",";
            }
            return strs;
        }
        /// <summary>
        /// 根据岗位编号s，获得该岗位下的人员编号s
        /// </summary>
        /// <param name="stationNos">岗位编号s</param>
        /// <returns>人员编号,格式为:zhangsan,lisi,wangwu</returns>
        public static string GenerEmpNosByStationNos(string stationNos)
        {
            if (BP.DA.DataType.IsNullOrEmpty(stationNos) == true)
                return "";

            string sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station IN (" + GenerWhereInSQL(stationNos) + ")";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string strs = "";
            foreach (DataRow dr in dt.Rows)
                strs += dr[0] + ",";
            return strs;
        }
        /// <summary>
        /// 格式化SQL
        /// </summary>
        /// <param name="ids">001,002,003,</param>
        /// <returns>'001','002','003'</returns>
        public static string GenerWhereInSQL(string ids)
        {
            if (BP.DA.DataType.IsNullOrEmpty(ids) == true)
                return "";
            if (ids.Substring(0, 1).Equals(",")==true)
                ids = ids.Substring(0);

            string str = "";
            string[] strs = ids.Split(',');
            foreach (string s in strs)
                str += ",'" + s + "'";
            return str.Substring(1);
        }
    }
}
