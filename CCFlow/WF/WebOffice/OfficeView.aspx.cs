using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;
using BP.WF;
using Microsoft.Office.Interop.Word;
using BP.DA;



namespace CCFlow.WF.WebOffice
{
    public partial class OfficeView : System.Web.UI.Page
    {
        #region 属性

        public bool IsEdit
        {
            get
            {
                string _isEdit = Request["IsEdit"];
                if (_isEdit == "1")
                    return true;
                else
                    return false;
            }
        }

        public string Path
        {
            get
            {
                string _path = getUTF8ToString("Path");
                return _path;
            }

        }


        public string CCFlowAppPath = BP.WF.Glo.CCFlowAppPath;

        #endregion

        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (DataType.IsNullOrEmpty(Path) || !File.Exists(Server.MapPath("~/" + Path)))
            {
                divMenu.InnerHtml = "<h1 style='color:red'>您传入的路径不正确!<h1>";
                return;
            }

            if (!IsPostBack)
            {
                string type = Request["action"];
                if (DataType.IsNullOrEmpty(type))
                {
                    LoadMenu(true);
                    ReadFile();
                }
                else
                {
                    LoadMenu(false);
                    if (type.Equals("LoadFile"))
                        LoadFile();
                    else if (type.Equals("SaveFile"))
                        SaveFile();
                    else
                        throw new Exception("传入的参数不正确!");
                }
            }
        }

        private void ReadFile()
        {
            string path = Server.MapPath("~/" + Path);

            FileInfo fileInfo = new FileInfo(path);

            fileName.Text = fileInfo.Name;
            fileType.Text = fileInfo.Extension.TrimStart('.');

        }

        private void LoadFile()
        {
            try
            {
                string loadType = Request["LoadType"];
                string type = fileType.Text;
                string name = getUTF8ToString("fileName");
                string path = Server.MapPath("~/" + Path);
               
                var result = File.ReadAllBytes(path);

                Response.Clear();

                Response.BinaryWrite(result);
                Response.End();
            }
            catch (Exception)
            {

                throw;
            }

        }


        private void LoadMenu(bool isMenu)
        {

            if (IsEdit)
            {
                AddBtn("saveFile", "保存", "saveOffice");
            }
            AddBtn("download", "下载", "DownLoad");
        }

        private void AddBtn(string id, string label, string clickEvent)
        {
            Btn btn = new Btn();
            btn.ID = id;
            btn.Text = label;
            btn.Attributes["onclick"] = "return " + clickEvent + "()";
            btn.Attributes["class"] = "btn";
            divMenu.Controls.Add(btn);
        }


        private void SaveFile()
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;

                

                if (files.Count > 0)
                {
                    ///'检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    string fileName, fileExtension;
                    fileName = System.IO.Path.GetFileName(postedFile.FileName);
                    string path = Server.MapPath("~/"+Path);

                    if (fileName != "")
                    {
                        fileExtension = System.IO.Path.GetExtension(fileName);

                        postedFile.SaveAs(path);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}