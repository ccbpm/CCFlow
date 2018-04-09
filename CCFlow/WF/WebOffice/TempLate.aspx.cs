using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft;
using Newtonsoft.Json;
using BP.DA;

namespace CCFlow.WF.WebOffice
{
    public partial class TempLate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string load = Request["load"];
            if (!DataType.IsNullOrEmpty(load))
            {
                if (load.Equals("true"))
                {
                    LoadJson();
                }
            }
        }

        private void LoadJson()
        {

            string type = Request["LoadType"];
            string loadType = Request["Type"];
            string fk_flow = Request["FK_Flow"];
            string workID = Request["WorkID"];
            string json = "";

            List<OfficeTemplate> list = new List<OfficeTemplate>();
            string path = "";
            if (type.Equals("word"))
            {
                path = Server.MapPath("~/DataUser/OfficeTemplate");
            }
            else if (type.Equals("over"))
            {
                path = Server.MapPath("~/DataUser/OfficeOverTemplate");
            }
            else if (type.Equals("seal"))
            {
                path = Server.MapPath("~/DataUser/OfficeSeal");
            }
            else if (type.Equals("flow"))
            {
                path = Server.MapPath("~/DataUser/FlowDesc");
            }
            else if (type.Equals("marks"))
            {
                path = Server.MapPath("~/DataUser/OfficeFile/" + fk_flow);
            }


            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (loadType.Equals("File"))
            {
                string[] files = System.IO.Directory.GetFiles(path);
                int i = 0;
                foreach (string fileName in files)
                {
                    FileInfo file = new FileInfo(fileName);
                    OfficeTemplate template = new OfficeTemplate();
                    template.Name = file.Name;
                    template.Type = file.Extension.TrimStart('.');
                    template.Size = file.Length / 1024 + "";
                    template.Index = i;
                    i++;
                    list.Add(template);
                }
                json = "{\"total\":" + list.Count + ",\"rows\":" + JsonConvert.SerializeObject(list) + "}";
            }
            else if (loadType.Equals("Dic"))
            {
                string[] dics = System.IO.Directory.GetDirectories(path);

                foreach (string fileName in dics)
                {
                    DirectoryInfo dicInfo = new DirectoryInfo(fileName);
                    OfficeTemplate template = new OfficeTemplate();
                    template.Name = dicInfo.Name;
                    template.Type = dicInfo.Extension.TrimStart('.');
                    template.Size = "无";

                    list.Add(template);
                }
                json = "{\"total\":" + list.Count + ",\"rows\":" + JsonConvert.SerializeObject(list) + "}";
            }
            else if (loadType.Equals("marks"))
            {
                string[] files = System.IO.Directory.GetFiles(path);

                int i = 0;
                foreach (string fileName in files)
                {
                    FileInfo file = new FileInfo(fileName);

                    if (!file.Name.StartsWith(workID + "Mark"))
                        continue;
                    OfficeTemplate template = new OfficeTemplate();
                    template.Name = "文档修订痕迹" + i;
                    template.Type = file.Extension.TrimStart('.');
                    template.RealName = file.Name;
                    template.Size = file.Length / 1024 + "";
                    template.Index = i;
                    i++;
                    list.Add(template);
                }
                json = "{\"total\":" + list.Count + ",\"rows\":" + JsonConvert.SerializeObject(list) + "}";

            }
            this.Page.Controls.Clear();
            Response.Clear();

            Response.Write(json);
        }
    }

    public class OfficeTemplate
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string RealName { get; set; }
        public int Index { get; set; }
    }
}