using System;
using System.Collections.Generic;
using System.Collections;
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
    public class WF_Setting : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Setting(HttpContext mycontext)
        {
            this.context = mycontext;
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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        public string Default_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", WebUser.No);
            ht.Add("UserName", WebUser.Name);

            BP.Port.Emp emp = new Emp();
            emp.No = WebUser.No;
            emp.Retrieve();

            //部门名称.
            ht.Add("DeptName", emp.FK_DeptText);

            if (SystemConfig.OSModel == OSModel.OneMore)
            {
                BP.GPM.DeptEmpStations des = new BP.GPM.DeptEmpStations();
                des.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Emp, WebUser.No);

                string depts = "";
                string stas = "";

                foreach (BP.GPM.DeptEmpStation item in des)
                {
                    BP.Port.Dept dept = new Dept(item.FK_Dept);
                    depts += dept.Name + "、";


                    BP.Port.Station sta = new Station(item.FK_Station);
                    stas += sta.Name + "、";
                }

                ht.Add("Depts", depts);
                ht.Add("Stations", stas);
            }

           

            BP.WF.Port.WFEmp wfemp = new Port.WFEmp(WebUser.No);
            ht.Add("Tel", wfemp.Tel);
            ht.Add("Email", wfemp.Email);
            return BP.Tools.Json.ToJson(ht);
        }

        #region 图片签名.
        public string Siganture_Init()
        {
            if (BP.Web.WebUser.NoOfRel == null)
                return "err@登录信息丢失";

            Hashtable ht = new Hashtable();
            ht.Add("No", BP.Web.WebUser.No);
            ht.Add("Name", BP.Web.WebUser.Name);
            ht.Add("FK_Dept", BP.Web.WebUser.FK_Dept);
            ht.Add("FK_DeptName", BP.Web.WebUser.FK_DeptName);
            return BP.Tools.Json.ToJson(ht);
        }
        public string Siganture_Save()
        {
            HttpPostedFile f = context.Request.Files[0];

            //判断文件类型.
            string fileExt = ",bpm,jpg,jpeg,png,gif,";
            string ext = f.FileName.Substring(f.FileName.LastIndexOf('.') + 1).ToLower();
            if (fileExt.IndexOf(ext + ",") == -1)
            {
                return "err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext;
            }

            try
            {
                string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/T" + WebUser.No + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                f.SaveAs(tempFile);
                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.No + ".jpg");
           // f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.Name + ".jpg");

            //f.PostedFile.InputStream.Close();
            //f.PostedFile.InputStream.Dispose();
            //f.Dispose();

            //   this.Response.Redirect(this.Request.RawUrl, true);
            return "上传成功！";
        }
        #endregion 图片签名.

        #region 切换部门.
        /// <summary>
        /// 初始化切换部门.
        /// </summary>
        /// <returns></returns>
        public string ChangeDept_Init()
        {
            string sql = "SELECT a.No,a.Name, NameOfPath, '0' AS  CurrentDept FROM Port_Dept A, Port_DeptEmp B WHERE A.No=B.FK_Dept AND B.FK_Emp='" + BP.Web.WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["CURRENTDEPT"].ColumnName = "CurrentDept";
                dt.Columns["NAMEOFPATH"].ColumnName = "NameOfPath";
            }

            //设置当前的部门.
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["No"].ToString() == WebUser.FK_Dept)
                    dr["CurrentDept"] = "1";

                if (dr["NameOfPath"].ToString() != "")
                    dr["Name"] = dr["NameOfPath"];
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
            BP.GPM.Dept dept = new GPM.Dept(deptNo);

            BP.Web.WebUser.FK_Dept = dept.No;
            BP.Web.WebUser.FK_DeptName = dept.Name;
            BP.Web.WebUser.FK_DeptNameOfFull = dept.NameOfPath;

            //重新设置cookies.
            string strs = "";
            strs += "@No=" + WebUser.No;
            strs += "@Name=" + WebUser.Name;
            strs += "@FK_Dept=" + WebUser.FK_Dept;
            strs += "@FK_DeptName=" + WebUser.FK_DeptName;
            strs += "@FK_DeptNameOfFull=" + WebUser.FK_DeptNameOfFull;
            BP.Web.WebUser.SetValToCookie(strs);

            BP.WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            emp.StartFlows = "";
            emp.Update();

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
            if (BP.DA.DBAccess.IsView("Port_Emp") == true)
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

            if (BP.Sys.SystemConfig.IsEnablePasswordEncryption == true)
                    pass = BP.Tools.Cryptography.EncryptString(pass);
            emp.Pass = pass;
            emp.Update();

            return "密码修改成功...";
        }
        #endregion 修改密码.

    }
}
