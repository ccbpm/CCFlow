using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.Difference;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Setting : DirectoryPageBase
    {
        /// <summary>
        /// 清楚缓存
        /// </summary>
        /// <returns></returns>
        public string Default_ClearCash()
        {
            DBAccess.RunSQL("DELETE FROM Sys_UserRegedit WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'");
            return "执行成功，请刷新菜单或者重新进入看看菜单权限是否有变化。";
        }
        public string UpdateEmpNo()
        {
            BP.Port.Emp emp = new Emp(WebUser.No);
            emp.Email = this.GetRequestVal("Email");
            emp.Tel = this.GetRequestVal("Tel");
            emp.Name = this.GetRequestVal("Name");
            emp.Update();
            return "修改成功.";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Setting()
        {
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.


        public string Default_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", WebUser.No);
            ht.Add("UserName", WebUser.Name);

            BP.Port.Emp emp = new Emp();
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                emp.No = BP.Web.WebUser.OrgNo + "_" + WebUser.No;
            else
                emp.No = WebUser.No;
            emp.UserID = WebUser.No;
            emp.Retrieve();

            //部门名称.
            ht.Add("DeptName", emp.FK_DeptText);


            BP.Port.DeptEmpStations des = new BP.Port.DeptEmpStations();
            des.Retrieve(BP.Port.DeptEmpStationAttr.FK_Emp, emp.UserID);

            string depts = "";
            string stas = "";

            foreach (BP.Port.DeptEmpStation item in des)
            {
                BP.Port.Dept dept = new Dept();
                dept.No = item.FK_Dept;
                int count = dept.RetrieveFromDBSources();
                if (count != 0)
                    depts += dept.Name + "、";


                if (DataType.IsNullOrEmpty(item.FK_Station) == true)
                    continue;

                if (DataType.IsNullOrEmpty(item.FK_Dept) == true)
                {
                    //   item.Delete();
                    continue;
                }

                BP.Port.Station sta = new Station();
                sta.No = item.FK_Station;
                count = sta.RetrieveFromDBSources();
                if (count != 0)
                    stas += sta.Name + "、";
            }

            ht.Add("Depts", depts);
            ht.Add("Stations", stas);

            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(WebUser.UserID);
            ht.Add("Tel", emp.Tel);
            ht.Add("Email", emp.Email);

            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>json数据</returns>
        public string Author_Init()
        {
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(BP.Web.WebUser.No);
            Hashtable ht = emp.Row;
            ht.Remove(BP.WF.Port.WFEmpAttr.StartFlows); //移除这一列不然无法形成json.
            return emp.ToJson();
        }

        #region 图片签名.
        public string Siganture_Init()
        {
            if (BP.Web.WebUser.NoOfRel == null)
                return "err@登录信息丢失";

            //首先判断是否存在，如果不存在就生成一个.
            BP.WF.DTS.GenerSiganture.GenerIt(BP.Web.WebUser.No, BP.Web.WebUser.Name);


            Hashtable ht = new Hashtable();
            ht.Add("No", BP.Web.WebUser.No);
            ht.Add("Name", BP.Web.WebUser.Name);
            ht.Add("FK_Dept", BP.Web.WebUser.FK_Dept);
            ht.Add("FK_DeptName", BP.Web.WebUser.FK_DeptName);
            return BP.Tools.Json.ToJson(ht);
        }
        public string Siganture_Save()
        {
            //HttpPostedFile f = context.Request.Files[0];
            string empNo = this.GetRequestVal("EmpNo");
            if (DataType.IsNullOrEmpty(empNo) == true)
                empNo = WebUser.No;
            try
            {
                string tempFile = BP.Difference.SystemConfig.PathOfWebApp + "DataUser/Siganture/" + empNo + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                //f.SaveAs(tempFile);
                HttpContextHelper.UploadFile(tempFile);
                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                string info = "\t\n 上传出现错误，有可能是文件的权限出现错误,请按照如下步骤解决.";
                info += "\t\n  1. 有可能是\\DataUser\\Siganture文件夹是只读的, 右键文件夹属性取消只读.";
                info += "\t\n  2. 当前的iis_user用户没有读写他的权限，请在文件夹属性设置.";
                info += "\t\n  3. 不要设置everyone 权限会导致，不安全.";
                info += "\t\n  4. 如果是.net用户,请尝试修改web.config (该步骤没有验证) identity impersonate=true userName=administrator password=bpm2017@123 ";

                return "err@上传失败";// + ex.Message + "" + info;
            }

            //f.SaveAs(BP.Difference.SystemConfig.PathOfWebApp + "DataUser/Siganture/" + WebUser.No + ".jpg");
            // f.SaveAs(BP.Difference.SystemConfig.PathOfWebApp + "DataUser/Siganture/" + WebUser.Name + ".jpg");
            //f.PostedFile.InputStream.Close();
            //f.PostedFile.InputStream.Dispose();
            //f.Dispose();

            //   this.Response.Redirect(this.Request.RawUrl, true);
            return "上传成功！";
        }
        #endregion 图片签名.

        #region 头像.
        public string HeadPic_Save()
        {
            //HttpPostedFile f = context.Request.Files[0];
            string empNo = this.GetRequestVal("EmpNo");

            if (DataType.IsNullOrEmpty(empNo) == true)
                empNo = WebUser.No;
            try
            {
                string tempFile = BP.Difference.SystemConfig.PathOfWebApp + "DataUser/UserIcon/" + empNo + ".png";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                //f.SaveAs(tempFile);
                HttpContextHelper.UploadFile(tempFile);
                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            return "上传成功！";
        }
        #endregion 头像.

        #region 切换部门.
        /// <summary>
        /// 初始化切换部门.
        /// </summary>
        /// <returns></returns>
        public string ChangeDept_Init()
        {

            string sql = "";
            //如果是集团版.
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
                sql = "SELECT a.No, a.Name, A.NameOfPath, '0' AS  CurrentDept, A.OrgNo, '' as OrgName FROM Port_Dept A, Port_DeptEmp B WHERE A.No=B.FK_Dept AND B.FK_Emp='" + WebUser.No + "'";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT a.No, a.Name, A.NameOfPath, '0' AS  CurrentDept  FROM Port_Dept A, Port_DeptEmp B WHERE A.No=B.FK_Dept AND B.FK_Emp='" + WebUser.No + "'";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                sql = "SELECT a.No, a.Name, A.NameOfPath, '0' AS  CurrentDept, A.OrgNo, '' as OrgName  FROM Port_Dept A, Port_DeptEmp B,Port_Emp C WHERE A.No=B.FK_Dept AND C.FK_Dept=A.No AND C.No=B.FK_Emp AND C.UserID='" + WebUser.No + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (dt.Rows.Count == 0)
            {
                sql = "SELECT a.No,a.Name,B.NameOfPath, '1' as CurrentDept ,  B.OrgNo, '' as OrgName FROM ";
                sql += " Port_Emp A, Port_Dept B WHERE A.FK_Dept=B.No  AND A.No='" + BP.Web.WebUser.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
            }

            dt.Columns[0].ColumnName = "No";
            dt.Columns[1].ColumnName = "Name";
            dt.Columns[2].ColumnName = "NameOfPath";
            dt.Columns[3].ColumnName = "CurrentDept";

            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
            {
                dt.Columns[4].ColumnName = "OrgNo";
                dt.Columns[5].ColumnName = "OrgName";

                //设置组织名字.
                foreach (DataRow dr in dt.Rows)
                {
                    string orgNo = dr[4].ToString();
                    dr[5] = DBAccess.RunSQLReturnVal("SELECT Name FROM Port_Org WHERE No='" + orgNo + "'", null);
                }
            }

            //设置当前的部门.
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["No"].ToString().Equals(WebUser.FK_Dept) == true)
                    dr["CurrentDept"] = "1";

                if (DataType.IsNullOrEmpty(dr["NameOfPath"].ToString()) == true)
                    dr["NameOfPath"] = dr["Name"];
            }
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 提交选择的部门。
        /// </summary>
        /// <returns></returns>
        public string ChangeDept_Submit()
        {
            string deptNo = this.GetRequestVal("DeptNo");

            BP.Port.Dept dept = new BP.Port.Dept(deptNo);

            // @honygan.
            DBAccess.RunSQL("UPDATE Port_Emp SET OrgNo='" + dept.OrgNo + "', FK_Dept='" + dept.No + "' WHERE No='" + WebUser.No + "'");

            BP.Web.WebUser.FK_Dept = dept.No;
            BP.Web.WebUser.FK_DeptName = dept.Name;
            BP.Web.WebUser.FK_DeptNameOfFull = dept.NameOfPath;
            BP.Web.WebUser.OrgNo = dept.OrgNo;

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            emp.StartFlows = "";
            emp.Update();
            //去掉切换主部门
            /* try
             {
                 string sql = "";

                 if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                     sql = "UPDATE Port_Emp SET fk_dept='" + deptNo + "' WHERE UserID='" + WebUser.No + "' AND OrgNo='" + WebUser.OrgNo + "'";
                 else
                     sql = "UPDATE Port_Emp SET fk_dept='" + deptNo + "' WHERE No='" + WebUser.No + "'";
                 DBAccess.RunSQL(sql);
                 BP.WF.Dev2Interface.Port_Login(WebUser.No);
             }
             catch (Exception ex)
             {

             }*/

            return "@执行成功,已经切换到｛" + BP.Web.WebUser.FK_DeptName + "｝部门上。";
        }
        #endregion

        public string UserIcon_Init()
        {
            return "";
        }

        public string UserIcon_Save()
        {
            return "";
        }


        #region 修改密码.
        public string ChangePassword_Init()
        {
            if (DBAccess.IsView("Port_Emp", BP.Difference.SystemConfig.AppCenterDBType) == true)
                return "err@当前是组织结构集成模式，您不能修改密码，请在被集成的系统修改密码。";

            return "";
        }
        /// <summary>
        /// 修改密码 .
        /// </summary>
        /// <returns></returns>
        public string ChangePassword_Submit()
        {
            string oldPass = this.GetRequestVal("OldPass");
            string pass = this.GetRequestVal("Pass");

            BP.Port.Emp emp = new Emp(BP.Web.WebUser.No);
            if (emp.CheckPass(oldPass) == false)
                return "err@旧密码错误.";

            if (BP.Difference.SystemConfig.IsEnablePasswordEncryption == true)
                pass = BP.Tools.Cryptography.EncryptString(pass);
            emp.Pass = pass;
            emp.Update();

            return "密码修改成功...";
        }
        #endregion 修改密码.


        public string SetUserTheme()
        {
            string theme = this.GetRequestVal("Theme");
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            emp.SetPara("Theme", theme);
            emp.Update();

            return "设置成功";
        }

    }
}
