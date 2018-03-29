using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
    public partial class ExcelFrmUI : BP.Web.WebPage
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
            BP.Sys.ToolbarExcel en = new ToolbarExcel(this.FK_MapData);


            //确定模板文件
            string moduleFile = getModuleFile(new[] { ".xls", ".xlsx" });
            this.Pub1.AddEasyUiPanelInfoBegin("Excel表单属性");
            this.Pub1.Add("模版文件(必须是*.xls/*.xlsx文件):");

            Literal lit = new Literal();
            lit.ID = "litInfo";

            if (!string.IsNullOrEmpty(moduleFile))
            {
                lit.Text = "[<span style='color:green'>已上传Excel表单模板:<a href='" + moduleFile +
                           "' target='_blank' title='下载或打开模版'>" + moduleFile +
                           "</a></span>]<br /><br />";

                this.Pub1.Add(lit);
            }
            else
            {
                lit.Text = "[<span style='color:red'>还未上传Excel表单模板</span>]<br /><br />";
                this.Pub1.Add(lit);
            }

            FileUpload fu = new FileUpload();
            fu.ID = "FU";
            fu.Width = 300;
            this.Pub1.Add(fu);
            this.Pub1.AddSpace(2);
            LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveExcelFrm_Click);
            this.Pub1.Add(btn);
            this.Pub1.AddSpace(2);
            this.Pub1.Add(
                string.Format(
                    "<a href=\"javascript:OpenEasyUiDialog('/WF/Comm/En.htm?EnName=BP.Sys.ToolbarExcel&No={0}','eudlgframe','Excel配置顶',800,495,'icon-config')\" class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-config'\">Excel配置项</a>",
                    this.FK_MapData));
            this.Pub1.AddEasyUiPanelInfoEnd();
        }

        void btn_SaveExcelFrm_Click(object sender, EventArgs e)
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
                string[] extArr = new[] { ".xls", ".xlsx" };
                string ext = Path.GetExtension(fu.FileName).ToLower();
                if (!extArr.Contains(ext))
                {
                    Response.Write("<script>alert('Excel表单模板只能上传*.xls/*.xlsx两种格式的文件！');history.back();</script>");
                    return;
                }

                string moduleFile = getModuleFile(extArr);

                if (!string.IsNullOrEmpty(moduleFile))
                    File.Delete(Server.MapPath(moduleFile));

                moduleFile = SystemConfig.PathOfDataUser + "FrmOfficeTemplate\\" + this.FK_MapData +
                             Path.GetExtension(fu.FileName);

                fu.SaveAs(moduleFile);
                moduleFile = moduleFile.Substring(moduleFile.IndexOf("\\DataUser\\")).Replace("\\", "/");
                Literal lit = this.Pub1.FindControl("litInfo") as Literal;

                if (lit != null)
                {
                    lit.Text = "[<span style='color:green'>已上传Excel表单模板:<a href='" + moduleFile +
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