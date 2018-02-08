using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CCFlow.Web.Comm.UC;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP;

public partial class CCFlow_Comm_UC_UIEn : BP.Web.UC.UCBase3
{
    #region 属性
    /// <summary>
    /// 类名成
    /// </summary>
    public new string EnsName
    {
        get
        {
            string ensName = this.Request.QueryString["EnsName"];
            if (ensName == null || ensName == "")
                ensName = this.ViewState["EnsName"] as string;
            else
                this.ViewState["EnsName"] = ensName;

            if (ensName == null)
            {
                string s = this.Request.QueryString["EnName"];
                Entity en = BP.En.ClassFactory.GetEn(s);
                this.ViewState["EnsName"] = en.GetNewEntities.ToString();
                this.ViewState["EnName"] = en.ToString();
            }

            ensName = this.ViewState["EnsName"] as string;
            return ensName;
        }
    }
    /// <summary>
    /// 获取当前页面是否存在于easyui-dialog的层中的标识inlayer，在层中时inlayer="1"
    /// </summary>
    public string InLayer
    {
        get
        {
            return Request.QueryString["inlayer"];
        }
    }
    /// <summary>
    /// 类名成
    /// </summary>
    public new string EnName
    {
        get
        {
            string enName = this.Request.QueryString["EnName"];
            if (enName == null || enName == "")
                enName = this.ViewState["EnName"] as string;
            else
                this.ViewState["EnName"] = enName;

            if (enName == null)
            {
                string s = this.Request.QueryString["EnsName"];
                if (s == null)
                    s = this.ViewState["EnsName"] as string;

                Entities ens = BP.En.ClassFactory.GetEns(s);
                this.ViewState["EnsName"] = ens.ToString();
                this.ViewState["EnName"] = ens.GetNewEntity.ToString();
            }
            enName = this.ViewState["EnName"] as string;
            return enName;
        }
    }
    /// <summary>
    /// 得到一个新的事例数据．
    /// </summary>
    public Entity GetEnDa
    {
        get
        {
            Entity en = this.GetEns.GetNewEntity;
            Attrs myattrs1 = en.EnMap.Attrs;
            foreach (Attr attr in myattrs1)
            {
                if (this.Request.QueryString[attr.Key] == null)
                    continue;
                en.SetValByKey(attr.Key, this.Request.QueryString[attr.Key]);
            }
            if (en.PKCount == 1)
            {
                if (this.Request.QueryString["PK"] != null)
                {
                    en.PKVal = this.Request.QueryString["PK"];
                }
                else
                {
                    if (this.Request.QueryString[en.PK] == null)
                        return en;
                    else
                        en.PKVal = this.Request.QueryString[en.PK];
                }

                if (en.IsExits == false)
                {
                    BP.Sys.PubClass.Alert("@记录{"+en.ToString()+"}{"+en.PKVal+"}不存在,或者没有保存,请执行保存后在打开编辑.");
                    return en;
                    //throw new Exception("@记录{"+en.ToString()+"}{"+en.PKVal+"}不存在,或者没有保存.");
                }
                else
                {
                    int i = en.RetrieveFromDBSources();
                    if (i == 0)
                        en.RetrieveFromDBSources();
                }


                Attrs myattrs = en.EnMap.Attrs;
                foreach (Attr attr in myattrs)
                {
                    if (this.Request.QueryString[attr.Key] == null)
                        continue;
                    en.SetValByKey(attr.Key, this.Request.QueryString[attr.Key]);
                }
                return en;
            }
            else if (en.IsMIDEntity)
            {
                string val = this.Request.QueryString["MID"];
                if (val == null)
                    val = this.Request.QueryString["PK"];
                if (val == null)
                {
                    return en;
                }
                else
                {
                    en.SetValByKey("MID", val);
                    en.RetrieveFromDBSources();
                    return en;
                }
            }

            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.IsPK)
                {
                    string str = this.Request.QueryString[attr.Key];
                    if (str == null)
                    {
                        if (en.IsMIDEntity)
                        {
                            en.SetValByKey("MID", this.Request.QueryString["PK"]);
                            continue;
                        }
                        else
                        {
                            throw new Exception("@没有把主键值[" + attr.Key + "]传输过来.");
                        }
                    }
                }
                if (this.Request.QueryString[attr.Key] != null)
                    en.SetValByKey(attr.Key, this.Request.QueryString[attr.Key]);
            }

