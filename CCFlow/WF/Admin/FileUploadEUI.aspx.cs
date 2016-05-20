using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace CCFlow.WF
{
    public partial class FileUploadEUI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string load = Request["type"];
            if (!string.IsNullOrEmpty(load))
            {
                if (load.Equals("load"))
                {
                    LoadJson();
                }
                else if (load.Equals("delete"))
                {
                    DeleteFile();
                }
            }
            
        }

        private void DeleteFile()
        {
            string type = Request["LoadType"];
            string name = Request["name"];

            string path = Server.MapPath("~/DataUser/" + type);
            string message = "";
            try
            {
                File.Delete(path + "\\" + name);
                message = "true";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Response.Clear();
            Response.Write(message);
            Response.End();
        }

      

        private void LoadJson()
        {

            string type = Request["LoadType"];
            List<OfficeTemplate> list = new List<OfficeTemplate>();
            string path = "";

            path = Server.MapPath("~/DataUser/" + type);
          

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] files = System.IO.Directory.GetFiles(path);

            foreach (string fileName in files)
            {
                FileInfo file = new FileInfo(fileName);
                OfficeTemplate template = new OfficeTemplate();
                template.Name = file.Name;
                template.Type = file.Extension.TrimStart('.');
                template.Size = file.Length / 1024 + "";

                list.Add(template);
            }
            string json = "{\"total\":" + list.Count + ",\"rows\":" + JsonConvert.SerializeObject(list) + "}";

       
            Response.Clear();

            Response.Write(json);
            Response.End();
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            string type = LoadType.SelectedValue;

            string path = Server.MapPath("~/DataUser/" + type);
            try
            {
                fileUpload.SaveAs(path + "\\" + fileUpload.FileName);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
            }
        }
    }

    public class OfficeTemplate
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
    }
}