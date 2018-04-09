using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;
using BP.Web;
using BP.WF;
using BP.WF.Data;
using BP.DA;


namespace CCFlow.WF.WebOffice
{
    public partial class AttachOffice : System.Web.UI.Page
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
        public int FK_Node
        {
            get
            {

                string str= this.Request.QueryString["FK_Node"];
                if (DataType.IsNullOrEmpty(str))
                {
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    gwf.WorkID = Int64.Parse(this.PKVal);
                    if (gwf.RetrieveFromDBSources() == 0)
                        return 0;
                    return gwf.FK_Node;
                }
                return int.Parse(str);
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
        /// <summary>
        ///用户名 
        /// </summary>
        public string UserName
        {
            get
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
        }

        private bool isReadOnly = false;
        public bool ReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = true; }
        }

        private string fileSavePath;

        public string FileSavePath
        {
            get { return fileSavePath; }
            set { fileSavePath = value; }
        }

        private string _realFileName;

        public string RealFileName
        {
            get { return _realFileName; }
            set { _realFileName = value; }
        }

        private string _fileFullName;

        public string FileFullName
        {
            get { return _fileFullName; }
            set { _fileFullName = value; }
        }
        /// <summary>
        /// 节点信息
        /// </summary>
        public string NodeInfo
        {
            get
            {
                if (FK_Node == 999999 || this.FK_Node == 0)
                    return WebUser.Name + "    时间:" + DateTime.Now.ToString("YYYY-MM-dd HH:mm:ss");

                BP.WF.Node nodeInfo = new BP.WF.Node(FK_Node);
                return nodeInfo.Name + ": " + WebUser.Name + "    时间:" + DateTime.Now.ToString("YYYY-MM-dd HH:mm:ss");
            }
        }
        private bool _isCheck = false;
        public bool IsCheck
        {
            get { return _isCheck; }
            set { _isCheck = value; }
        }

        private bool _isSavePDF = false;

        public bool IsSavePDF
        {
            get { return _isSavePDF; }
            set { _isSavePDF = value; }
        }

        private bool _isMarks = false;

        public bool IsMarks
        {
            get { return _isMarks; }
            set { _isMarks = value; }
        }


        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (DataType.IsNullOrEmpty(FK_MapData) || DataType.IsNullOrEmpty(FK_FrmAttachment))
            {
                divMenu.InnerHtml = "<h1 style='color:red'>传入参数错误!<h1>";
                return;
            }

            if (IsPostBack==false)
            {
                string type = Request["action"];
                if (DataType.IsNullOrEmpty(type))
                {
                    InitOffice(true);
                }
                else
                {
                    InitOffice(false);
                    if (type.Equals("LoadFile"))
                        LoadFile();
                    else if (type.Equals("SaveFile"))
                        SaveFile();
                    else
                        throw new Exception("传入的参数不正确!");
                }
            }
        }

        private void InitOffice(bool isMenu)
        {
            bool isCompleate = false;
            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = this.FK_Node; 
            node.RetrieveFromDBSources();
            try
            {
                WorkFlow workFlow = new WorkFlow(node.FK_Flow, Int64.Parse(PKVal));
                isCompleate = workFlow.IsComplete;
            }
            catch (Exception)
            {
                try
                {
                    Flow fl = new Flow(node.FK_Flow);
                    GERpt rpt = fl.HisGERpt;
                    rpt.OID = Int64.Parse(PKVal);
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

            if (isCompleate==false)
            {
                try
                {
                    isCompleate = !BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(node.FK_Flow, node.NodeID, Int64.Parse(PKVal), WebUser.No);
                    //WorkFlow workFlow = new WorkFlow(node.FK_Flow, Int64.Parse(PKVal));
                    //isCompleate = !workFlow.IsCanDoCurrentWork(WebUser.No);
                }
                catch
                {
                }
            }

            FrmAttachment attachment = new FrmAttachment();
            int result = 0;
            //表单编号与节点不为空
            if (this.FK_MapData != null && this.FK_Node != 0)
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
            if (!isCompleate)
            {
                if (attachment.IsWoEnableReadonly)
                {
                    ReadOnly = true;
                }
                if (attachment.IsWoEnableCheck)
                {
                    IsCheck = true;
                }
                IsMarks = attachment.IsWoEnableMarks;


            }
            else
            {
                if (attachment.IsWoEnableReadonly)
                {
                    ReadOnly = true;
                }
            }
            if (isMenu && !isCompleate)
            {
                #region 初始化按钮
                if (attachment.IsWoEnableViewKeepMark)
                {
                    divMenu.InnerHtml = "<select id='marks' onchange='ShowUserName()'  style='width: 100px'><option value='1'>显示留痕</option><option value='2'>隐藏留痕</option><select>";
                }

                if (attachment.IsWoEnableTemplete)
                {
                    AddBtn("openTempLate", "打开模板", "OpenTempLate");
                }
                if (attachment.IsWoEnableSave)
                {
                    AddBtn("saveFile", "保存", "saveOffice");
                }
                if (attachment.IsWoEnableRevise)
                {
                    AddBtn("accept", "接受修订", "acceptOffice");
                    AddBtn("refuse", "拒绝修订", "refuseOffice");
                }

                if (attachment.IsWoEnableOver)
                {
                    AddBtn("over", "套红文件", "overOffice");
                }


                if (attachment.IsWoEnableSeal)
                {
                    AddBtn("seal", "签章", "sealOffice");
                }
                if (attachment.IsWoEnableInsertFlow)
                {
                    AddBtn("flow", "插入流程图", "InsertFlow");
                }
                if (attachment.IsWoEnableInsertFlow)
                {
                    AddBtn("fegnxian", "插入风险点", "InsertFengXian");
                }
               
                #endregion
            }

            #region   初始化文件

            FrmAttachmentDB downDB = new FrmAttachmentDB();

            downDB.MyPK = this.DelPKVal;
            downDB.Retrieve();
            fileName.Text = downDB.FileName;
            fileType.Text = downDB.FileExts;
            RealFileName = downDB.FileName;
            FileFullName = downDB.FileFullName;
            FileSavePath = attachment.SaveTo;

            #endregion
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
                    try
                    {
                        path = Server.MapPath("~/" + FileFullName);

                    }
                    catch
                    {
                        path = FileFullName;
                    }
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

        private void SaveFile()
        {
            string message = "true";
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
                    
                    string path = "";
                    try
                    {
                        path = Server.MapPath("~/" + FileFullName);
                    }
                    catch
                    {
                        path = FileFullName;
                    }

                    if (fileName != "")
                    {
                        fileExtension = System.IO.Path.GetExtension(fileName);
                        postedFile.SaveAs(path);
                    }
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

        private void AddBtn(string id, string label, string clickEvent)
        {
            Btn btn = new Btn();
            btn.ID = id;
            btn.Text = label;
            btn.Attributes["onclick"] = "return " + clickEvent + "()";
            btn.Attributes["class"] = "btn";
            divMenu.Controls.Add(btn);
        }





    }
}