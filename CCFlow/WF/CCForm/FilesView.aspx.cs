using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using BP;
using BP.Sys;
using BP.Web;
using BP.DA;
using System.Data;
using BP.WF.Template;

namespace CCFlow.WF.CCForm
{
    public partial class FilesView : PageBase
    {
        #region  属性
        public string DelPKVal
        {
            get
            {
                return this.Request.QueryString["DelPKVal"];
            }
        }
        public string DoType
        {
            get { return this.Request.QueryString["DoType"]; }
        }

        public string FK_FrmAttachment
        {

            get { return this.Request.QueryString["FK_FrmAttachment"]; }
        }

        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
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
                if (_fk_node == 0 && !string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
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
                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["OID"];

                if (string.IsNullOrEmpty(str))
                    str = this.Request.QueryString["PKVal"];

                return Int64.Parse(str);
            }
        }

        public string FK_FrmAttachmentExt
        {
            get
            {
                return "ND" + this.FK_Node + "_DocMultiAth"; // this.Request.QueryString["FK_FrmAttachment"];
            }
        }

        public string FK_MapData
        {
            get
            {
                string fk_mapdata = this.Request.QueryString["FK_MapData"];
                if (string.IsNullOrEmpty(fk_mapdata))
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
                if (string.IsNullOrEmpty(paras) == false)
                    if (paras.Contains("IsCC=1") == true)
                        return "1";
                return "ssss";
            }
        }
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            FrmAttachmentDB downDB = new FrmAttachmentDB();
            if ( this.DoType.Equals("view") || DoType.Equals("ViewPic"))
            {
                if (!string.IsNullOrEmpty(DelPKVal))
                {
                    downDB.MyPK = this.DelPKVal;
                    downDB.Retrieve();
                    string filePath = "";
                    if (DoType.Equals("view"))
                    {
                        if (DataType.IsImgExt(downDB.FileExts))
                        {
                            RenderPic(null);
                            return;
                        }
                    }

                    try
                    {
                        filePath = Server.MapPath("~/" + downDB.FileFullName);
                    }
                    catch
                    {
                        filePath = downDB.FileFullName;
                    }

                    if (downDB.FileExts.ToUpper().Equals("CEB"))
                    {
                        //判断是绝对路径还是相对路径
                        string fileSave = Server.MapPath("~/DataUser/UploadFile/" + downDB.MyPK + "." + downDB.FileName);

                        if (!System.IO.File.Exists(fileSave))
                        {

                            byte[] fileBytes = File.ReadAllBytes(filePath);

                            File.WriteAllBytes(fileSave, fileBytes);

                        }
                        this.Response.Redirect("/DataUser/UploadFile/" + downDB.MyPK + "." +
                                                      downDB.FileName, true);
                    }

                    if (File.Exists(filePath))
                    {
                        byte[] result;
                        try
                        {
                            result = File.ReadAllBytes(filePath);
                        }
                        catch
                        {
                            result = File.ReadAllBytes(downDB.FileFullName);
                        }

                        Response.Clear();
                        if (downDB.FileExts == "pdf")
                            Response.ContentType = "Application/pdf";

                        Response.BinaryWrite(result);
                        Response.End();


                    }
                    else
                    {
                        this.Alert("没有找到文件。");
                        this.WinClose();
                    }
                }
            }
            else
            {
                RenderPic(DoType);

            }
        }

        /// <summary>
        /// 添加图片
        /// </summary>
        private void RenderPic(string type)
        {

            #region 处理权限控制.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment();
            athDesc.MyPK = this.FK_FrmAttachment;
            if (this.FK_Node == 0 || this.FK_Flow == null)
            {
                athDesc.RetrieveFromDBSources();
            }
            else
            {
                #region 判断是否可以查询出来，如果查询不出来，就可能是公文流程。
                athDesc.MyPK = this.FK_FrmAttachment;
                if (athDesc.RetrieveFromDBSources() == 0 && string.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    /*如果没有查询到它,就有可能是公文多附件被删除了.*/
                    athDesc.MyPK = this.FK_FrmAttachment;
                    athDesc.NoOfObj = "DocMultiAth";
                    athDesc.FK_MapData = this.FK_MapData;
                    athDesc.Exts = "*.*";

                    //存储路径.
                    string path = Server.MapPath("/DataUser/UploadFile/");
                    path += "\\F" + this.FK_Flow + "MultiAth";
                    athDesc.SaveTo = path;
                    athDesc.IsNote = false; //不显示note字段.


                    //位置.
                    athDesc.X = (float)94.09;
                    athDesc.Y = (float)333.18;
                    athDesc.W = (float)626.36;
                    athDesc.H = (float)150;

                    //多附件.
                    athDesc.UploadType = AttachmentUploadType.Multi;
                    athDesc.Name = "公文多附件(系统自动增加)";
                    athDesc.SetValByKey("AtPara",
                        "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                    athDesc.Insert();

                    //有可能在其其它的节点上没有这个附件，所以也要循环增加上它.
                    BP.WF.Nodes nds = new BP.WF.Nodes(this.FK_Flow);
                    foreach (BP.WF.Node nd in nds)
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
                #endregion 判断是否可以查询出来，如果查询不出来，就可能是公文流程。

                #region 处理权限方案。
                /*首先判断是否具有权限方案*/
                string at = BP.Sys.SystemConfig.AppCenterDBVarStr;
                Paras ps = new BP.DA.Paras();
                ps.SQL = "SELECT FrmSln FROM WF_FrmNode WHERE FK_Node=" + at + "FK_Node AND FK_Flow=" + at + "FK_Flow AND FK_Frm=" + at + "FK_Frm";
                ps.Add("FK_Node", this.FK_Node);
                ps.Add("FK_Flow", this.FK_Flow);
                ps.Add("FK_Frm", this.FK_MapData);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    athDesc.RetrieveFromDBSources();
                }
                else
                {
                    int sln = int.Parse(dt.Rows[0][0].ToString());
                    if (sln == 0)
                    {
                        athDesc.RetrieveFromDBSources();
                    }
                    else
                    {
                        int result = athDesc.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData,
                             FrmAttachmentAttr.FK_Node, this.FK_Node, FrmAttachmentAttr.NoOfObj, this.Ath);

                        if (result == 0) /*如果没有定义，就获取默认的.*/
                            athDesc.RetrieveFromDBSources();
                        //  throw new Exception("@该独立表单在该节点("+this.FK_Node+")使用的是自定义的权限控制，但是没有定义该附件的权限。");
                    }
                }
                #endregion 处理权限方案。
            }




            #endregion 处理权限控制.

            BP.Sys.FrmAttachmentDBs dbs = LoadAttach(athDesc);
            this.Pub1.AddTable("style='width:100%;padding:0px;margin:0px;display:block;text-align:center'");
            int idx = 0;
            bool isHave = false;
            bool isRedirect = false;
            int count = 0;
            foreach (FrmAttachmentDB db in dbs)
            {

                if (!DataType.IsImgExt(db.FileExts))
                    continue;

                if (!string.IsNullOrEmpty(type))
                {
                    int updateIdx = int.Parse(this.Request.QueryString["Idx"]);
                    if (type.Equals("UP"))
                    {
                        if (db.Idx == updateIdx)
                        {
                            db.Idx = updateIdx - 1;
                            db.Update();
                        }
                        else if (db.Idx == updateIdx - 1)
                        {
                            db.Idx = updateIdx;
                            db.Update();
                        }
                    }
                    else if (type.Equals("DOWN"))
                    {
                        if (db.Idx == updateIdx)
                        {
                            db.Idx = updateIdx + 1;
                            db.Update();
                        }
                        else if (db.Idx == updateIdx + 1)
                        {
                            db.Idx = updateIdx;
                            db.Update();
                        }
                    }

                }


                if (db.Idx !=0)
                    isHave = true;

                count++;
            }

            if (!string.IsNullOrEmpty(type))
                dbs = LoadAttach(athDesc);

            foreach (FrmAttachmentDB db in dbs)
            {

                if (!DataType.IsImgExt(db.FileExts))
                    continue;

                this.Pub1.AddTR();
                if (!athDesc.SaveTo.EndsWith("/"))
                    athDesc.SaveTo += "/";

              //s  image.ImageUrl = athDesc.SaveTo + this.WorkID + "/" + db.MyPK + "." + db.FileName;
               // this.Pub1.AddTD("style='width:80%;margin-left:5%;display:block;text-align:center;overflow:scroll' align='center' valign='middle'" ,image);
                this.Pub1.AddTDBegin();

                this.Pub1.Add("<div style='width:80%;margin:0 auto;'>");
               // image.Attributes["style"]="width:800px;heght:600px;";

                string url = athDesc.SaveTo + this.WorkID + "/" + db.UploadGUID + "." + db.FileName;
              //  url = "";
                this.Pub1.Add("<img src='" + url + "' onload=\"AutoResizeImage(800,600,this)\" border=0  />");
                this.Pub1.Add("</div>");
                this.Pub1.AddTDEnd();

                if (athDesc.IsOrder && count >1)
                { 
                    this.Pub1.AddTR();
                    idx++;

                    if (db.Idx == 0)
                    {
                        if (isHave)
                        {
                            db.Idx = count - idx + 1;
                            db.Update();
                        }
                        else
                        {
                            db.Idx = idx;
                            db.Update();
                        }
                        isRedirect = true;
                    }

                    this.Pub1.AddTDBegin();

                    this.Pub1.Add("<div style='width:80%;margin:0 auto;text-align:center;'>");
                   
               
                    if (idx == count)
                        this.Pub1.Add(db.FileName + "&nbsp;&nbsp;<a href='FilesView.aspx?DoType=UP&Idx=" + idx + "&DelPKVal=" + db.MyPK + "&FK_FrmAttachment=" + this.FK_FrmAttachment + "&PKVal=" + this.PKVal + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FK_FrmAttachmentExt=" + this.FK_FrmAttachmentExt + "&IsCC=" + this.IsCC + "&Ath=" + this.Ath + "'>上移</a>&nbsp;&nbsp;<a href='FilesView.aspx?DoType=ViewPic&DelPKVal=" + db.MyPK + "'  target='_balnk'>查看原图</a>");
                    else if (idx == 1)
                        this.Pub1.Add(db.FileName + "&nbsp;&nbsp;<a href='FilesView.aspx?DoType=DOWN&Idx=" + idx + "&DelPKVal=" + db.MyPK + "&FK_FrmAttachment=" + this.FK_FrmAttachment + "&PKVal=" + this.PKVal + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FK_FrmAttachmentExt=" + this.FK_FrmAttachmentExt + "&IsCC=" + this.IsCC + "&Ath=" + this.Ath + "'>下移</a>&nbsp;&nbsp;<a href='FilesView.aspx?DoType=ViewPic&DelPKVal=" + db.MyPK + "'  target='_balnk'>查看原图</a>");
                    else
                        this.Pub1.Add(db.FileName + "&nbsp;&nbsp;<a href='FilesView.aspx?DoType=UP&Idx=" + idx + "&DelPKVal=" + db.MyPK + "&FK_FrmAttachment=" + this.FK_FrmAttachment + "&PKVal=" + this.PKVal + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FK_FrmAttachmentExt=" + this.FK_FrmAttachmentExt + "&IsCC=" + this.IsCC + "&Ath=" + this.Ath + "'>上移</a>&nbsp;&nbsp;<a href='FilesView.aspx?DoType=DOWN&Idx=" + idx + "&DelPKVal=" + db.MyPK + "&FK_FrmAttachment=" + this.FK_FrmAttachment + "&PKVal=" + this.PKVal + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FK_FrmAttachmentExt=" + this.FK_FrmAttachmentExt + "&IsCC=" + this.IsCC + "&Ath=" + this.Ath + "'>下移</a>&nbsp;&nbsp;<a href='FilesView.aspx?DoType=ViewPic&DelPKVal=" + db.MyPK + "'  target='_balnk'>查看原图</a>");
                    this.Pub1.Add("</div>");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
                else
                { 
                    this.Pub1.AddTR();

                    this.Pub1.Add("<div style='width:80%;margin:0 auto;text-align:center'>");
                    this.Pub1.AddTDBegin();
                    this.Pub1.Add(db.FileName);
                    this.Pub1.Add("</div>");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }

                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();


            if (isRedirect)
                this.Response.Redirect(this.Request.RawUrl,true);
        }


        private FrmAttachmentDBs LoadAttach(BP.Sys.FrmAttachment athDesc)
        {
            BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();

            if (athDesc.HisCtrlWay == AthCtrlWay.MySelfOnly)
            {
                 dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment,
                    FrmAttachmentDBAttr.RefPKVal, this.PKVal, FrmAttachmentDBAttr.Rec, BP.Web.WebUser.No);
                 return dbs;
            }

            if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
            {
                string pWorkID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + this.PKVal, 0).ToString();
                if (pWorkID == null || pWorkID == "0")
                    pWorkID = this.PKVal;

                if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                {
                    /* 继承模式 */
                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);
                    qo.addOr();
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, int.Parse(this.PKVal));
                    qo.addOrderBy("Idx");
                    qo.DoQuery();
                }

                if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                {
                    /*共享模式*/
                    //dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, pWorkID);

                    BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
                    qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pWorkID);

                    qo.addOrderBy("Idx");
                    qo.DoQuery();
                }
            }
            else
            {

                int num = dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachment,
                      FrmAttachmentDBAttr.RefPKVal, this.PKVal, "Idx");
                if (num == 0 && this.IsCC == "1")
                {
                    CCList cc = new CCList();
                    int nnn = cc.Retrieve(CCListAttr.FK_Node, this.FK_Node, CCListAttr.WorkID, this.WorkID, CCListAttr.CCTo, WebUser.No);
                    if (cc.NDFrom != 0)
                    {
                        this._fk_node = cc.NDFrom;

                        dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, this.FK_FrmAttachmentExt,
                            FrmAttachmentDBAttr.FK_MapData, "ND" + cc.NDFrom, FrmAttachmentDBAttr.RefPKVal, this.WorkID.ToString());

                        //重新设置文件描述。
                        athDesc.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData, FrmAttachmentAttr.NoOfObj, "DocMultiAth");
                    }
                }
            }

            return dbs;
        }
    }
}