using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.WF.Template;
using BP.WF;

namespace CCFlow.WF.CCForm
{
    public partial class AttachmentUploadImg : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// ath.
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
            }
        }
        public string IsReadonly
        {
            get
            {
                return this.Request.QueryString["IsReadonly"];
            }
        }
        public string DelPKVal
        {
            get
            {
                return this.Request.QueryString["DelPKVal"];
            }
        }
        public string FK_FrmAttachment
        {
            get
            {
                return this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_FrmAttachmentExt
        {
            get
            {
                return "ND" + this.FK_Node + "_DocMultiAth"; // this.Request.QueryString["FK_FrmAttachment"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int _fk_node = 0;
        public int FK_Node
        {
            get
            {
                if (_fk_node == 0 && !DataType.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return int.Parse(this.Request.QueryString["FK_Node"]);

                return _fk_node;
            }
            set
            {
                _fk_node = value;
            }
        }
        public Int64 WorkID
        {
            get
            {
                string str = this.Request.QueryString["WorkID"];
                if (DataType.IsNullOrEmpty(str))
                    str = this.Request.QueryString["OID"];

                if (DataType.IsNullOrEmpty(str))
                    str = this.Request.QueryString["PKVal"];

                return Int64.Parse(str);
            }
        }
        public string FK_MapData
        {
            get
            {
                string fk_mapdata = this.Request.QueryString["FK_MapData"];
                if (DataType.IsNullOrEmpty(fk_mapdata))
                    fk_mapdata = "ND" + FK_Node;
                return fk_mapdata;
            }
        }
        public string Ath
        {
            get
            {
                return this.Request.QueryString["Ath"];
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
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 保存文件，在这里做修改就可以了.
        /// </summary>
        public void SaveFile()
        {
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(this.FK_FrmAttachment);
            System.Web.UI.WebControls.FileUpload fu = null; 
            //this.Pub1.FindControl("file")as System.Web.UI.WebControls.FileUpload;

            if (fu.HasFile == false || fu.FileName.Length <= 2)
            {
                this.Alert("请选择上传的文件.");
                return;
            }
            string exts = System.IO.Path.GetExtension(fu.FileName).ToLower().Replace(".", "");

            //如果有上传类型限制，进行判断格式
            if (athDesc.Exts == "*.*" || athDesc.Exts == "")
            {
                /*任何格式都可以上传*/
            }
            else
            {
                if (athDesc.Exts.ToLower().Contains(exts) == false)
                {
                    this.Alert("您上传的文件，不符合系统的格式要求，要求的文件格式:" + athDesc.Exts + "，您现在上传的文件格式为:" + exts);
                    return;
                }
            }

            string savePath = athDesc.SaveTo;

            if (savePath.Contains("@") == true || savePath.Contains("*") == true)
            {
                /*如果有变量*/
                savePath = savePath.Replace("*", "@");
                GEEntity en = new GEEntity(athDesc.FK_MapData);
                en.PKVal = this.PKVal;
                en.Retrieve();
                savePath = BP.WF.Glo.DealExp(savePath, en, null);

                if (savePath.Contains("@") && this.FK_Node != 0)
                {
                    /*如果包含 @ */
                    BP.WF.Flow flow = new BP.WF.Flow(this.FK_Flow);
                    BP.WF.Data.GERpt myen = flow.HisGERpt;
                    myen.OID = this.WorkID;
                    myen.RetrieveFromDBSources();
                    savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                }
                if (savePath.Contains("@") == true)
                    throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
            }
            else
            {
                //savePath = athDesc.SaveTo + "\\" + this.PKVal;
            }

            //替换关键的字串.
            savePath = savePath.Replace("\\\\", "\\");
            savePath = Server.MapPath("~/" + savePath);
            try
            {
                if (System.IO.Directory.Exists(savePath) == false)
                    System.IO.Directory.CreateDirectory(savePath);
            }
            catch (Exception ex)
            {
                throw new Exception("@创建路径出现错误，可能是没有权限或者路径配置有问题:" + Server.MapPath("~/" + savePath) + "===" + savePath + "@技术问题:" + ex.Message);
            }

            string guid = BP.DA.DBAccess.GenerGUID();
            string fileName = fu.FileName.Substring(0, fu.FileName.LastIndexOf('.'));
            string ext = System.IO.Path.GetExtension(fu.FileName);
            //string realSaveTo = Server.MapPath("~/" + savePath) + "/" + guid + "." + fileName + "." + ext;
            //string realSaveTo = Server.MapPath("~/" + savePath) + "\\" + guid + "." + fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
            //string saveTo = savePath + "/" + guid + "." + fileName + "." + ext;
            string realSaveTo = savePath + "/" + guid + "." + fileName + ext;
            string saveTo = realSaveTo;
            try
            {
                fu.SaveAs(realSaveTo);
            }
            catch (Exception ex)
            {
                this.Response.Write("@文件存储失败,有可能是路径的表达式出问题,导致是非法的路径名称:" + ex.Message);
                return;
            }

            FileInfo info = new FileInfo(realSaveTo);
            FrmAttachmentDB dbUpload = new FrmAttachmentDB();
            dbUpload.MyPK = guid; // athDesc.FK_MapData + oid.ToString();
            dbUpload.NodeID = FK_Node.ToString();
            dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;
            if (athDesc.AthUploadWay == AthUploadWay.Inherit)
            {
                /*如果是继承，就让他保持本地的PK. */
                dbUpload.RefPKVal = this.PKVal.ToString();
            }

            if (athDesc.AthUploadWay == AthUploadWay.Interwork)
            {
                /*如果是协同，就让他是PWorkID. */
                string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
                if (pWorkID == null || pWorkID == "0")
                    pWorkID = this.PKVal;
                dbUpload.RefPKVal = pWorkID;
            }

            dbUpload.FK_MapData = athDesc.FK_MapData;
            dbUpload.FK_FrmAttachment = this.FK_FrmAttachment;

            dbUpload.FileFullName = saveTo;
            dbUpload.FileName = fu.FileName;
            dbUpload.FileSize = (float)info.Length;

            dbUpload.RDT = DataType.CurrentDataTimess;
            dbUpload.Rec = BP.Web.WebUser.No;
            dbUpload.RecName = BP.Web.WebUser.Name;

            //if (athDesc.IsNote)
            //    dbUpload.MyNote = this.Pub1.GetTextBoxByID("TB_Note").Text;

            //if (athDesc.Sort.Contains(","))
            //    dbUpload.Sort = this.Pub1.GetDDLByID("ddl").SelectedItemStringVal;

            dbUpload.UploadGUID = guid;
            dbUpload.Insert();
        }
    }
}