using System;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using BP.DA;
using BP.Web;
using BP;

namespace CCFlow.WF.CCForm
{
    public partial class DocMainAth : System.Web.UI.Page
    {
        #region 参数.
        public string FK_MapData
        {
            get
            {
                return "ND" + this.FK_Node;
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public int _fk_node = 0;
        public int FK_Node
        {
            get
            {
                if (_fk_node == 0)
                    return int.Parse(this.Request.QueryString["FK_Node"]);

                return _fk_node;
            }
            set
            {
                _fk_node = value;
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        public string IsCC
        {
            get
            {
                string paras = this.Request.QueryString["Paras"];
                if (DataType.IsNullOrEmpty(paras) == false)
                    if (paras.Contains("IsCC=1") == true)
                        return "1";
                return "ssss";
            }
        }
        public bool IsReadonly = false;
        #endregion 参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            FrmAttachment athDesc = new FrmAttachment();
            int i = athDesc.Retrieve(FrmAttachmentAttr.FK_MapData,
                this.FK_MapData, FrmAttachmentAttr.NoOfObj, "DocMainAth");
            if (i == 0)
            {
                /*如果没有数据.*/
                /*如果没有查询到它,就有可能是公文多附件被删除了.*/
                athDesc.NoOfObj = "DocMainAth";
                athDesc.Exts = "doc,docx,xls,xlsx";
                athDesc.MyPK = athDesc.FK_MapData + "_" + athDesc.NoOfObj;
                athDesc.FK_MapData = this.FK_MapData;


                //存储路径.
                string path = Server.MapPath("/DataUser/UploadFile/");
                path += "\\F" + this.FK_Flow + "MainAth";
                athDesc.SaveTo = path;

                //位置.
                athDesc.X = (float)94.09;
                athDesc.Y = (float)140.18;
                athDesc.W = (float)626.36;
                athDesc.H = (float)150;

                //多附件.
                athDesc.UploadType = AttachmentUploadType.Single;
                athDesc.Name = "公文正文(系统自动增加)";
                athDesc.SetValByKey("AtPara",
                    "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                athDesc.Insert();

                //有可能在其其它的节点上没有这个附件，所以也要循环增加上它.
                BP.WF.Nodes nds = new Nodes(this.FK_Flow);
                foreach (Node nd in nds)
                {
                    athDesc.FK_MapData = "ND" + nd.NodeID;
                    athDesc.MyPK = athDesc.FK_MapData + "_" + athDesc.NoOfObj;
                    if (athDesc.IsExits == true)
                        continue;
                    athDesc.Insert();
                }

                //重新查询一次，把默认值加上.
                athDesc.RetrieveFromDBSources();
            }

            FrmAttachmentDBs athDBs = null;
            athDBs = new FrmAttachmentDBs(this.FK_MapData, this.WorkID.ToString());

            FrmAttachmentDB athDB = null;
            if (athDBs.Count == 0 && this.IsCC == "1")
            {
                /*如果是抄送过来的, 有可能是抄送到的节点不是发送到的节点，导致附件数据没有copy。
                 * 也就是说，发给b节点，但是抄送到c节点上去了，导致c节点上的人看不到附件数据。*/

                CCList cc = new CCList();
                int nnn = cc.Retrieve(CCListAttr.FK_Node, this.FK_Node, CCListAttr.WorkID, this.WorkID, CCListAttr.CCTo, WebUser.No);
                this._fk_node = cc.FK_Node;
                if (cc.FK_Node != 0)
                {
                    athDBs.Retrieve(FrmAttachmentDBAttr.FK_MapData, "ND" + cc.FK_Node, FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());

                    string ndFromMapdata = athDesc.MyPK.Replace(athDesc.FK_MapData, "ND" + cc.FK_Node);
                    athDB = athDBs.GetEntityByKey(FrmAttachmentDBAttr.FK_FrmAttachment, ndFromMapdata) as FrmAttachmentDB;
                    //重新设置文件描述。
                    athDesc.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData, FrmAttachmentAttr.NoOfObj, "DocMainAth");
                }
            }
            else
            {
                /* 单个文件 */
                athDB = athDBs.GetEntityByKey(FrmAttachmentDBAttr.FK_FrmAttachment, athDesc.MyPK) as FrmAttachmentDB;
            }


            Label lab = new Label();
            lab.ID = "Lab" + athDesc.MyPK;
            this.Pub1.Add(lab);
            if (athDB != null)
            {
                if (athDB.FileExts == "ceb")
                    athDB.FileExts = "pdf";
                if (athDesc.IsWoEnableWF)
                    lab.Text = "<a  href=\"javascript:OpenOfiice('" + athDB.FK_FrmAttachment + "','" + this.WorkID + "','" + athDB.MyPK + "','" + this.FK_MapData + "','" + athDesc.NoOfObj + "','" + this.FK_Node + "')\"><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName + "</a>";
                else
                    lab.Text = "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName;
            }

            #region 处理权限问题.
            // 处理权限问题, 有可能当前节点是可以上传或者删除，但是当前节点上不能让此人执行工作。
            // bool isDel = athDesc.IsDeleteInt == 0 ? false : true ;
            bool isDel = athDesc.HisDeleteWay == AthDeleteWay.None ? false : true;
            bool isUpdate = athDesc.IsUpload;
            if (isDel == true || isUpdate == true)
            {
                if (this.WorkID != 0
                    && DataType.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    isDel = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No);
                    if (isDel == false)
                        isUpdate = false;
                }
            }
            #endregion 处理权限问题.

            Button mybtn = new Button();
            mybtn.CssClass = "Btn";

            if (athDesc.IsUpload && isUpdate == true)
            {
                FileUpload fu = new FileUpload();
                fu.ID = athDesc.MyPK;
                Btn_Upload.ID = "Btn_Upload_" + athDesc.MyPK + "_" + this.WorkID;
                fu.Attributes["Width"] = athDesc.W.ToString();
                fu.Attributes["onchange"] = "UploadChange('" + mybtn.ID + "');";
                this.Pub1.Add(fu);
            }

            if (athDesc.IsDownload)
            {
                mybtn = new Button();
                mybtn.Text = "下载";
                mybtn.CssClass = "Btn";

                mybtn.ID = "Btn_Download_" + athDesc.MyPK + "_" + this.WorkID;
                mybtn.Click += new EventHandler(btnUpload_Click);
                mybtn.CssClass = "bg";
                if (athDB == null)
                    mybtn.Visible = false;
                else
                    mybtn.Visible = true;
                this.Pub1.Add(mybtn);
            }

            if (this.IsReadonly == false)
            {
                if (athDesc.HisDeleteWay != AthDeleteWay.None && isDel == true)// if (athDesc.IsDeleteInt != 0 && isDel == true)
                {
                    bool isDeleteBtn = true;
                    if (athDesc.HisDeleteWay == AthDeleteWay.DelSelf) //if (athDesc.IsDeleteInt == 2)
                    {
                        if (!athDB.Rec.Equals(WebUser.No))
                            isDeleteBtn = false;
                    }
                    if (isDeleteBtn)
                    {
                        mybtn = new Button();
                        mybtn.CssClass = "Btn";
                        mybtn.Text = "删除";
                        mybtn.Attributes["onclick"] = " return confirm('您确定要执行删除吗？');";
                        mybtn.ID = "Btn_Delete_" + athDesc.MyPK + "_" + this.WorkID;
                        mybtn.Click += new EventHandler(btnUpload_Click);
                        mybtn.CssClass = "bg";
                        if (athDB == null)
                            mybtn.Visible = false;
                        else
                            mybtn.Visible = true;
                        this.Pub1.Add(mybtn);
                    }
                }

                if (athDesc.IsWoEnableWF)
                {
                    mybtn = new Button();
                    mybtn.CssClass = "Btn";
                    mybtn.Text = "打开";
                    mybtn.ID = "Btn_Open_" + athDesc.MyPK + "_" + this.WorkID;
                    mybtn.Click += new EventHandler(btnUpload_Click);
                    mybtn.CssClass = "bg";
                    if (athDB == null)
                        mybtn.Visible = false;
                    else
                        mybtn.Visible = true;
                    this.Pub1.Add(mybtn);
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string[] ids = btn.ID.Split('_');
            //string athPK = ids[2] + "_" + ids[3] ;

            string doType = ids[1];

            string athPK = btn.ID.Replace("Btn_" + doType + "_", "");
            athPK = athPK.Substring(0, athPK.LastIndexOf('_'));

            string athDBPK = athPK + "_" + this.WorkID;
            FrmAttachment frmAth = new FrmAttachment();
            frmAth.MyPK = athPK;
            frmAth.RetrieveFromDBSources();

            string pkVal = this.WorkID.ToString();
            switch (doType)
            {
                case "Delete":
                    FrmAttachmentDB db = new FrmAttachmentDB();
                    db.MyPK = athDBPK;
                    db.Delete();
                    try
                    {
                        Button btnDel = this.Pub1.GetButtonByID("Btn_Delete_" + athDBPK);
                        btnDel.Visible = false;

                        btnDel = this.Pub1.GetButtonByID("Btn_Download_" + athDBPK);
                        btnDel.Visible = false;

                        btnDel = this.Pub1.GetButtonByID("Btn_Open_" + athDBPK);
                        btnDel.Visible = false;
                    }
                    catch
                    {

                    }

                    Label lab1 = this.Pub1.GetLabelByID("Lab" + frmAth.MyPK);
                    lab1.Text = "";
                    break;
                case "Upload":
                    FileUpload fu = this.Pub1.FindControl(athPK) as FileUpload;
                    if (fu.HasFile == false || fu.FileName.Length <= 2)
                    {
                        BP.Sys.PubClass.Alert("请选择上传的文件");
                        return;
                    }

                    //检查格式是否符合要求.
                    if (frmAth.Exts == "" || frmAth.Exts == "*.*")
                    {
                        /*任何格式都可以上传.*/
                    }
                    else
                    {
                        string fileExt = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
                        fileExt = fileExt.ToLower().Replace(".", "");
                        if (frmAth.Exts.ToLower().Contains(fileExt) == false)
                        {
                            BP.Sys.PubClass.Alert("您上传的文件格式不符合要求,要求格式为:" + frmAth.Exts);
                            return;
                        }
                    }

                    //处理保存路径.
                    string saveTo = frmAth.SaveTo;
                    if (saveTo.Contains("*") || saveTo.Contains("@"))
                    {
                        /*如果路径里有变量.*/
                        saveTo = saveTo.Replace("*", "@");
                        saveTo = BP.WF.Glo.DealExp(saveTo, null, null);
                    }

                    try
                    {
                        saveTo = Server.MapPath("~/" + saveTo);
                    }
                    catch
                    {
                        //saveTo = saveTo;
                    }
                    if (System.IO.Directory.Exists(saveTo) == false)
                        System.IO.Directory.CreateDirectory(saveTo);

                    saveTo = saveTo + "\\" + athDBPK + "." + fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
                    fu.SaveAs(saveTo);


                    FileInfo info = new FileInfo(saveTo);
                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = athDBPK;
                    dbUpload.FK_FrmAttachment = athPK;
                    dbUpload.RefPKVal = this.WorkID.ToString();
                    if (this.EnName == null)
                        dbUpload.FK_MapData = this.FK_MapData;
                    else
                        dbUpload.FK_MapData = this.EnName;

                    dbUpload.FileExts = info.Extension;
                    dbUpload.FileFullName = saveTo;
                    dbUpload.FileName = fu.FileName;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.Rec = WebUser.No;
                    dbUpload.RecName = WebUser.Name;
                    dbUpload.RDT = BP.DA.DataType.CurrentDataTime;

                    if (this.Request.QueryString["FK_Node"] != null)
                        dbUpload.NodeID = this.Request.QueryString["FK_Node"];

                    dbUpload.Save();

                    Button myBtnDel = this.Pub1.GetButtonByID("Btn_Delete_" + athDBPK);
                    if (myBtnDel != null)
                    {
                        myBtnDel.Visible = true;
                        myBtnDel = this.Pub1.GetButtonByID("Btn_Download_" + athDBPK);
                        myBtnDel.Visible = true;
                    }

                    Button myBtnOpen = this.Pub1.GetButtonByID("Btn_Open_" + athDBPK);

                    if (myBtnOpen != null)
                    {
                        myBtnOpen.Visible = true;
                        myBtnOpen = this.Pub1.GetButtonByID("Btn_Download_" + athDBPK);
                        myBtnOpen.Visible = true;
                    }

                    Label lab = this.Pub1.GetLabelByID("Lab" + frmAth.MyPK);
                    if (lab != null)
                    {
                        if (frmAth.IsWoEnableWF)
                        {
                            if (dbUpload.FileExts.ToUpper().Equals("CEB"))
                            {
                                lab.Text = "<a  href=\"javascript:OpenOfiice('" + dbUpload.FK_FrmAttachment + "','" +
                                       this.WorkID + "','" + dbUpload.MyPK + "','" + this.FK_MapData + "','" +
                                       frmAth.NoOfObj + "','" + this.FK_Node + "')\"><img src='" +
                                       BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/pdf.gif' border=0/>" + dbUpload.FileName + "</a>";
                            }
                            else
                            {
                                lab.Text = "<a  href=\"javascript:OpenOfiice('" + dbUpload.FK_FrmAttachment + "','" +
                                           this.WorkID + "','" + dbUpload.MyPK + "','" + this.FK_MapData + "','" +
                                           frmAth.NoOfObj + "','" + this.FK_Node + "')\"><img src='" +
                                           BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + dbUpload.FileExts +
                                           ".gif' border=0/>" + dbUpload.FileName + "</a>";
                            }
                        }
                        else
                        {
                            if (dbUpload.FileExts.ToUpper().Equals("CEB"))
                            {
                                lab.Text = "<a  href=\"javascript:OpenFileView('" + this.WorkID + "','" + dbUpload.MyPK + "')\"><img src='" +
                                     BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/pdf.gif' border=0/>" + dbUpload.FileName + "</a>";
                            }
                            else
                            {
                                lab.Text = "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + dbUpload.FileExts + ".gif' border=0/>" + dbUpload.FileName;
                            }
                        }
                        //lab.Text = "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + dbUpload.FileExts + ".gif' alt='" + dbUpload.FileName + "' ID='" + frmAth.NoOfObj + "'  border=0/>" + dbUpload.FileName;
                    }
                    return;
                case "Download":
                    FrmAttachmentDB dbDown = new FrmAttachmentDB();
                    dbDown.MyPK = athDBPK;
                    if (dbDown.RetrieveFromDBSources() == 0)
                    {
                        dbDown.Retrieve(FrmAttachmentDBAttr.FK_MapData, this.FK_MapData,
                            FrmAttachmentDBAttr.RefPKVal, this.WorkID, FrmAttachmentDBAttr.FK_FrmAttachment, frmAth.FK_MapData + "_" + frmAth.NoOfObj);
                    }
                    string downPath = GetRealPath(dbDown.FileFullName);

                    FrmAttachment dbAtt = new FrmAttachment();
                    dbAtt.MyPK = dbDown.FK_FrmAttachment;

                    if (dbAtt.AthSaveWay  == AthSaveWay.IISServer)
                    {
                        PubClass.DownloadFile(dbDown.FileFullName, dbDown.FileName);
                    }
                    else if (dbAtt.AthSaveWay == AthSaveWay.FTPServer)
                    {
                        PubClass.DownloadHttpFile(dbDown.FileFullName, dbDown.FileName);
                    }
                    break;
                case "Open":
                    var url = BP.WF.Glo.CCFlowAppPath + "WF/WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=" + athDBPK + "&FK_FrmAttachment=" + frmAth.MyPK + "&PKVal=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_MapData=" + frmAth.FK_MapData + "&NoOfObj=" + frmAth.NoOfObj;
                    PubClass.WinOpen(url, "WebOffice编辑", 850, 600);
                    break;
                default:
                    break;
            }
        }
        private string GetRealPath(string fileFullName)
        {
            bool isFile = false;
            string downpath = "";
            try
            {
                //如果相对路径获取不到可能存储的是绝对路径
                FileInfo downInfo = new FileInfo(Server.MapPath("~/" + fileFullName));
                isFile = true;
                downpath = Server.MapPath("~/" + fileFullName);
            }
            catch (Exception)
            {
                FileInfo downInfo = new FileInfo(fileFullName);
                isFile = true;
                downpath = fileFullName;
            }
            if (!isFile)
            {
                throw new Exception("没有找到下载的文件路径！");
            }

            return downpath;
        }
        bool CanEditor(string fileType)
        {
            try
            {
                string fileTypes = BP.Sys.SystemConfig.AppSettings["OpenTypes"];

                if (fileTypes.Contains(fileType))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}