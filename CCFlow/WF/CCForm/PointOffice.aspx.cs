using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using System.Data;
using BP.DA;


namespace CCFlow.WF.CCForm
{
    public partial class PointOffice : System.Web.UI.Page
    {
        #region 属性
        /// <summary>
        /// OID编号
        /// </summary>
        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
            }
        }
        /// <summary>
        /// 附件数据编号主键
        /// </summary>
        public string DelPKVal
        {
            get
            {
                return this.Request.QueryString["DelPKVal"];
            }
        }
        /// <summary>
        /// 设计表单主键
        /// </summary>
        public string FK_FrmAttachment
        {
            get
            {
                return this.Request.QueryString["FK_FrmAttachment"];
            }
        }

        public string DoType
        {
            get { return this.Request.QueryString["DoType"]; }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        /// <summary>
        /// 表单编号
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {

            string action = Request["action"];

            string name = null;
            if (BP.Web.WebUser.No == "Guest")
            {
                name = BP.Web.GuestUser.Name;
            }
            else
            {
                name = BP.Web.WebUser.Name;
            }
            TB_User.Value = name;


            FrmAttachmentDB downDB = new FrmAttachmentDB();

            if (DoType.Equals("EditOffice"))
            {
                if (!DataType.IsNullOrEmpty(DelPKVal))
                {

                    downDB.MyPK = this.DelPKVal;
                    downDB.Retrieve();
                    TB_FilePath.Value = downDB.FileFullName;
                    TB_FileType.Value = downDB.FileExts;
                }
                if (!DataType.IsNullOrEmpty(this.FK_FrmAttachment))
                {
                    FrmAttachment attachment = new FrmAttachment();
                    int result = 0;
                    //表单编号与节点不为空
                    if (this.FK_MapData != null && this.FK_Node != null)
                    {
                        BP.En.QueryObject objInfo = new BP.En.QueryObject(attachment);
                        objInfo.AddWhere(FrmAttachmentAttr.FK_MapData, this.FK_MapData);
                        objInfo.addAnd();
                        objInfo.AddWhere(FrmAttachmentAttr.FK_Node, this.FK_Node);
                        objInfo.addAnd();
                        objInfo.AddWhere(FrmAttachmentAttr.NoOfObj, this.NoOfObj);
                        result = objInfo.DoQuery();
                        //result = attachment.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData,
                        //                            FrmAttachmentAttr.FK_Node, this.FK_Node, FrmAttachmentAttr.NoOfObj, this.DelPKVal);
                    }
                    if (result == 0) /*如果没有定义，就获取默认的.*/
                    {
                        attachment.MyPK = this.FK_FrmAttachment;
                        attachment.Retrieve();
                    }

                    if (!attachment.IsWoEnableSave)
                    {
                        Btn_Save.Visible = false;
                    }
                    if (attachment.IsWoEnableReadonly)
                    {
                        TB_IsReadOnly.Value = "1";
                    }
                    else
                    {
                        TB_IsReadOnly.Value = "0";
                    }

                    if (!attachment.IsWoEnableRevise)
                    {
                        Btn_AttachDoc.Visible = false;
                        Btn_UnAttachDoc.Visible = false;
                    }

                    if (!attachment.IsWoEnablePrint)
                    {
                        TB_IsPrint.Value = "0";
                    }
                    else
                    {
                        TB_IsPrint.Value = "1";

                    }

                    if (!attachment.IsWoEnableViewKeepMark)
                    {
                        sShowName.Visible = false;
                    }

                   
                }
            }

            if (!DataType.IsNullOrEmpty(action))
            {
                if (action.Equals("SaveFile"))
                {
                    SaveFile();
                }
                else if (action.Equals("LoadFile"))
                {
                    LoadFile();
                }
                else if (action.Equals("LoadFlow"))
                {
                    GetFlow();
                }
            }
        }

        private void GetFlow()
        {

            string sql = "select No,Name from WF_Flow ";

            DataTable table = BP.DA.DBAccess.RunSQLReturnTable(sql);



            System.Collections.ArrayList dic = new System.Collections.ArrayList();

            foreach (DataRow row in table.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn dc in table.Columns)
                {
                    drow.Add(dc.ColumnName, row[dc.ColumnName]);
                }
                dic.Add(drow);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            string result = BP.Tools.Json.ToJson(dic); // jss.Serialize(dic);

            Response.Clear();
            Response.Write(result);
            Response.End();


        }

        private void LoadFile()
        {
            string path = TB_FilePath.Value;
            path = Server.MapPath("~/" + path);
            var result = File.ReadAllBytes(path);

            Response.Clear();

            Response.BinaryWrite(result);
            Response.End();



        }

        private void SaveFile()
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;

                //string file = Request["Path"];
                //file = HttpUtility.UrlDecode(file, Encoding.UTF8);

                if (files.Count > 0)
                {
                    ///'检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    string fileName, fileExtension;
                    fileName = System.IO.Path.GetFileName(postedFile.FileName);
                    if (fileName != "")
                    {
                        fileExtension = System.IO.Path.GetExtension(fileName);

                        postedFile.SaveAs(TB_FilePath.Value);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}