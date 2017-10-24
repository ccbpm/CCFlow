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

            if (SystemConfig.OSModel == OSModel.OneOne)
            {
                BP.Port.EmpStations des = new BP.Port.EmpStations();
                des.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Emp, WebUser.No);

                string depts = "";
                string stas = "";

                foreach (BP.Port.EmpStation item in des)
                {
                    BP.Port.Station sta = new Station(item.FK_Station);
                    stas += sta.Name + "、";
                }

                ht.Add("Depts", emp.FK_DeptText);
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
            Hashtable ht = new Hashtable();
            ht.Add("No", BP.Web.WebUser.No);
            ht.Add("Name", BP.Web.WebUser.Name);
            ht.Add("FK_Dept", BP.Web.WebUser.FK_Dept);
            ht.Add("FK_DeptName", BP.Web.WebUser.FK_DeptName);

            return BP.Tools.Json.ToJson(ht);
        }
        public string Siganture_Save()
        {
            return "";

            //FileUpload f = (FileUpload)this.FindControl("F");
            //if (f.HasFile == false)
            //    return "err@请上传文件.";

            ////if (f.FileName.EndsW

            ////判断文件类型.
            //string fileExt = ",bpm,jpg,jpeg,png,gif,";
            //string ext = f.FileName.Substring(f.FileName.LastIndexOf('.') + 1).ToLower();
            //if (fileExt.IndexOf(ext + ",") == -1)
            //{
            //    return "err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext;
            //}

            //try
            //{
            //    string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/T" + WebUser.No + ".jpg";
            //    if (System.IO.File.Exists(tempFile) == true)
            //        System.IO.File.Delete(tempFile);

            //    f.SaveAs(tempFile);
            //    System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
            //    img.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    return "err@"+ex.Message;
            //}

            //f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.No + ".jpg");
            //f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.Name + ".jpg");

            //f.PostedFile.InputStream.Close();
            //f.PostedFile.InputStream.Dispose();
            //f.Dispose();

            //   this.Response.Redirect(this.Request.RawUrl, true);
        }
        #endregion 图片签名.

        public string UserIcon_Init()
        {
            return "";
        }

        public string UserIcon_Save()
        {
            return "";
        }
    }
}
