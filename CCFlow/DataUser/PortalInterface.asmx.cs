using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using BP.DA;

namespace ccbpm
{
    /// <summary>
    /// OverrideInterface 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class PortalInterface : System.Web.Services.WebService
    {
        #region 短消息写入接口.
        /// <summary>
        /// 该方法是调用发送短消息的接口。
        /// 需要与web.config中 ShortMessageWriteTo 配置才能起作用。
        /// 一般来说，它用于写入到手机短信接口.
        /// </summary>
        /// <param name="smsEntityJson"></param>
        /// <returns></returns>
        [WebMethod]
        public bool WriteShortMessage(string smsEntityJson)
        {

            return true;
        }
        #endregion 短消息写入接口.


        #region 组织结构.
        /// <summary>
        /// 检查用户名密码是否正确
        /// </summary>
        /// <param name="userNo">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>正确返回1，不正确返回0，其他的情况抛出异常。</returns>
        [WebMethod]
        public int CheckUserNoPassWord(string userNo,string password)
        {
            #region 简单Demo
            try
            {
                string sql= "SELECT Pass FROM Port_Emp WHERE No='"+userNo+"'";
                string pass = BP.DA.DBAccessOfMSSQL1.RunSQLReturnVal(sql) as string;
                if ( pass == password)
                    return 1; //成功返回1.
                return 0; //失败返回0.
            }
            catch (Exception ex)
            {
                throw new Exception("@校验出现错误:"+ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单Demo
        }
        /// <summary>
        /// 获得部门信息
        /// </summary>
        /// <returns>返回No,Name,ParentNo至少三个列的部门信息</returns>
        [WebMethod]
        public DataTable GetDept(string deptNo)
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Port_Dept WHERE No='"+deptNo+"'");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得部门出现错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得部门信息
        /// </summary>
        /// <returns>返回No,Name,ParentNo至少三个列的部门信息</returns>
        [WebMethod]
        public DataTable GetDepts()
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Port_Dept ORDER BY ParentNo,No");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得部门出现错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得部门信息
        /// </summary>
        /// <returns>返回No,Name,ParentNo至少三个列的部门信息</returns>
        [WebMethod]
        public DataTable GetDeptsByParentNo(string parentDeptNo)
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Port_Dept WHERE ParentNo='" + parentDeptNo + "' ORDER BY ParentNo,No");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得部门出现错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得岗位信息
        /// </summary>
        /// <returns>返回No,Name,StaGrade至少三个列的岗位信息</returns>
        [WebMethod]
        public DataTable GetStations()
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT No,Name,StaGrade FROM Port_Station ORDER BY StaGrade,No");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得岗位出现错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得岗位信息
        /// </summary>
        /// <returns>返回No,Name,StaGrade至少三个列的岗位信息</returns>
        [WebMethod]
        public DataTable GetStation(string stationNo)
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT No,Name,StaGrade FROM Port_Station WHERE No='" + stationNo + "'");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得岗位出现错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得人员信息(一人多部门)
        /// </summary>
        /// <returns>返回No,Name,FK_Dept至少三个列的部门、人员、岗位信息</returns>
        [WebMethod]
        public DataTable GetEmps()
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT a.No,a.Name,a.FK_Dept,b.Name as FK_DeptText FROM Port_Emp a, Port_Dept b WHERE (a.FK_Dept=b.No) ");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得人员信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得人员信息(一人多部门)
        /// </summary>
        /// <returns>返回No,Name,FK_Dept至少三个列的部门、人员、岗位信息</returns>
        [WebMethod]
        public DataTable GetEmpsByDeptNo(string deptNo)
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT a.No,a.Name,a.FK_Dept,b.Name as FK_DeptText FROM Port_Emp a, Port_Dept b WHERE a.FK_Dept=b.No AND A.FK_Dept='"+deptNo+"' ");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得人员信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得人员信息(一人多部门)
        /// </summary>
        /// <returns>返回No,Name,FK_Dept至少三个列的部门、人员、岗位信息</returns>
        [WebMethod]
        public DataTable GetEmp(string no)
        {

            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT a.No,a.Name,a.FK_Dept,b.Name as FK_DeptText FROM Port_Emp a, Port_Dept b WHERE (a.No='"+no+"') AND (a.FK_Dept=b.No) ");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得人员信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得部门人员信息(一人多部门)
        /// </summary>
        /// <returns>返回FK_Dept,FK_Emp至少三个列的部门、人员、岗位信息</returns>
        [WebMethod]
        public DataTable GetDeptEmp()
        {
            #region 简单 Demo
            try
            {

                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT FK_Dept,FK_Emp FROM Port_DeptEmp");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得部门人员信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得人员他的部门实体信息集合.
        /// </summary>
        /// <returns>返回No,Name,ParentNo部门信息</returns>
        [WebMethod]
        public DataTable GetEmpHisDepts(string empNo)
        {
            #region 简单 Demo
            try
            {
                string sql = "SELECT No,Name,ParentNo FROM Port_Dept WHERE No IN(SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp='"+empNo+"')";
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("@获得人员他的部门实体信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得人员他的岗位实体信息集合.
        /// </summary>
        /// <returns>返回No,Name,StaGrade岗位信息</returns>
        [WebMethod]
        public DataTable GetEmpHisStations(string empNo)
        {
            #region 简单 Demo
            try
            {
                string sql = "SELECT No,Name,StaGrade FROM Port_Station WHERE No IN(SELECT FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + empNo + "')";
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("@获得人员他的岗位实体信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 获得部门人员岗位对应信息
        /// </summary>
        /// <returns>返回FK_Dept,FK_Emp,FK_Station至少三个列的部门、人员、岗位信息</returns>
        [WebMethod]
        public DataTable GetDeptEmpStations()
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT FK_Dept,FK_Emp,FK_Station FROM Port_DeptEmpStation");
            }
            catch (Exception ex)
            {
                throw new Exception("@获得部门人员岗位对应信息:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
       
        #endregion 组织结构.

        #region 特殊的查询
        /// <summary>
        /// 通过一组岗位编号获得他的人员集合
        /// </summary>
        /// <param name="stationNos">用逗号隔开的岗位集合比如: '01','02'  </param>
        /// <returns>返回No,Name,FK_Dept三个列.</returns>
        [WebMethod]
        public DataTable GenerEmpsByStations(string stationNos)
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT a.No,a.Name,a.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND B.FK_Station IN (" + stationNos + ")");
            }
            catch (Exception ex)
            {
                throw new Exception("@根据岗位集合，获得人员集合错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        /// <summary>
        /// 通过一组部门编号获得他的人员集合
        /// </summary>
        /// <param name="deptNos">用逗号隔开的部门集合比如: '01','02'  </param>
        /// <returns>返回No,Name,FK_Dept三个列.</returns>
        [WebMethod]
        public DataTable GenerEmpsByDepts(string deptNos)
        {
            #region 简单 Demo
            try
            {
                return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable("SELECT a.No,a.Name,a.FK_Dept FROM Port_Emp A, Port_DeptEmp B WHERE A.No=B.No AND B.FK_Dept IN (" + deptNos + ")");
            }
            catch (Exception ex)
            {
                throw new Exception("@根据部门集合，获得人员集合错误:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
       
        /// <summary>
        /// 指定部门与一个岗位集合，获得他们的人员。
        /// </summary>
        /// <param name="deptNo">部门编号</param>
        /// <param name="stations">岗位编号s</param>
        /// <returns>No,Name,FK_Dept三个列的人员信息</returns>
        [WebMethod]
        public DataTable GenerEmpsBySpecDeptAndStats(string deptNo, string stations)
        {
            #region 简单 Demo
            try
            {
                string sql = "SELECT a.No,a.Name,a.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND B.FK_Station IN (" + stations + ") AND A.FK_Dept='" + deptNo + "'";
                 return BP.DA.DBAccessOfMSSQL1.RunSQLReturnTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("@ 指定部门与一个岗位集合，获得他们的人员:" + ex.Message); //连接错误，直接抛出异常.
            }
            #endregion 简单 Demo
        }
        #endregion
    }
}
