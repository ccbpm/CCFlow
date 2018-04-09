using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
using BP.Tools;
namespace CCFlow.WF.MapDef.MapExtUI
{
    public partial class WordFrmUI : BP.Web.WebPage
    {
        #region 属性。
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 操作的Key
        /// </summary>
        public string OperAttrKey
        {
            get
            {
                return this.Request.QueryString["OperAttrKey"];
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string ExtType
        {
            get
            {
                return MapExtXmlList.DDLFullCtrl;
            }
        }
        public string Lab = null;
        #endregion 属性。
        
        protected void Page_Load(object sender, EventArgs e)
        {
            MapData ath = new MapData(this.FK_MapData);

            #region WebOffice控制方式.
            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("colspan=3", "WebOffice控制方式.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableWF;
            cb.Text = "是否启用weboffice？";
            cb.Checked = ath.IsWoEnableWF;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableSave;
            cb.Text = "是否启用保存？";
            cb.Checked = ath.IsWoEnableSave;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableReadonly;
            cb.Text = "是否只读？";
            cb.Checked = ath.IsWoEnableReadonly;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableRevise;
            cb.Text = "是否启用修订？";
            cb.Checked = ath.IsWoEnableRevise;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark;
            cb.Text = "是否查看用户留痕？";
            cb.Checked = ath.IsWoEnableViewKeepMark;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnablePrint;
            cb.Text = "是否打印？";
            cb.Checked = ath.IsWoEnablePrint;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableOver;
            cb.Text = "是否启用套红？";
            cb.Checked = ath.IsWoEnableOver;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableSeal;
            cb.Text = "是否启用签章？";
            cb.Checked = ath.IsWoEnableSeal;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableTemplete;
            cb.Text = "是否启用模板文件？";
            cb.Checked = ath.IsWoEnableTemplete;
            this.Pub1.AddTD(cb);

            this.Pub1.AddTREnd();
            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableCheck;
            cb.Text = "是否记录节点信息？";
            cb.Checked = ath.IsWoEnableCheck;
            this.Pub1.AddTD(cb);
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow;
            cb.Text = "是否启用插入流程？";
            cb.Checked = ath.IsWoEnableInsertFlow;
            this.Pub1.AddTD(cb);
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian;
            cb.Text = "是否启用插入风险点？";
            cb.Checked = ath.IsWoEnableInsertFengXian;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTR();
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableMarks;
            cb.Text = "是否进入留痕模式？";
            cb.Checked = ath.IsWoEnableMarks;
            this.Pub1.AddTD(cb);
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsWoEnableDown;
            cb.Text = "是否启用下载？";
            cb.Checked = ath.IsWoEnableDown;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            #endregion WebOffice控制方式.

            //确定模板文件
            string moduleFile = getModuleFile(new[] { ".doc", ".docx" });

            Pub1.AddTR();
            Pub1.AddTDBegin("colspan='3'");
            this.Pub1.Add("模版文件(必须是*.doc/*.docx文件):");

            Literal lit = new Literal();
            lit.ID = "litInfo";

            if (!DataType.IsNullOrEmpty(moduleFile))
            {
                lit.Text = "[<span style='color:green'>已上传Word表单模板:<a href='" + moduleFile +
                           "' target='_blank' title='下载或打开模版'>" + moduleFile +
                           "</a></span>]<br /><br />";

                this.Pub1.Add(lit);
            }
            else
            {
                lit.Text = "[<span style='color:red'>还未上传Word表单模板</span>]<br /><br />";
                this.Pub1.Add(lit);
            }

            FileUpload fu = new FileUpload();
            fu.ID = "FU";
            fu.Width = 300;
            this.Pub1.Add(fu);
            this.Pub1.AddSpace(2);
            LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveWordFrm_Click);
            this.Pub1.Add(btn);

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void btn_SaveWordFrm_Click(object sender, EventArgs e)
        {
            MapData ath = new MapData(this.FK_MapData);
            ath.IsWoEnableWF = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableWF).Checked;
            ath.IsWoEnableSave = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSave).Checked;
            ath.IsWoEnableReadonly = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableReadonly).Checked;
            ath.IsWoEnableRevise = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableRevise).Checked;
            ath.IsWoEnableViewKeepMark = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableViewKeepMark).Checked;
            ath.IsWoEnablePrint = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnablePrint).Checked;
            ath.IsWoEnableSeal = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableSeal).Checked;
            ath.IsWoEnableOver = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableOver).Checked;
            ath.IsWoEnableTemplete = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableTemplete).Checked;
            ath.IsWoEnableCheck = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableCheck).Checked;
            ath.IsWoEnableInsertFengXian = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFengXian).Checked;
            ath.IsWoEnableInsertFlow = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableInsertFlow).Checked;
            ath.IsWoEnableMarks = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableMarks).Checked;
            ath.IsWoEnableDown = this.Pub1.GetCBByID("CB_" + FrmAttachmentAttr.IsWoEnableDown).Checked;
            ath.Update();

            FileUpload fu = this.Pub1.FindControl("FU") as FileUpload;
            if (!string.IsNullOrWhiteSpace(fu.FileName))
            {
                string[] extArr = new[] { ".doc", ".docx" };
                string ext = Path.GetExtension(fu.FileName).ToLower();
                if (!extArr.Contains(ext))
                {
                    Response.Write("<script>alert('Word表单模板只能上传*.doc/*.docx两种格式的文件！');history.back();</script>");
                    return;
                }

               string  moduleFile = getModuleFile(extArr);

                if (!DataType.IsNullOrEmpty(moduleFile))
                    File.Delete(Server.MapPath(moduleFile));

                moduleFile = SystemConfig.PathOfDataUser + "FrmOfficeTemplate\\" + this.FK_MapData +
                             Path.GetExtension(fu.FileName);

                fu.SaveAs(moduleFile);
                moduleFile = moduleFile.Substring(moduleFile.IndexOf("\\DataUser\\")).Replace("\\", "/");
                Literal lit = this.Pub1.FindControl("litInfo") as Literal;

                if (lit != null)
                {
                    lit.Text = "[<span style='color:green'>已上传Word表单模板:<a href='" + moduleFile +
                               "' target='_blank' title='下载或打开模版'>" + moduleFile +
                               "</a></span>]<br /><br />";
                }
            }
        }

        /// <summary>
        /// 获取上传的模板文件相对路径
        /// </summary>
        /// <returns></returns>
        private string getModuleFile(string[] extArr)
        {
            string dir = Server.MapPath("/DataUser/FrmOfficeTemplate/");
            FileInfo[] files = new DirectoryInfo(dir).GetFiles(this.FK_MapData + ".*");
            string moduleFile = string.Empty;

            foreach (var file in files)
            {
                if (extArr.Contains(file.Extension.ToLower()))
                {
                    moduleFile = file.FullName.Replace("\\", "/");
                    moduleFile = moduleFile.Substring(moduleFile.IndexOf("/DataUser/"));
                    break;
                }
            }

            return moduleFile;
        }
    }
}