using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;
using BP.WF;
using Microsoft.Office.Interop.Word;
using BP.WF.Template;
using BP.WF.Data;
using BP.DA;

namespace CCFlow.WF.WorkOpt
{
    public partial class WebOffice : System.Web.UI.Page
    {
        #region 属性
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        public string _MarkName = "";

        public string MarkName
        {
            get { return _MarkName; }
            set { _MarkName = value; }
        }

        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(Request["FK_Node"]);
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(Request["FID"]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public Int64 WorkID
        {
            get
            {
                try
                {
                    return int.Parse(Request["WorkID"]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public string FK_Flow
        {
            get { return Request["FK_Flow"]; }
        }

        private bool _isTrueTH = false;
        public bool IsTrueTH
        {
            get { return _isTrueTH; }
            set { _isTrueTH = value; }
        }

        private string _isTrueTHTempLate = "";

        public string IsTrueTHTemplate
        {
            get { return _isTrueTHTempLate; }
            set { _isTrueTHTempLate = value; }
        }

        public string UserName
        {
            get
            {
                try
                {
                    if (BP.Web.WebUser.No == "Guest")
                    {
                        return BP.Web.GuestUser.Name;
                    }
                    else
                    {
                        return BP.Web.WebUser.Name;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        private string heBing = "";

        public string HeBing
        {
            get { return heBing; }
            set { heBing = value; }
        }
        private bool isReadOnly = false;
        public bool ReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = true; }
        }

        private bool isCheckInfo = false;

        public bool IsCheckInfo
        {
            get { return isCheckInfo; }
            set { isCheckInfo = value; }
        }

        public string NodeInfo
        {
            get
            {
                try
                {
                    BP.WF.Node node = new BP.WF.Node(this.FK_Node);
                    return node.Name + ":" + WebUser.Name + "  日期:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch
                { return null; }
            }
        }

        private bool isSavePDF = false;

        public bool IsSavePDF
        {
            get { return isSavePDF; }
            set { isSavePDF = value; }
        }
        private bool _isMarks = false;

        public bool IsMarks
        {
            get { return _isMarks; }
            set { _isMarks = value; }
        }

        private string _officeTemplate = null;

        public string OfficeTemplate
        {
            get { return _officeTemplate; }
            set { _officeTemplate = value; }
        }

        private bool _isLoadTempLate = false;

        public bool IsLoadTempLate
        {
            get { return _isLoadTempLate; }
            set { _isLoadTempLate = value; }
        }
        public string CCFlowAppPath = BP.WF.Glo.CCFlowAppPath;


        public string FengXianURL
        {
            get { return this.Request.Url.Scheme + "://" + this.Request.Url.Authority + CCFlowAppPath + "WF/WebOffice/FengXian.aspx" + this.Request.Url.Query; }
        }

        public bool _isImpBtn = false;
        public bool IsImpBtn
        {
            get { return _isImpBtn; }
            set { _isImpBtn = value; }
        }


        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (DataType.IsNullOrEmpty(FK_Node.ToString()) || DataType.IsNullOrEmpty(WorkID.ToString()))
            {
                divMenu.InnerHtml = "<h1 style='color:red'>传入参数错误!<h1>";
                return;
            }

            if (!IsPostBack)
            {
                ReadFile();

                string type = Request["action"];
                if (DataType.IsNullOrEmpty(type))
                {
                    isReadOnly = Request.QueryString["DoType"] == "View";
                    LoadMenu(true && !isReadOnly);
                }
                else
                {
                    LoadMenu(false);
                    if (type.Equals("LoadFile"))
                        LoadFile();
                    else if (type.Equals("SaveFile"))
                        SaveFile();
                    else if (type.Equals("LoadOver"))
                        GetFileBytes();
                    else if (type.Equals("SaveBak"))
                        SaveBak();
                    else if (type.Equals("Download"))
                        DownloadFile();
                    else
                        throw new Exception("传入的参数不正确!");
                }
            }
        }
        private void SaveBak()
        {
            string result = "true";
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;

                BP.WF.Node node = new BP.WF.Node(FK_Node);

                string fileStart = WorkID.ToString() + "Mark";
                if (node.HisNodeWorkType == NodeWorkType.SubThreadWork)
                {
                    fileStart = FID.ToString();
                }

                //string file = Request["Path"];
                //file = HttpUtility.UrlDecode(file, Encoding.UTF8);

                string path = Server.MapPath("~/DataUser/OfficeFile/" + FK_Flow + "/");
                string[] haveFiles = Directory.GetFiles(path);
                string fileName = "", fileExtension = "";

                bool isHave = false;
                foreach (string file in haveFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Name.StartsWith(fileStart + "."))
                        isHave = true;

                }
                if (isHave)
                    fileStart += "." + Guid.NewGuid().ToString("N");

                if (files.Count > 0)
                {
                    ///'检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    fileName = System.IO.Path.GetFileName(postedFile.FileName);

                    if (fileName != "")
                    {
                        // if (!isHave)
                        fileExtension = System.IO.Path.GetExtension(fileName);

                        postedFile.SaveAs(path + "\\" + fileStart + fileExtension);
                        this.MarkName = fileStart + fileExtension;
                    }
                }

            }
            catch  
            {
                result = "false";
            }

            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
        }

        /// <summary>
        /// 获取套红文件的记录
        /// </summary>
        private void GetFileBytes()
        {
            int name = int.Parse(HttpUtility.UrlDecode(Request.QueryString["fileName"], System.Text.Encoding.UTF8));

            string type = Request.QueryString["type"];
            string realFileName = "";
            string path = Server.MapPath("~/DataUser/OfficeOverTemplate/");
            if (type == "1")
            {
                string[] files = System.IO.Directory.GetFiles(path);
                int i = 0;
                foreach (string fileName in files)
                {
                    if (i == name)
                    {
                        realFileName = fileName;
                    }
                    i++;
                }
            }
            else
            {
                realFileName = Server.MapPath("~/DataUser/OfficeOverTemplate/" + IsTrueTHTemplate);
            }

            var result = File.ReadAllBytes(realFileName);

            Response.Clear();
            Response.BinaryWrite(result);
            Response.End();

        }

        private void ReadFile()
        {
            string path = Server.MapPath("~/DataUser/OfficeFile/" + FK_Flow + "/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] files = Directory.GetFiles(path);
            bool isHave = false;
            BP.WF.Node node = new BP.WF.Node(FK_Node);




            string fileStart = WorkID.ToString();
            if (node.HisNodeWorkType == NodeWorkType.SubThreadWork)
            {
                fileStart = FID.ToString();
            }

            try
            {
                WorkFlow workflow = new WorkFlow(this.FK_Flow, this.WorkID);

                if (workflow.HisGenerWorkFlow.PWorkID != 0)
                {
                    BtnLab btnLab = new BtnLab(this.FK_Node);
                    if (btnLab.OfficeIsParent)
                        fileStart = workflow.HisGenerWorkFlow.PWorkID.ToString();
                }


            }
            catch (Exception)
            {


            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.StartsWith(fileStart + ".") && fileInfo.Name.Contains(".doc"))
                {
                    fileName.Text = fileInfo.Name;
                    fileType.Text = fileInfo.Extension.TrimStart('.');
                    isHave = true;
                    break;
                }
            }

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Name.StartsWith(fileStart + "Mark."))
                {
                    MarkName = fileInfo.Name;
                    break;
                }
            }
            if (!isHave)
            {
                if (node.IsStartNode)
                {

                    //加载template
                    BtnLab btnLab = new BtnLab(this.FK_Node);
                    OfficeTemplate = btnLab.OfficeTemplate;

                    if (!DataType.IsNullOrEmpty(OfficeTemplate))
                    {
                        fileName.Text = "/" + OfficeTemplate;
                        fileType.Text = OfficeTemplate.Split('.')[1];
                        IsLoadTempLate = true;
                    }
                }

                //if (node.HisNodeWorkType == NodeWorkType.SubThreadWork)
                //{
                //    File.Exists(path+)
                //    foreach (string file in files)
                //    {
                //        FileInfo fileInfo = new FileInfo(file);
                //        if (fileInfo.Name.StartsWith(this.FID.ToString()))
                //        {
                //            fileInfo.CopyTo(path + "\\" + this.WorkID + fileInfo.Extension);
                //            fileName.Text = this.WorkID + fileInfo.Extension;
                //            fileType.Text = fileInfo.Extension.TrimStart('.');
                //            break;
                //        }
                //    }
                //}
            }
            else
            {

                //    if (node.HisNodeWorkType == NodeWorkType.WorkHL || node.HisNodeWorkType == NodeWorkType.WorkFHL)
                //    {

                //        GenerWorkFlows generWorksFlows = new GenerWorkFlows();
                //        generWorksFlows.RetrieveByAttr(GenerWorkFlowAttr.FID, this.WorkID);
                //        string tempH = "";
                //        foreach (GenerWorkFlow generWork in generWorksFlows)
                //        {
                //            tempH += generWork.WorkID + ",";
                //        }
                //        HeBing = tempH.TrimEnd(',');
                //    }
            }
        }

        private void LoadFile()
        {
            try
            {
                string loadType = Request["LoadType"];
                string type = fileType.Text;
                string name = Request["fileName"];
                string path = null;
                if (loadType.Equals("1"))
                {
                    path = Server.MapPath("~/DataUser/OfficeFile/" + FK_Flow + "/" + name);
                }
                else
                {
                    path = Server.MapPath("~/DataUser/OfficeTemplate/" + name);
                }

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

        private void DownloadFile()
        {
            if(string.IsNullOrWhiteSpace(FK_Flow) || this.WorkID == 0)
            {
                Response.Write("参数不完整，必须具备FK_Flow和WorkID参数。");
                Response.End();
                return;
            }

            string docPath = Server.MapPath("~/DataUser/OfficeFile/" + FK_Flow + "/");
            FileInfo docFile = null;
            FileInfo[] docFiles = new DirectoryInfo(docPath).GetFiles(this.WorkID + ".doc*");
            if (docFiles.Length > 0)
            {
                docFile = docFiles[0];
            }

            if(docFile == null)
            {
                Response.Write("未找到公文！");
                Response.End();
                return;
            }

            BP.Sys.PubClass.DownloadFile(docFile.FullName, docFile.Name);
            this.WinClose();
        }

        protected void WinClose()
        {
            this.Response.Write("<script language='JavaScript'> window.close();</script>");
        }

        private void LoadAttachment()
        {
            string EnName = "ND" + this.FK_Node;
            BP.Sys.MapData mapdata = new BP.Sys.MapData(EnName);

            FrmAttachments attachments = new BP.Sys.FrmAttachments();

            attachments = mapdata.FrmAttachments;


            bool isCompleate = false;
            BP.WF.Node node = new BP.WF.Node(FK_Node);
            try
            {
                WorkFlow workFlow = new WorkFlow(node.FK_Flow, WorkID);
                isCompleate = workFlow.IsComplete;

            }
            catch (Exception)
            {
                try
                {
                    Flow fl = new Flow(node.FK_Flow);
                    GERpt rpt = fl.HisGERpt;
                    rpt.OID = WorkID;
                    rpt.Retrieve();

                    if (rpt != null)
                    {
                        if (rpt.WFState == WFState.Complete)
                            isCompleate = true;
                    }
                }
                catch
                {

                }
            }



            foreach (FrmAttachment ath in attachments)
            {
                string src = "";
                if (!isCompleate)
                    src = CCFlowAppPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + EnName + "&FK_FrmAttachment=" + ath.MyPK + "&FK_Node=" + this.FK_Node;
                else
                    src = CCFlowAppPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.WorkID + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + EnName + "&FK_FrmAttachment=" + ath.MyPK + "&FK_Node=" + this.FK_Node + "&IsReadonly=1";

                this.Pub1.Add("<iframe ID='F" + ath.MyPK + "'    src='" + src + "' frameborder=0  style='position:absolute;width:" + ath.W + "px; height:" + ath.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

            }
        }

        private void LoadMenu(bool isMenu)
        {
            BtnLab btnLab = new BtnLab(this.FK_Node);
            bool isCompleate = false;
            BP.WF.Node node = new BP.WF.Node(FK_Node);
            try
            {
                WorkFlow workFlow = new WorkFlow(node.FK_Flow, WorkID);
                isCompleate = workFlow.IsComplete;

            }
            catch (Exception)
            {
                try
                {
                    Flow fl = new Flow(node.FK_Flow);
                    GERpt rpt = fl.HisGERpt;
                    rpt.OID = WorkID;
                    rpt.Retrieve();

                    if (rpt != null)
                    {
                        if (rpt.WFState == WFState.Complete)
                            isCompleate = true;
                    }
                }
                catch
                {

                }
            }



            if (isMenu && !isCompleate)
            {
                if (btnLab.OfficeMarksEnable)
                {
                    divMenu.InnerHtml =
                        "查看留痕:<select id='marks' onchange='ShowUserName()'  style='width: 100px'><option value='显示留痕'>显示留痕</option><option value='隐藏留痕'>隐藏留痕</option><select>";
                }
                if (btnLab.OfficeOpenEnable)
                {

                    AddBtn("openFile", btnLab.OfficeOpenLab, "OpenFile");

                }
                if (btnLab.OfficeOpenTemplateEnable)
                {
                    AddBtn("openTempLate", btnLab.OfficeOpenTemplateLab, "OpenTempLate");

                }
                if (btnLab.OfficeSaveEnable)
                {

                    AddBtn("saveFile", btnLab.OfficeSaveLab, "saveOffice");

                }
                if (btnLab.OfficeAcceptEnable)
                {

                    AddBtn("accept", btnLab.OfficeAcceptLab, "acceptOffice");

                }
                if (btnLab.OfficeRefuseEnable)
                {

                    AddBtn("refuse", btnLab.OfficeRefuseLab, "refuseOffice");

                }
                if (btnLab.OfficeOverEnable)
                {
                    AddBtn("over", btnLab.OfficeOverLab, "overOffice");

                }

                if (btnLab.OfficeSealEnable)
                {
                    AddBtn("seal", btnLab.OfficeSealLab, "sealOffice");

                }

                if (btnLab.OfficeInsertFlowEnable)
                {
                    AddBtn("flow", btnLab.OfficeInsertFlowLab, "InsertFlow");

                }

                if (btnLab.OfficeDownEnable)
                {
                    AddBtn("download", btnLab.OfficeDownLab, "DownLoad");

                }

                if (btnLab.OfficeIsMarks)
                    IsMarks = true;
                if (btnLab.OfficeNodeInfo)
                    IsCheckInfo = true;
            }
           // OfficeTemplate = btnLab.OfficeTemplate;
            IsSavePDF = btnLab.OfficeReSavePDF;

            if (!DataType.IsNullOrEmpty(this.MarkName))
                AddBtn("ViewMarks", "文档痕迹", "ViewMark");

            if (isMenu)
            {
                LoadAttachment();
            }
        }

        protected void impBtn_Click(object sender, EventArgs e)
        {

            Flow flow = new Flow(this.FK_Flow);

            GERpt rpt = flow.HisGERpt;

            rpt.RetrieveByAttr(GERptAttr.OID, this.WorkID);

            string zhiduNo = rpt.GetValStringByKey("ZhiDuNo");


            string filePath = AppDomain.CurrentDomain.BaseDirectory + ("/DataUser/OfficeFile/" + this.FK_Flow + "/" + this.WorkID + ".doc");

            if (!File.Exists(filePath))
                filePath = AppDomain.CurrentDomain.BaseDirectory + ("/DataUser/OfficeFile/" + this.FK_Flow + "/" + this.WorkID + ".docx");


            string url = BP.Sys.SystemConfig.AppSettings["HostURL"] + "/ZhiDu/UploadHander.ashx?zhiduNo=" + zhiduNo + "&IsSendXCBank=0";
            byte[] result = null;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/octet-stream");
                // wc.Headers.Add(HttpRequestHeader.UserAgent, "Mwwozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                result = client.UploadFile(url, "POST", filePath);
            }
            LoadMenu(true);
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
            string message = "true";
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;
                BP.WF.Node node = new BP.WF.Node(FK_Node);
                string fileStart = WorkID.ToString();
                if (node.HisNodeWorkType == NodeWorkType.SubThreadWork)
                    fileStart = FID.ToString();

                //string file = Request["Path"];
                //file = HttpUtility.UrlDecode(file, Encoding.UTF8);

                string path = Server.MapPath("~/DataUser/OfficeFile/" + FK_Flow + "/");
                string[] haveFiles = Directory.GetFiles(path);
                string fileName = "", fileExtension = "";

                foreach (string file in haveFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Name.StartsWith(fileStart + "."))
                    {

                        fileInfo.Delete();
                    }
                }

                if (files.Count > 0)
                {
                    ///'检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    fileName = System.IO.Path.GetFileName(postedFile.FileName);

                    if (fileName != "")
                    {
                        //if (!isHave)
                        fileExtension = System.IO.Path.GetExtension(fileName);



                        postedFile.SaveAs(path + "\\" + fileStart + fileExtension);


                        if (IsSavePDF)
                        {
                            using (WebClient wc = new WebClient())
                            {
                                string url = "http://" + Request.Url.Authority + BP.WF.Glo.CCFlowAppPath + "WF/WebOffice/OfficeServices.ashx";
                                Uri uri = new Uri(url);


                                string json = "Start=" + fileStart + "&Path=" + path + "&Extension=" + fileExtension + "&Type=savePDF";
                                //"{\"Start\":\"" + fileStart + "\",\"Path\":\"" + path + "\",\"Extension\":\"" + fileExtension + "\",\"Type\":\"savePDF\"}";
                                wc.Encoding = System.Text.Encoding.UTF8;

                                NameValueCollection value = new NameValueCollection();
                                value.Add("Start", fileStart);
                                value.Add("Path", path);
                                value.Add("Extension", fileExtension);
                                value.Add("Type", "savePDF");
                                wc.QueryString = value;
                                wc.UploadStringAsync(uri, "PUT", json, wc);
                            }
                        }
                    }
                    #region 标示已经自动套红
                    if (IsTrueTH)
                    {
                        //BP.Data.XCWordOver wordOver = new BP.Data.XCWordOver();
                      //  wordOver.ID = int.Parse(this.WorkID.ToString());
                      //  wordOver.IsOVer = true;
                      //  wordOver.Insert();
                    }
                    #endregion 标示已经自动套红

                    //try
                    //{


                    //    Microsoft.Office.Interop.Word.Application appClass = new Microsoft.Office.Interop.Word.Application();
                    //    appClass.Visible = false;

                    //    Object missing = System.Reflection.Missing.Value;

                    //    object fileNameR = path + "\\" + fileStart + fileExtension;
                    //    var wordDoc = appClass.Documents.Open(ref fileNameR, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);


                    //    object format = WdSaveFormat.wdFormatPDF;
                    //    object savePath = path + "\\" + fileStart + ".pdf";
                    //    wordDoc.SaveAs(ref savePath, ref format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                    //    wordDoc.Close();

                    //}
                    //catch (Exception ex)
                    //{

                    //}
                }
            }
            catch (Exception ex)
            {
                message = ex.Message.ToString();
            }
            this.Response.Clear();
            this.Response.Write(message);
            this.Response.End();
        }
    }
}