            if (en.IsExits == false)
            {
                throw new Exception("@数据没有记录.");
            }
            else
            {
                en.RetrieveFromDBSources();
                Attrs myattrs = en.EnMap.Attrs;
                foreach (Attr attr in myattrs)
                {
                    if (this.Request.QueryString[attr.Key] == null)
                        continue;
                    en.SetValByKey(attr.Key, this.Request.QueryString[attr.Key]);
                }
            }
            return en;
        }
    }
    public Entities _GetEns = null;
    public BP.Web.Controls.Btn Btn_New
    {
        get
        {
            return this.ToolBar1.GetBtnByID(NamesOfBtn.New);
        }
    }
    public BP.Web.Controls.Btn Btn_Copy
    {
        get
        {
            return this.ToolBar1.GetBtnByID(NamesOfBtn.Copy);
        }
    }
    public BP.Web.Controls.Btn Btn_Delete
    {
        get
        {
            return this.ToolBar1.GetBtnByID(NamesOfBtn.Delete);
        }
    }
    public BP.Web.Controls.Btn Btn_Adjunct
    {
        get
        {
            return this.ToolBar1.GetBtnByID(NamesOfBtn.Adjunct);
        }
    }
    /// <summary>
    /// 当前的实体集合．
    /// </summary>
    public Entities GetEns
    {
        get
        {
            if (_GetEns == null)
            {
                string enName = this.EnName;
                if (enName != null)
                {
                    if (enName.Contains("."))
                    {
                        Entity en = BP.En.ClassFactory.GetEn(enName);
                        _GetEns = en.GetNewEntities;
                    }
                    else
                    {
                        _GetEns = BP.En.ClassFactory.GetEns(enName);
                    }
                }
                else
                {
                    _GetEns = BP.En.ClassFactory.GetEns(EnsName);
                }
            }
            return _GetEns;
        }
    }
    public Entity _CurrEn = null;
    public Entity CurrEn
    {
        get
        {
            if (_CurrEn == null)
            {
                _CurrEn = this.GetEnDa;
            }
            return _CurrEn;
        }
        set
        {
            _CurrEn = value;
        }
    }
    /// <summary>
    /// 是否Readonly.
    /// </summary>
    public bool IsReadonly
    {
        get
        {
            return (bool)this.ViewState["IsReadonly"];
        }
        set
        {
            ViewState["IsReadonly"] = value;
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        #region 清除缓存;
        this.Response.Expires = -1;
        this.Response.ExpiresAbsolute = DateTime.Now.AddMonths(-1);
        this.Response.CacheControl = "no-cache";
        #endregion 清除缓存

        try
        {
            #region 判断权限
            string pk = this.Request.QueryString["PK"];
            if (pk == null)
                pk = this.Request.QueryString[this.CurrEn.PK];

            Entity en = this.CurrEn;
            if (en == null)
            {
                this.WinClose();
                return;
            }

            UAC uac = this.CurrEn.HisUAC;
            if (uac.IsView == false)
                throw new Exception("@对不起，您没有查看的权限！");

            this.IsReadonly = !uac.IsUpdate;  //是否更有修改的权限．
            if (this.Request.QueryString["IsReadonly"] == "1"
                || this.Request.QueryString["Readonly"] == "1")
                this.IsReadonly = true;
            #endregion

            this.ToolBar1.InitFuncEn(uac, this.CurrEn);

            this.UCEn1.IsReadonly = this.IsReadonly;
            this.UCEn1.IsShowDtl = true;
            this.UCEn1.HisEn = this.CurrEn;

            EnCfg ec = new EnCfg();
            ec.No = this.EnName;
            int i = ec.RetrieveFromDBSources();

            if(i >= 1)
                this.UCEn1.BindV2(this.CurrEn, this.CurrEn.ToString(), this.IsReadonly, true);
            else
                this.UCEn1.Bind(this.CurrEn, this.CurrEn.ToString(), this.IsReadonly, true);

            if (this.CurrEn.EnMap.Attrs.Contains("Name") == true)
                this.Page.Title = this.CurrEn.GetValStringByKey("Name");
            else if (this.CurrEn.EnMap.Attrs.Contains("Title") == true)
                this.Page.Title = this.CurrEn.GetValStringByKey("Title");
            else
                this.Page.Title = "信息卡片";
        }
        catch (Exception ex)
        {
            this.Response.Write(ex.Message);
            Entity en = ClassFactory.GetEns(this.EnsName).GetNewEntity;
            en.CheckPhysicsTable();
            return;
        }

        this.Page.Title = this.CurrEn.EnDesc;

        #region 设置事件
        if (this.Btn_DelFile != null)
            this.Btn_DelFile.Click += new ImageClickEventHandler(Btn_DelFile_Click);

        if (this.ToolBar1.IsExit(NamesOfBtn.New))
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.New).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

        if (this.ToolBar1.IsExit(NamesOfBtn.Save))
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Save).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

        if (this.ToolBar1.IsExit(NamesOfBtn.SaveAndClose))
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.SaveAndClose).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

        if (this.ToolBar1.IsExit(NamesOfBtn.SaveAndNew))
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.SaveAndNew).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

        if (this.ToolBar1.IsExit(NamesOfBtn.Delete))
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Delete).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

        AttrFiles fls = this.CurrEn.EnMap.HisAttrFiles;
        foreach (AttrFile fl in fls)
        {
            if (this.UCEn1.IsExit("Btn_DelFile" + fl.FileNo))
                this.UCEn1.GetImageButtonByID("Btn_DelFile" + fl.FileNo).Click += new ImageClickEventHandler(Btn_DelFile_X_Click);
        }
        #endregion 设置事件

        //此处增加一个判断删除成功的逻辑，办法不太好，暂用，added by liuxc，2015-11-10
        if (IsPostBack==false)
        {
            if (Request["DeleteOver"] == "1")
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "showmsg", "alert('删除成功！');", true);
                this.WinClose(); //add by zhoupeng 2016.11.06 删除之后要关闭.
                return;
            }
        }
    }
    public ImageButton Btn_DelFile
    {
        get
        {
            return this.UCEn1.FindControl("Btn_DelFile") as ImageButton;
        }
    }
    public void DelFile(string id)
    {
    }
    private void Btn_DelFile_X_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton btn = sender as ImageButton;
        string id = btn.ID.Replace("Btn_DelFile", "");
        SysFileManager sf = new SysFileManager();
        string sql = "DELETE FROM " + sf.EnMap.PhysicsTable + " WHERE " + SysFileManagerAttr.EnName + "='" + this.GetEns.GetNewEntity.ToString() + "' AND RefVal='" + this.PKVal + "' AND " + SysFileManagerAttr.AttrFileNo + "='" + id + "'";
        BP.DA.DBAccess.RunSQL(sql);
        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + this.PKVal + "&inlayer=" + this.InLayer, true);
    }
    private void Btn_DelFile_Click(object sender, ImageClickEventArgs e)
    {
        Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
        en.PKVal = this.PKVal;
        en.RetrieveFromDBSources();

        string file = en.GetValStringByKey("MyFilePath") + "//" + en.PKVal + "." + en.GetValStringByKey("MyFileExt");
        try
        {
            System.IO.File.Delete(file);
        }
        catch
        {
        }
        en.SetValByKey("MyFileExt", "");
        en.SetValByKey("MyFileName", "");
        en.SetValByKey("MyFilePath", "");
        en.Update();
        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&EnName=" + this.EnName + "&PK=" + this.PKVal + "&inlayer=" + this.InLayer, true);
    }
    private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
    {

        LinkBtn btn = (LinkBtn)sender;
        try
        {
            switch (btn.ID)
            {
                case NamesOfBtn.Copy:
                    Copy();
                    break;
                case NamesOfBtn.Help:
                    //this.Helper(this.GetEns.GetNewEntity.EnMap.Helper);
                    break;
                case NamesOfBtn.New:
                    //   New();
                    this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&EnName=" + this.EnName + "&inlayer=" + this.InLayer, true);
                    break;
                case NamesOfBtn.SaveAndNew:
                    try
                    {
                        this.Save();
                    }
                    catch (Exception ex)
                    {
                        this.Alert(ex.Message);
                        // this.ResponseWriteBlueMsg(ex.Message);
                        return;
                    }
                    this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&EnName=" + this.EnName + "&inlayer=" + this.InLayer, true);
                    break;
                case NamesOfBtn.SaveAndClose:
                    try
                    {
                        this.Save();
                        this.WinClose();

                        //string script = "function ParentWindowClose() {";
                        //script += "   if(window.parent && window.parent.Win_Close_Dialog){";
                        //script += "      window.parent.Win_Close_Dialog();";
                        //script += "   }else{";
                        //script += "      window.close();";
                        //script += "   }";
                        //script += " }";

                        //if (Request.QueryString["inlayer"] == "1")
                        //    script += "window.parent.closeDlg();";
                        //else
                        //    script += " ParentWindowClose();";
                        
                        //this.Response.Write("<script language='JavaScript'>" + script + "</script>");
                    }
                    catch (Exception ex)
                    {
                        this.Alert(ex.Message);
                        // this.ResponseWriteBlueMsg(ex.Message);
                        return;
                    }
                    break;
                case NamesOfBtn.Save:
                    try
                    {
                        this.Save();
                    }
                    catch (Exception ex)
                    {
                        this.Alert(ex.Message);
                        return;
                    }
                    this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&PK=" + this.PKVal + "&EnName=" + this.EnName + "&tab=" + Uri.EscapeDataString(GetHiddenTabTitle()) + "&inlayer=" + this.InLayer, true);
                    break;
                case NamesOfBtn.Delete:
                    try
                    {
                        Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
                        if (this.PKVal != null)
                            en.PKVal = this.PKVal;
                        en.Delete();
                        //this.Alert("删除成功");
                        //this.WinCloseWithMsg("删除成功!!!");
                        //this.ToMsgPage("删除成功!!!");
                        //edited by liuxc,2015-11-10，因为删除成功后，此记录不存在，回调在Page_Load中，导致引发错误，因此使用此种方法，传递DeleteOver=1参数，在Page_Load中判断，显示删除成功的消息
                        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&DeleteOver=1&t=" +
                                               DateTime.Now.ToString("yyyyMMddHHmmssffffff") + "&inlayer=" + this.InLayer, true);
                        return;
                    }
                    catch (Exception ex)
                    {
                        this.Alert(ex.Message);
                        //this.ToMsgPage("删除期间出现错误: \t\n" + ex.Message);
                        //this.ToMsgPage("删除成功!!!");
                        return;
                    }
                case NamesOfBtn.Close:
                    this.WinClose();
                    break;
                case "Btn_EnList":
                    this.EnList();
                    break;
                case NamesOfBtn.Export:
                    //this.ExportDGToExcel_OpenWin(this.UCEn1,"" );
                    break;
                case NamesOfBtn.Adjunct:
                    //this.InvokeFileManager(this.GetEnDa);
                    break;
                default:
                    throw new Exception("@没有找到" + btn.ID);
            }
        }
        catch (Exception ex)
        {
            this.ResponseWriteRedMsg(ex.Message);
        }
    }

    /// <summary>
    /// 获取隐藏域中保存的当前打开标签值
    /// </summary>
    /// <returns></returns>
    private string GetHiddenTabTitle()
    {
        var hiddenField = UCEn1.FindControl("Hid_CurrentTab") as HiddenField;

        if (hiddenField != null)
            return hiddenField.Value;

        return "";
    }

    public object PKVal
    {
        get
        {
            object obj = ViewState["MyPK"];
            if (obj == null)
                obj = this.Request.QueryString["PK"];

            if (obj == null)
                obj = this.Request.QueryString["OID"];

            if (obj == null)
                obj = this.Request.QueryString["No"];

            if (obj == null)
                obj = this.Request.QueryString["MyPK"];
            if(obj == null)
                obj = this.Request.QueryString[this.CurrEn.PK];
            if (obj == null)
                obj = this.Request.QueryString["NodeID"];

            return obj;
        }
        set
        {
            this.ViewState["MyPK"] = value;
        }
    }

    #region 操作
    /// <summary>
    /// new
    /// </summary>
    public void New()
    {
        this.Response.Redirect("UIEn.aspx?EnsName=" + this.EnsName + "&inlayer=" + this.InLayer, true);
        //return;

        //this.CurrEn = this.GetEns.GetNewEntity;
        //this.PKVal = null;

        //if (this.CurrEn.EnMap.Attrs.Contains("No"))
        //{
        //    Attr attr = this.CurrEn.EnMap.GetAttrByKey("No");

        //    if (attr.UIIsReadonly || this.CurrEn.EnMap.IsAutoGenerNo)
        //    {
        //        if (this.CurrEn.GetValStringByKey("No") == "")
        //        {
        //            this.CurrEn.SetValByKey("No", this.CurrEn.GenerNewNoByKey("No"));
        //            //string val = BP.Sys.SystemConfig.GetConfigXmlEns(ConfigKeyEns.IsInsertBeforeNew, CurrEn.ToString());
        //            //if (val == "1")
        //            //{
        //            //    //CurrEn.SetValByKey("No",dr[attr.Key]);
        //            //    CurrEn.Insert();
        //            //}
        //        }
        //    }
        //}

        //if (this.ToolBar1.IsExit(NamesOfBtn.Adjunct) == true)
        //    this.Btn_Adjunct.Enabled = false;

        //if (this.ToolBar1.IsExit(NamesOfBtn.Delete) == true)
        //    this.Btn_Delete.Enabled = false;

        ////if (this.ToolBar1.IsExitsContral(NamesOfBtn.Copy) == true)
        ////    this.Btn_Copy.Enabled = false;

        //this.UCEn1.Bind(this.CurrEn, this.CurrEn.ToString(), false, false);

        //this.PKVal = this.CurrEn.PKVal;
    }
    public void Copy()
    {
        try
        {
            this.PKVal = null;
            Entity en = this.UCEn1.GetEnData(this.GetEns.GetNewEntity);
            en.Copy();
            this.UCEn1.Bind(en, en.ToString(), this.IsReadonly, true);
        }
        catch (Exception ex)
        {
            this.ResponseWriteRedMsg(ex);
        }
    }
    /// <summary>
    /// delete
    /// </summary>
    public void Delete()
    {
        Entity en = this.GetEnDa;
        en.PKVal = this.PKVal;
        en.Delete();
        this.WinClose();
    }
    public void Save()
    {
        Entity en = this.GetEns.GetNewEntity;
        if (this.PKVal != null)
        {
            en.PKVal = this.PKVal;
            en.RetrieveFromDBSources();
        }

        en = this.UCEn1.GetEnData(en);

        this.CurrEn = en;
        en.Save();
        this.PKVal = en.PKVal;

        #region 保存 实体附件
        try
        {
            if (en.EnMap.Attrs.Contains("MyFileName"))
            {
                HtmlInputFile file = this.UCEn1.FindControl("file") as HtmlInputFile;
                if (file != null && file.Value.IndexOf(".") != -1)
                {
                    //求出保存路径.
                    string path = en.EnMap.FJSavePath;

                    if (path == "" || path == null || path == string.Empty)
                        path= BP.Sys.SystemConfig.PathOfDataUser +en.ToString() + "\\";

                    if (System.IO.Directory.Exists(path) == false)
                        System.IO.Directory.CreateDirectory(path);

                    /* 如果包含这二个字段。*/
                    string fileName = file.PostedFile.FileName;
                    fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);

                    en.SetValByKey("MyFilePath", path);

                    string ext = "";
                    if (fileName.IndexOf(".") != -1)
                        ext = fileName.Substring(fileName.LastIndexOf(".") + 1);

                    string reldir = path;
                    if (reldir.Length > SystemConfig.PathOfDataUser.Length)
                        reldir =
                            reldir.Substring(reldir.ToLower().IndexOf(@"\datauser\") + @"\datauser\".Length).Replace(
                                @"\", "/");
                    else
                        reldir = "";

                    if (reldir.Length > 0 && Equals(reldir[0], '/') == true)
                        reldir = reldir.Substring(1);

                    if (reldir.Length > 0 && Equals(reldir[reldir.Length - 1], '/') == false)
                        reldir += "/";

                    en.SetValByKey("MyFileExt", ext);
                    en.SetValByKey("MyFileName", fileName);
                    en.SetValByKey("WebPath", "/DataUser/"+ reldir + en.PKVal + "." + ext);

                    string fullFile = path + @"\" + en.PKVal + "." + ext;

                    //检测是否已经存在此文件
                    string tmpFile = path + @"\" + en.PKVal + ".tmp";
                    if (System.IO.File.Exists(fullFile))
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(fullFile);
                        if (fi.IsReadOnly)
                            fi.IsReadOnly = false;

                        System.IO.File.Copy(fullFile, tmpFile, true);
                        System.IO.File.Delete(fullFile);
                    }

                    try
                    {
                        file.PostedFile.SaveAs(fullFile);

                        try
                        {
                            if (System.IO.File.Exists(tmpFile))
                                System.IO.File.Delete(tmpFile);
                        }
                        catch { }
                    }
                    catch (Exception ex0)
                    {
                        if (System.IO.File.Exists(tmpFile))
                            System.IO.File.Move(tmpFile, fullFile);

                        throw ex0;
                    }

                    file.PostedFile.InputStream.Close();
                    file.PostedFile.InputStream.Dispose();
                    file.Dispose();

                    System.IO.FileInfo info = new System.IO.FileInfo(fullFile);
                    en.SetValByKey("MyFileSize", BP.DA.DataType.PraseToMB(info.Length));
                    if (DataType.IsImgExt(ext))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(fullFile);
                        en.SetValByKey("MyFileH", img.Height);
                        en.SetValByKey("MyFileW", img.Width);
                        img.Dispose();
                    }
                    en.Update();
                }
            }
        }
        catch (Exception ex)
        {
            this.Alert("保存附件出现错误：" + ex.Message);
        }
        #endregion

        #region 保存 属性 附件
        try
        {
            AttrFiles fils = en.EnMap.HisAttrFiles;
            SysFileManagers sfs = new SysFileManagers(en.ToString(), en.PKVal.ToString());
            foreach (AttrFile fl in fils)
            {
                HtmlInputFile file = (HtmlInputFile)this.UCEn1.FindControl("F" + fl.FileNo);
                if (file.Value.Contains(".") == false)
                    continue;

                SysFileManager enFile = sfs.GetEntityByKey(SysFileManagerAttr.AttrFileNo, fl.FileNo) as SysFileManager;
                SysFileManager enN = null;
                if (enFile == null)
                {
                    enN = this.FileSave(null, file, en);
                }
                else
                {
                    enFile.Delete();
                    enN = this.FileSave(null, file, en);
                }

                enN.AttrFileNo = fl.FileNo;
                enN.AttrFileName = fl.FileName;
                enN.EnName = en.ToString();
                enN.Update();
            }
        }
        catch (Exception ex)
        {
            this.Alert("保存附件出现错误：" + ex.Message);
        }
        #endregion
    }
    /// <summary>
    /// 文件保存
    /// </summary>
    /// <param name="fileNameDesc"></param>
    /// <param name="File1"></param>
    /// <returns></returns>
    private SysFileManager FileSave(string fileNameDesc, HtmlInputFile File1, Entity myen)
    {
        SysFileManager en = new SysFileManager();
        en.EnName = myen.ToString();
        // en.FileID = this.RefPK + "_" + count.ToString();
        EnCfg cfg = new EnCfg(en.EnName);

        string filePath = cfg.FJSavePath; // BP.Sys.SystemConfig.PathOfFDB + "\\" + this.EnName + "\\";
        if (System.IO.Directory.Exists(filePath) == false)
            System.IO.Directory.CreateDirectory(filePath);

        string ext = System.IO.Path.GetExtension(File1.PostedFile.FileName);
        ext = ext.Replace(".", "");
        en.MyFileExt = ext;
        if (fileNameDesc == "" || fileNameDesc == null)
            en.MyFileName = System.IO.Path.GetFileNameWithoutExtension(File1.PostedFile.FileName);
        else
            en.MyFileName = fileNameDesc;
        en.RDT = DataType.CurrentData;
        en.RefVal = myen.PKVal.ToString();
        en.MyFilePath = filePath;
        en.Insert();

        string fileName = filePath + en.OID + "." + en.MyFileExt;
        File1.PostedFile.SaveAs(fileName);

        File1.PostedFile.InputStream.Close();
        File1.PostedFile.InputStream.Dispose();
        File1.Dispose();

        System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
        en.MyFileSize = DataType.PraseToMB(fi.Length);

        if (DataType.IsImgExt(en.MyFileExt))
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            en.MyFileH = img.Height;
            en.MyFileW = img.Width;
            img.Dispose();
        }
        en.WebPath = cfg.FJWebPath + en.OID + "." + en.MyFileExt;
        en.Update();
        return en;
    }

    public void EnList()
    {
        this.Response.Redirect(this.Request.ApplicationPath + "/Comm/UIEns.aspx?EnsName=" + this.EnsName + "&inlayer=" + this.InLayer, true);
    }
    #endregion
}
