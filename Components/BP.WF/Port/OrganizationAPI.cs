using BP.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Port
{
    /// <summary>
    /// 组织接口API
    /// </summary>
    public class OrganizationAPI
    {
        #region 关于组织结构的接口.
        /// <summary>
        /// 集团模式下同步组织以及管理员信息.
        /// </summary>
        /// <param name="orgNo">组织编号</param>
        /// <param name="name">组织名称</param>
        /// <param name="adminer">管理员账号</param>
        /// <param name="adminerName">管理员名字</param>
        /// <param name="keyval">比如：@Leaer=zhangsan@Tel=12233333@Idx=1</param>
        /// <returns>return 1 增加成功，其他的增加失败.</returns>
        public static string Port_Org_Save(string orgNo, string name, string adminer, string adminerName, string keyVals)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护组织信息";

            int msg = 0;
            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                AtPara ap = new AtPara(keyVals);
                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
                org.No = orgNo;
                org.Name = name;
                org.Adminer = adminer;
                org.AdminerName = adminerName;
                foreach (string key in ap.HisHT.Keys)
                {
                    if (DataType.IsNullOrEmpty(key) == true)
                        continue;
                    org.SetValByKey(key, ap.GetValStrByKey(key));
                }
                msg = org.Insert();

                BP.WF.Port.Admin2Group.OrgAdminer oa = new BP.WF.Port.Admin2Group.OrgAdminer();
                oa.setMyPK(orgNo + "_" + adminer);
                oa.OrgNo = orgNo;
                oa.EmpNo = adminer;
                oa.EmpName = adminerName;
                msg = oa.Insert();
            }
            return msg.ToString();
        }
        /// <summary>
        /// 保存用户数据, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="orgNo">组织编号</param>
        /// <param name="userNo">用户编号,如果是saas版本就是orgNo_userID</param>
        /// <param name="userName">用户名称</param>
        /// <param name="deptNo">部门编号</param>
        /// <param name="kvs">属性值，比如: @Name=张三@Tel=18778882345@Pass=123, 如果是saas模式：就必须有@UserID=xxxx </param>
        /// <param name="stats">角色编号：比如:001,002,003,</param>
        /// <returns>执行信息.</returns>
        public static string Port_Emp_Save(string orgNo, string userNo, string userName, string deptNo, string kvs, string stats)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护人员信息";

            if (BP.Difference.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
            {
                if (kvs == null || kvs.IndexOf("@UserID=") == -1)
                    return "err@saas模式下，需要在kvs参数里，增加@UserID=xxxx 属性.";
            }

            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(orgNo) == true)
                    return "err@组织编号不能为空.";

                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
                org.No = orgNo;
                if (org.RetrieveFromDBSources() == 0)
                    return "err@组织编号错误:" + orgNo;
            }
            else
            {
                orgNo = "";
            }

            if (DataType.IsNullOrEmpty(userNo) || DataType.IsNullOrEmpty(userName) || DataType.IsNullOrEmpty(deptNo) == true)
                throw new Exception("err@用户编号，名称，部门不能为空.");

            BP.Port.Dept dept = new BP.Port.Dept();
            dept.No = deptNo;
            if (dept.RetrieveFromDBSources() == 0)
                throw new Exception("err@部门编号错误:" + deptNo);

            try
            {
                //增加人员信息.
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = userNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    emp.Name = userName;
                    emp.DeptNo = deptNo;
                    emp.OrgNo = orgNo;
                    emp.Insert();
                }

                BP.DA.AtPara ap = new AtPara(kvs);
                foreach (string key in ap.HisHT.Keys)
                {
                    if (DataType.IsNullOrEmpty(key) == true)
                        continue;
                    emp.SetValByKey(key, ap.GetValStrByKey(key));
                }
                emp.DeptNo = deptNo;
                emp.Name = userName;
                emp.OrgNo = orgNo;
                emp.Update();

                if (BP.Difference.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.Single)
                {
                    BP.DA.DBAccess.RunSQL("DELETE FROM Port_DeptEmp WHERE FK_Emp='" + userNo + "'");
                    BP.DA.DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation WHERE FK_Emp='" + userNo + "'");
                }
                else
                {
                    BP.DA.DBAccess.RunSQL("DELETE FROM Port_DeptEmp WHERE FK_Emp='" + userNo + "' AND OrgNo='" + orgNo + "'");
                    BP.DA.DBAccess.RunSQL("DELETE FROM Port_DeptEmpStation WHERE FK_Emp='" + userNo + "' AND OrgNo='" + orgNo + "'");
                }

                //插入部门.
                BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                de.DeptNo = deptNo;
                de.EmpNo = userNo;
                de.OrgNo = orgNo;
                de.DeptName = dept.Name;
                de.MyPK = de.DeptNo + "_" + userNo;

                //更新角色.
                if (stats == null)
                    stats = "";
                string[] strs = stats.Split(',');
                string staNames = "";
                for (int i = 0; i < strs.Length; i++)
                {
                    string str = strs[i];
                    if (DataType.IsNullOrEmpty(str))
                        continue;

                    Station st = new Station();
                    st.No = str;
                    if (st.RetrieveFromDBSources() == 0)
                        throw new Exception("err@角色编号错误." + str);

                    staNames += st.Name + ",";

                    //插入部门.
                    DeptEmpStation des = new DeptEmpStation();
                    des.DeptNo = deptNo;
                    des.EmpNo = userNo;
                    des.StationNo = str;
                    des.OrgNo = orgNo;
                    des.MyPK = de.DeptNo + "_" + des.EmpNo + "_" + des.StationNo;
                    des.DirectInsert();
                }

                de.StationNo = stats;
                de.StationNoT = staNames;
                de.DirectInsert();

                DBAccess.RunSQL("UPDATE Port_Emp SET OrgNo='" + orgNo + "' WHERE No='" + emp.No + "'");
                return "人员信息保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="stas">角色用逗号分开</param>
        /// <returns>reutrn 1=成功,  其他的标识异常.</returns>
        public static string Port_Emp_Delete(string orgNo, string userNo)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能删除人员信息";
            try
            {
                //增加人员信息.
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.No = userNo;
                emp.OrgNo = orgNo;
                if (emp.RetrieveFromDBSources() == 0)
                    return "err@该用户【" + userNo + "】不存在.";

                //删除角色.
                BP.DA.DBAccess.RunSQL("delete from port_deptemp where fk_emp='" + userNo + "' AND OrgNo='" + orgNo + "'");
                BP.DA.DBAccess.RunSQL("delete from port_deptempStation where fk_emp='" + userNo + "' AND OrgNo='" + orgNo + "'");
                emp.Delete();
                return "人员信息删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存部门, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="orgNo">组织编号</param>
        /// <param name="no">部门编号</param>
        /// <param name="name">名称</param>
        /// <param name="parntNo">父节点编号</param>
        /// <param name="keyval">比如：@Leaer=zhangsan@Tel=12233333@Idx=1</param>
        /// <returns>return 1 增加成功，其他的增加失败.</returns>
        public static string Port_Dept_Save(string orgNo, string no, string name, string parntNo, string keyVals)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护部门信息";

            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(orgNo) == true)
                    return "err@组织编号不能为空.";

                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
                org.No = orgNo;
                if (org.RetrieveFromDBSources() == 0)
                    return "err@组织编号错误:" + orgNo;
            }

            try
            {
                //增加人员信息.
                BP.Port.Dept deptP = new BP.Port.Dept(parntNo);
                AtPara ap = new AtPara(keyVals);
                //增加部门.
                BP.Port.Dept dept = new BP.Port.Dept();
                dept.No = no;
                if (dept.RetrieveFromDBSources() == 0)
                {
                    dept.Name = name;
                    dept.ParentNo = parntNo;
                    dept.OrgNo = orgNo;

                    foreach (string key in ap.HisHT.Keys)
                    {
                        if (DataType.IsNullOrEmpty(key) == true)
                            continue;
                        dept.SetValByKey(key, ap.GetValStrByKey(key));
                    }
                    dept.Insert();
                }
                else
                {
                    dept.Name = name;
                    dept.ParentNo = parntNo;
                    dept.OrgNo = orgNo;

                    foreach (string key in ap.HisHT.Keys)
                    {
                        if (DataType.IsNullOrEmpty(key) == true)
                            continue;
                        dept.SetValByKey(key, ap.GetValStrByKey(key));
                    }

                    dept.Update();
                }

                DBAccess.RunSQL("UPDATE Port_Dept SET OrgNo='" + orgNo + "' WHERE No='" + dept.No + "'");

                return "部门信息保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 删除部门.
        /// </summary>
        /// <param name="no">删除指定的部门编号</param>
        /// <returns></returns>

        public static string Port_Dept_Delete(string no)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能删除部门信息";

            try
            {
                //删除部门.
                BP.Port.Dept dept = new BP.Port.Dept(no);
                dept.Delete();

                return "删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存角色, 如果有此数据则修改，无此数据则增加.
        /// </summary>
        /// <param name="orgNo">组织编号</param>
        /// <param name="no">编号</param>
        /// <param name="name">名称</param>
        /// <returns>return 1 增加成功，其他的增加失败.</returns>
        public static string Port_Station_Save(string orgNo, string no, string name, string keyVals)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护岗位信息";

            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(orgNo) == true)
                    return "err@组织编号不能为空.";

                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
                org.No = orgNo;
                if (org.RetrieveFromDBSources() == 0)
                    return "err@组织编号错误:" + orgNo;
            }

            try
            {
                AtPara ap = new AtPara(keyVals);

                //增加部门.
                BP.Port.Station en = new BP.Port.Station();
                en.No = no;
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.Name = name;
                    en.OrgNo = orgNo;
                    en.Insert();
                }
                foreach (string item in ap.HisHT.Keys)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                        continue;
                    en.SetValByKey(item, ap.GetValStrByKey(item));
                }
                en.Name = name;
                en.OrgNo = orgNo;
                en.Update();

                DBAccess.RunSQL("UPDATE Port_Station SET OrgNo='" + orgNo + "' WHERE No='" + no + "'");
                return "[" + name + "]保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 删除部门.
        /// </summary>
        /// <param name="no">删除指定的部门编号</param>
        /// <returns></returns>
        public static string Port_Station_Delete(string no)
        {
            try
            {
                if (BP.Web.WebUser.IsAdmin == false)
                    return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能删除岗位信息";
                //删除部门.
                BP.Port.Station dept = new BP.Port.Station(no);
                dept.Delete();

                return "删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public static string Port_Team_Delete(string no)
        {
            try
            {
                if (BP.Web.WebUser.IsAdmin == false)
                    return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能删除岗位信息";
                //删除部门.
                BP.Port.Team dept = new BP.Port.Team(no);
                dept.Delete();

                return "删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public static string Port_Team_Save(string orgNo, string no, string name, string keyVals)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护岗位信息";

            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(orgNo) == true)
                    return "err@组织编号不能为空.";

                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
                org.No = orgNo;
                if (org.RetrieveFromDBSources() == 0)
                    return "err@组织编号错误:" + orgNo;
            }

            try
            {
                AtPara ap = new AtPara(keyVals);

                //增加部门.
                BP.Port.Team en = new BP.Port.Team();
                en.No = no;
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.Name = name;
                    en.SetValByKey("OrgNo", orgNo);
                    en.Insert();
                }
                foreach (string item in ap.HisHT.Keys)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                        continue;
                    en.SetValByKey(item, ap.GetValStrByKey(item));
                }
                en.Name = name;
                en.SetValByKey("OrgNo", orgNo);
                en.Update();

                DBAccess.RunSQL("UPDATE Port_Team SET OrgNo='" + orgNo + "' WHERE No='" + no + "'");
                return "[" + name + "]保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        public static string Port_TeamType_Delete(string no)
        {
            try
            {
                if (BP.Web.WebUser.IsAdmin == false)
                    return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能删除岗位信息";
                //删除部门.
                BP.Port.TeamType dept = new BP.Port.TeamType(no);
                dept.Delete();

                return "删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public static string Port_TeamType_Save(string orgNo, string no, string name, string keyVals)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护岗位信息";

            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(orgNo) == true)
                    return "err@组织编号不能为空.";

                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org(orgNo);
                if (org.RetrieveFromDBSources() == 0)
                    return "err@组织编号错误:" + orgNo;
            }

            try
            {
                AtPara ap = new AtPara(keyVals);

                //增加部门.
                BP.Port.TeamType en = new BP.Port.TeamType();
                en.No = no;
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.Name = name;
                    en.SetValByKey("OrgNo", orgNo);
                    en.Insert();
                }
                foreach (string item in ap.HisHT.Keys)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                        continue;
                    en.SetValByKey(item, ap.GetValStrByKey(item));
                }
                en.Name = name;
                en.SetValByKey("OrgNo", orgNo);
                en.Update();

                DBAccess.RunSQL("UPDATE Port_TeamType SET OrgNo='" + orgNo + "' WHERE No='" + no + "'");
                return "[" + name + "]保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion 关于组织的接口.

        #region 岗位类型
        public static string Port_StationType_Delete(string no)
        {
            try
            {
                if (BP.Web.WebUser.IsAdmin == false)
                    return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能删除岗位信息";
                //删除部门.
                BP.Port.StationType dept = new BP.Port.StationType(no);
                dept.Delete();

                return "删除成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        public static string Port_StationType_Save(string orgNo, string no, string name, string keyVals)
        {
            if (BP.Web.WebUser.IsAdmin == false)
                return "err@[" + BP.Web.WebUser.Name + "]不是管理员不能维护岗位信息";

            if (BP.Difference.SystemConfig.CCBPMRunModel != BP.Sys.CCBPMRunModel.Single)
            {
                if (DataType.IsNullOrEmpty(orgNo) == true)
                    return "err@组织编号不能为空.";

                BP.WF.Port.AdminGroup.Org org = new BP.WF.Port.AdminGroup.Org();
                org.No = orgNo;
                if (org.RetrieveFromDBSources() == 0)
                    return "err@组织编号错误:" + orgNo;
            }

            try
            {
                AtPara ap = new AtPara(keyVals);

                //增加部门.
                BP.Port.StationType en = new BP.Port.StationType();
                en.No = no;
                if (en.RetrieveFromDBSources() == 0)
                {
                    en.Name = name;
                    en.SetValByKey("OrgNo", orgNo);
                    en.Insert();
                }
                foreach (string item in ap.HisHT.Keys)
                {
                    if (DataType.IsNullOrEmpty(item) == true)
                        continue;
                    en.SetValByKey(item, ap.GetValStrByKey(item));
                }
                en.Name = name;
                en.SetValByKey("OrgNo", orgNo);
                en.Update();

                DBAccess.RunSQL("UPDATE Port_StationType SET OrgNo='" + orgNo + "' WHERE No='" + no + "'");
                return "[" + name + "]保存成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion
    }
}
