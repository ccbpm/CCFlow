using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web;

namespace GPMApp.GPM
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 变量.
        public HttpContext context = null;
        public string DoType
        {
            get
            {
                return context.Request.QueryString["DoType"];
            }
        }
        public string FK_Emp
        {
            get
            {
                string str= context.Request.QueryString["FK_Emp"];
                if (str==null)
                    str = context.Request.QueryString["EmpNo"];

                return str;
            }
        }
        #endregion 变量.

        public void ProcessRequest(HttpContext hc)
        {
            context = hc;
            string info = "";
            if (this.DoType == "Siganture_Init")
                info= Siganture_Init();
            if (this.DoType == "Siganture_Save")
                info = Siganture_Save();

            //输出信息.
            this.OutText(info);
        }
        public string Siganture_Init()
        {
            if (BP.Web.WebUser.NoOfRel == null)
                return "err@登录信息丢失";

            BP.Port.Emp emp = new BP.Port.Emp(this.FK_Emp);

            Hashtable ht = new Hashtable();
            ht.Add("No", emp.No);
            ht.Add("Name", emp.Name);
            ht.Add("FK_Dept", emp.FK_Dept);
            ht.Add("FK_DeptName", emp.FK_DeptText);
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
                string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg";
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

            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp+ ".jpg");
            // f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.Name + ".jpg");

            //f.PostedFile.InputStream.Close();
            //f.PostedFile.InputStream.Dispose();
            //f.Dispose();

            //   this.Response.Redirect(this.Request.RawUrl, true);
            return "上传成功！";
        }

        public void OutText(string info)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(info);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}