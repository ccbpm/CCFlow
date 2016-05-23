
using System;
using System.Data;
using System.IO;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Sys.XML;
using BP.Sys.XML;


namespace BP.Web.UC
{
    /// <summary>
    ///	UCWFRpt 的摘要说明。
    /// </summary>
    public class UCBase : System.Web.UI.UserControl
    {
        /// <summary>
        /// 页面Index.
        /// </summary>
        public int PageIdx
        {
            get
            {
                string str = this.Request.QueryString["PageIdx"];
                if (str == null || str == "")
                    return 1;
                return int.Parse(str);
            }
            set
            {
                ViewState["PageIdx"] = value;
            }
        }
        protected string ExportDGToExcel(System.Data.DataTable dt, string title)
        {
            title = title.Trim();
            string filename = "Ep" + title + ".xls";
            string file = filename;
            bool flag = true;
            string filepath = this.Request.PhysicalApplicationPath + "\\Temp\\";

            #region 参数及变量设置
            //			//参数校验
            //			if (dg == null || dg.Items.Count <=0 || filename == null || filename == "" || filepath == null || filepath == "")
            //				return null;

            //如果导出目录没有建立，则建立.
            if (Directory.Exists(filepath) == false)
                Directory.CreateDirectory(filepath);

            filename = filepath + filename;

            //删除原有的重名文件，否则在下载时，会造成下载内容的一些乱码 added by liuxc,2014-11-3
            if (File.Exists(filename))
                File.Delete(filename);

            FileStream objFileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter objStreamWriter = new StreamWriter(objFileStream, System.Text.Encoding.Unicode);
            #endregion

            #region 生成导出文件
            try
            {
                objStreamWriter.WriteLine(Convert.ToChar(9) + title + Convert.ToChar(9));
                string strLine = "";
                //生成文件标题
                foreach (DataColumn attr in dt.Columns)
                {
                    strLine = strLine + attr.ColumnName + Convert.ToChar(9);
                }

                objStreamWriter.WriteLine(strLine);
                strLine = "";
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn attr in dt.Columns)
                    {
                        strLine = strLine + dr[attr.ColumnName] + Convert.ToChar(9);
                    }
                    objStreamWriter.WriteLine(strLine);
                    strLine = "";
                }
                //    objStreamWriter.WriteLine();
                //   objStreamWriter.WriteLine(Convert.ToChar(9) + " 制表人：" + Convert.ToChar(9) + WebUser.Name + Convert.ToChar(9) + "日期：" + Convert.ToChar(9) + DateTime.Now.ToShortDateString());
            }
            catch
            {
                flag = false;
            }
            finally
            {
                objStreamWriter.Close();
                objFileStream.Close();
            }
            #endregion

            #region 删除掉旧的文件
            //DelExportedTempFile(filepath);
            #endregion

            if (flag)
            {

                BP.Web.Controls.Glo MyFileDown = new BP.Web.Controls.Glo();
                MyFileDown.DownFileByPath(filename,
                     file);
                //this.WinOpen(this.Request.ApplicationPath + "/Temp/" + file,"down",90,90);
                //this.Write_Javascript(" window.open('"+ Request.ApplicationPath + @"/Report/Exported/" + filename +"'); " );
                //this.Write_Javascript(" window.open('"+Request.ApplicationPath+"/Temp/" + file +"'); " );
            }

            return file;
        }
        protected void WinOpenShowModalDialog(string url, string title, string key, int width, int height, int top, int left)
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), key, "<script language='JavaScript'>window.showModalDialog('" + url + "','" + key + "' ,'dialogHeight: 500px; dialogWidth:" + width + "px; dialogTop: " + top + "px; dialogLeft: " + left + "px; center: yes; help: no' ) ;  </script> ");
        }
        protected void Alert(string mess)
        {
            if (string.IsNullOrEmpty(mess))
                return;

            this.Alert(mess, false);
        }
        /// <summary>
        /// 不用page 参数，show message
        /// </summary>
        /// <param name="mess"></param>
        protected void Alert(string mess, bool isClent, bool goBack = false)
        {
            if (string.IsNullOrEmpty(mess))
                return;

            //this.ResponseWriteRedMsg(mess);
            //return;

            mess = mess.Replace("@@", "@");
            mess = mess.Replace("@@", "@");

            mess = mess.Replace("'", "＇");

            mess = mess.Replace("\"", "＇");

            mess = mess.Replace("\"", "＂");

            mess = mess.Replace(";", "；");
            mess = mess.Replace(")", "）");
            mess = mess.Replace("(", "（");

            mess = mess.Replace(",", "，");
            mess = mess.Replace(":", "：");

            mess = mess.Replace("<", "［");
            mess = mess.Replace(">", "］");

            mess = mess.Replace("[", "［");
            mess = mess.Replace("]", "］");

            mess = mess.Replace("'", "‘");

            mess = mess.Replace("@", "\\n@");
            string script = string.Format("<script language=JavaScript>alert('{0}');{1}</script>", mess, goBack ? "history.back();" : "");
            if (isClent)
                System.Web.HttpContext.Current.Response.Write(script);
            else
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "kesy", script);
            //this.RegisterStartupScript("key1", script);
        }
        protected void Alert(Exception ex, bool goBack = false)
        {
            this.Alert(ex.Message, false, goBack);
        }
        public int RefOID
        {
            get
            {
                string s = this.Request.QueryString["RefOID"];
                if (s == null)
                    s = this.Request.QueryString["OID"];
                if (s == null)
                    return 0;
                return int.Parse(s);
            }
        }
        public string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string RefNo
        {
            get
            {
                string s = this.Request.QueryString["RefNo"];
                if (s == null)
                    s = this.Request.QueryString["No"];
                return s;
            }
        }
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        private string _pageID = null;
        public string PageID
        {
            get
            {
                if (_pageID == null)
                {
                    string url = System.Web.HttpContext.Current.Request.RawUrl;
                    int i = url.LastIndexOf("/") + 1;
                    int i2 = url.IndexOf(".aspx") - 6;
                    try
                    {
                        url = url.Substring(i);
                        _pageID = url.Substring(0, url.IndexOf(".aspx"));

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                    }
                }
                return _pageID;
            }
        }
        public Entity GenerEnValForView(Entity en)
        {
            Map map = en.EnMap;
            string msg = "";
            foreach (Attr attr in map.Attrs)
            {
                string ctlid = "";
                if (attr.UIContralType == UIContralType.DDL)
                    ctlid = "DDL_" + attr.Key;
                else
                    ctlid = "TB_" + attr.Key;

                if (this.IsExit(ctlid) == false)
                    continue;

                try
                {
                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        try
                        {
                            this.GetDDLByID(ctlid).SetSelectItem(en.GetValStrByKey(attr.Key));
                        }
                        catch
                        {

                        }

                        try
                        {
                            this.GetDropDownListByID(ctlid).SelectedValue = en.GetValStrByKey(attr.Key);
                            continue;
                        }
                        catch
                        {

                        }
                        continue;
                    }

                    try
                    {
                        this.GetTBByID(ctlid).Text = en.GetValStrByKey(attr.Key);
                        this.GetTBByID(ctlid).MaxLength = attr.MaxLength;

                        continue;
                    }
                    catch
                    {
                    }

                    try
                    {

                        this.GetTextBoxByID(ctlid).Text = en.GetValStrByKey(attr.Key);
                        this.GetTextBoxByID(ctlid).MaxLength = attr.MaxLength;
                        continue;
                    }
                    catch
                    {
                    }
                }
                catch (Exception ex)
                {
                    msg += ex.Message;
                }
            }
            return en;
        }
        /// <summary>
        /// 复制一个新的 Entities .
        /// </summary>
        /// <param name="ens">包含数据的Ens</param>
        /// <returns></returns>
        public Entities Copy(Entities ens)
        {
            foreach (Entity en in ens)
                this.Copy(en, en.PKVal.ToString());
            return ens;
        }
        /// <summary>
        /// copy实体
        /// </summary>
        /// <param name="en"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        public Entity Copy(Entity en, string pk=null)
        {
            return this.Copy(en, pk, en.EnMap);
        }
        /// <summary>
        /// 执行复制
        /// </summary>
        /// <param name="en">实体</param>
        /// <param name="pk">主键</param>
        /// <param name="map">映射</param>
        /// <returns>数据实体</returns>
        public Entity Copy(Entity en, string pk, Map map)
        {
            if (pk == null || pk == "")
            {
                pk = "";
            }
            else
            {
                return CopyRow(en, pk, map);
                // pk = "_" + pk;
            }

            #region 判断外部数据。
            foreach (string paramKey in Request.Params.AllKeys)
                en.SetValByKey(paramKey, Request[paramKey]);
            #endregion

            foreach (System.Web.UI.Control ctl in this.Controls)
            {
                if (ctl == null || string.IsNullOrEmpty(ctl.ID))
                    continue;

                string ctlid = ctl.ID;
                string key = null;

                #region 处理textbox.
                if (ctlid.Contains("TB_") == true)
                {
                    key = ctlid.Replace("TB_", "");
                    TB tb = ctl as TB;
                    if (tb != null)
                    {
                        if (!tb.Enabled || tb.ReadOnly)
                        {
                            if (Request.Path.EndsWith("Frm.aspx") || Request.Path.EndsWith("MyFlow.aspx"))
                            {
                                foreach (string paramKey in Request.Params.AllKeys)
                                {
                                    if (paramKey.EndsWith(tb.ID))
                                    {
                                        en.SetValByKey(key, Request[paramKey]);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            en.SetValByKey(key, tb.Text);
                        }
                        continue;
                    }

                    TextBox mytb = ctl as TextBox;
                    if (mytb != null)
                    {
                        if (!mytb.Enabled || mytb.ReadOnly)
                        {
                            if (Request.Path.EndsWith("Frm.aspx") || Request.Path.EndsWith("MyFlow.aspx"))
                            {
                                foreach (string paramKey in Request.Params.AllKeys)
                                {
                                    if (paramKey.EndsWith(mytb.ID))
                                    {
                                        en.SetValByKey(key, Request[paramKey]);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            en.SetValByKey(key, mytb.Text);
                        }
                        continue;
                    }
                }
                #endregion 处理textbox.

                #region 处理ddl.
                if (ctlid.Contains("DDL_") == true)
                {
                    key = ctlid.Replace("DDL_", "");
                    DDL ddl = ctl as DDL;
                    if (ddl != null)
                    {
                        if (ddl.Items.Count == 0)
                            continue;

                        en.SetValByKey(key, ddl.SelectedValue);

                        //设置他的影子字段值.
                        if (ddl.SelectedItem.Text != "请选择")
                            en.SetValByKey(key + "T", ddl.SelectedItem.Text);
                        continue;
                    }

                    DropDownList myddl = ctl as DropDownList;
                    if (myddl != null)
                    {
                        if (myddl.Items.Count == 0)
                            continue;

                        en.SetValByKey(key, myddl.SelectedValue);

                        //设置他的影子字段值.
                        if (myddl.SelectedItem.Text != "请选择")
                            en.SetValByKey(key + "T", myddl.SelectedItem.Text);
                    }
                    continue;
                }
                #endregion 处理ddl.

                #region 处理 checkbox
                if (ctlid.Contains("CB_") == true)
                {
                    key = ctlid.Replace("CB_", "");
                    CheckBox cb = ctl as CheckBox;
                    if (cb != null)
                    {
                        if (cb.Checked)
                            en.SetValByKey(key, 1);
                        else
                            en.SetValByKey(key, 0);
                    }
                }
                #endregion

                #region 处理 RadioButton.
                if (ctlid.Contains("RB_") == true)
                {
                    RadioButton radio = ctl as RadioButton;
                    if (radio == null)
                        continue;
                    if (radio.Checked == false)
                        continue;

                    key = ctlid.Replace("RB_", "");
                    string val = key.Substring(key.LastIndexOf('_') + 1);
                    if (key.LastIndexOf('_') > 0)
                        key = key.Substring(0, key.LastIndexOf('_'));

                    en.SetValByKey(key, val);
                    continue;
                }
                #endregion
            }

            if (map.IsHaveAutoFull == false)
                return en;
            en.AutoFull();
            return en;
        }
        public Entity CopyRow(Entity en, string pk, Map map)
        {
            if (pk == null || pk == "")
                pk = "";
            else
                pk = "_" + pk;

            Attrs attrs = map.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                string ctlid = "";
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        ctlid = "TB_" + attr.Key + pk;
                        System.Web.UI.Control ctl = this.FindControl(ctlid);
                        if (ctl == null)
                            continue;

                        TB tb = ctl as TB;
                        if (tb != null)
                        {
                            en.SetValByKey(attr.Key, tb.Text);
                            continue;
                        }

                        TextBox mytb = ctl as TextBox;
                        if (mytb != null)
                        {
                            if (mytb.ReadOnly == true)
                            {
                                if (Request.Path.EndsWith("Dtl.aspx"))
                                {
                                    foreach (string paramKey in Request.Params.AllKeys)
                                    {
                                        if (paramKey.EndsWith(ctlid))
                                        {
                                            en.SetValByKey(attr.Key, Request[paramKey]);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                en.SetValByKey(attr.Key, mytb.Text);
                            }
                            continue;
                        }
                        break;
                    case UIContralType.DDL:
                        ctlid = "DDL_" + attr.Key + pk;
                        System.Web.UI.Control ctl_ddl = this.FindControl(ctlid);
                        DDL ddl = ctl_ddl as DDL;
                        if (ddl != null)
                        {
                            en.SetValByKey(attr.Key, ddl.SelectedValue);

                            //设置他的值.
                            if (ddl.SelectedItem.Text != "请选择")
                                en.SetValByKey(attr.Key + "T", ddl.SelectedItem.Text);
                            continue;
                        }

                        DropDownList myddl = ctl_ddl as DropDownList;
                        if (myddl != null)
                        {
                            en.SetValByKey(attr.Key, myddl.SelectedValue);

                            //设置他的值.
                            if (myddl.SelectedItem.Text != "请选择")
                                en.SetValByKey(attr.Key + "T", myddl.SelectedItem.Text);
                        }
                        continue;
                    case UIContralType.CheckBok:
                        ctlid = "CB_" + attr.Key + pk;
                        CheckBox cb = this.GetCBByID(ctlid);
                        if (cb != null)
                        {
                            if (cb.Checked)
                                en.SetValByKey(attr.Key, 1);
                            else
                                en.SetValByKey(attr.Key, 0);
                        }
                        break;
                    case UIContralType.RadioBtn:
                        if (attr.IsEnum)
                        {
                            SysEnums ses = new SysEnums(attr.UIBindKey);
                            foreach (SysEnum se in ses)
                            {
                                string id = "RB_" + attr.Key + "_" + se.IntKey;
                                RadioButton rb = this.GetRBLByID(id);

                                #region 如果是空的,有可能是标记它是 rb 但是它用的ddl 显示的.
                                if (rb == null)
                                {
                                    ctlid = "DDL_" + attr.Key + pk;
                                    System.Web.UI.Control ctl_ddl_rb = this.FindControl(ctlid);
                                    DDL myddlrb = ctl_ddl_rb as DDL;
                                    if (myddlrb != null)
                                    {
                                        en.SetValByKey(attr.Key, myddlrb.SelectedValue);
                                        break;
                                    }

                                    DropDownList myddl22 = ctl_ddl_rb as DropDownList;
                                    if (myddl22 != null)
                                        en.SetValByKey(attr.Key, myddl22.SelectedValue);
                                    break;
                                }
                                #endregion 如果是空的

                                if (rb != null && rb.Checked)
                                {
                                    en.SetValByKey(attr.Key, se.IntKey);
                                    break;
                                }
                            }
                        }
                        if (attr.MyFieldType == FieldType.FK)
                        {
                            Entities ens = BP.En.ClassFactory.GetEns(attr.UIBindKey);
                            ens.RetrieveAll();
                            foreach (Entity enNoName in ens)
                            {
                                RadioButton rb = this.GetRBLByID(attr.Key + "_" + enNoName.GetValStringByKey(attr.UIRefKeyValue));
                                if (rb != null && rb.Checked)
                                {
                                    en.SetValByKey(attr.Key, enNoName.GetValStrByKey(attr.UIRefKeyValue));
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (attr.MyDataType == DataType.AppBoolean)
                {
                    ctlid = "RB_" + attr.Key + pk + "_1";
                    bool isOk = true;
                    if (this.IsExit(ctlid))
                    {
                        RadioButton rb = this.FindControl(ctlid) as RadioButton;
                        isOk = rb.Checked;
                        en.SetValByKey(attr.Key, isOk);
                        continue;
                    }

                    ctlid = "CB_" + attr.Key + pk;
                    if (this.IsExit(ctlid))
                    {
                        CheckBox cb = this.FindControl(ctlid) as CheckBox;
                        isOk = cb.Checked;
                        en.SetValByKey(attr.Key, isOk);
                        continue;
                    }
                }
            }
            if (map.IsHaveAutoFull == false)
                return en;
            en.AutoFull();
            return en;
        }

        /// <summary>
        /// 重设置里面的信息
        /// </summary>
        /// <param name="en"></param>
        public void ResetEnVal(Entity en)
        {
            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                string ctlid = "";
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        ctlid = "TB_" + attr.Key;
                        TB tb = this.GetTBByID(ctlid);
                        if (tb != null)
                        {
                            tb.Text = en.GetValStrByKey(attr.Key);
                            continue;
                        }

                        TextBox mytb = this.GetTextBoxByID(ctlid);
                        if (mytb != null)
                        {
                            tb.Text = en.GetValStrByKey(attr.Key);
                            continue;
                        }

                        break;
                    case UIContralType.DDL:
                        try
                        {
                            ctlid = "DDL_" + attr.Key;
                            DDL ddl = this.GetDDLByID(ctlid);
                            if (ddl != null)
                            {
                                ddl.SetSelectItem(en.GetValStrByKey(attr.Key));
                                continue;
                            }

                            DropDownList myddl = this.GetDropDownListByID(ctlid);
                            if (myddl != null)
                                ddl.SetSelectItem(en.GetValStrByKey(attr.Key));
                        }
                        catch
                        {

                        }
                        continue;
                    case UIContralType.CheckBok:
                        ctlid = "CB_" + attr.Key;
                        CheckBox cb = this.GetCBByID(ctlid);
                        if (cb != null)
                        {
                            cb.Checked = en.GetValBooleanByKey(attr.Key);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public void BindTable(DataTable dt)
        {
            this.AddTable();
            this.AddTR();
            foreach (DataColumn dc in dt.Columns)
            {
                this.AddTDTitle(dc.ColumnName);
            }
            this.AddTREnd();
            foreach (DataRow dr in dt.Rows)
            {
                this.AddTR();
                foreach (DataColumn dc in dt.Columns)
                {
                    this.AddTD(dr[dc.ColumnName].ToString());
                }
                this.AddTREnd();
            }
            this.AddTableEnd();
        }
        public void BindEns(Entities ens)
        {
            Attrs attrs = ens.GetNewEntity.EnMap.Attrs;
            //this.AddTable();
            this.AddTableNormal();
            this.AddTR();

            foreach (Attr attr in attrs)
            {
                if (attr.Key == "MyNum" || attr.UIIsDoc == true)
                    continue;

                if (attr.IsRefAttr || attr.UIVisible == false)
                    continue;

                //this.AddTDTitle(attr.Desc);
                this.AddTDGroupTitle(attr.Desc);
            }

            this.AddTREnd();

            bool is1 = false;
            foreach (Entity en in ens)
            {
                is1 = this.AddTR(is1);
                foreach (Attr attr in attrs)
                {
                    if (attr.Key == "MyNum" || attr.UIIsDoc == true)
                        continue;

                    if (attr.IsRefAttr || attr.UIVisible == false)
                        continue;

                    if (attr.UIHeight != 0 && attr.IsNum == false)
                        continue;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppFloat:
                        case DataType.AppDouble:
                        case DataType.AppInt:
                            this.AddTDNum(en.GetValStringByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.AddTDNum(en.GetValDecimalByKey(attr.Key).ToString("0.00"));
                            break;
                        default:
                            this.AddTD(en.GetValStrByKey(attr.Key));
                            break;
                    }
                }
                this.AddTREnd();
            }

            this.AddTableEnd();
        }

        public void BindCard(Entity en)
        {
            this.BindCard(en, "Table");
        }
        public void BindCard(Entity en, string tableClass)
        {
            if (en.HisUAC.IsView == false)
            {
                this.AddMsgOfWarning("提示", "您没有查看此该数据的权限。");
                return;
            }

            Attrs attrs = en.EnMap.Attrs;
            this.Add("<table class='" + tableClass + "' >");
            bool is1 = false;
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (attr.UIVisible == false)
                    continue;

                if (attr.Key == "MyNum" || attr.Key == "Doc")
                    continue;

                is1 = this.AddTR(is1);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyDataType == DataType.AppString && attr.MaxLength > 400
                       )
                    {
                        this.AddTD("class=BigDoc colspan=2", "<B>" + attr.Desc + "</b><BR>" + en.GetValHtmlStringByKey(attr.Key));
                    }
                    else
                    {
                        this.AddTD("<B>" + attr.Desc + "</b>");
                        this.AddTD(en.GetValStrByKey(attr.Key));
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    this.AddTD("<B>" + attr.Desc + "</b>");

                    if (en.GetValBooleanByKey(attr.Key))
                        this.AddTD("是");

                    else
                        this.AddTD("否");
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {

                    this.AddTD("<B>" + attr.Desc + "</b>");
                    this.AddTD(en.GetValRefTextByKey(attr.Key));
                }

                this.AddTREnd();
            }
            this.AddTableEnd();
        }

        /// <summary>
        /// 使用EasyUI中的easyui-pagination分页组件进行分页显示
        /// <para></para>
        /// <para>注意：此方法需要页面引入easyui库才有效</para>
        /// <para>added by liuxc,2014-11-3</para>
        /// </summary>
        /// <param name="totalRecords">记录总条数</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="pageIdx">当前页码</param>
        /// <param name="url">翻页时中转的URL</param>
        /// <param name="layout">分页显示布局，此设置请参考EasyUi中的说明[EasyUI v1.3.5之后版本支持此属性]
        /// <para></para>
        /// <para>格式如：'list','sep','first','prev','sep','manual','sep','next','last','sep','refresh'</para>
        /// <para>1) list: 显示分页条数列表[10,20,30,50].</para>
        /// <para>2) sep: 分隔符.</para>
        /// <para>3) first: 第一页.</para>
        /// <para>4) prev: 前一页.</para>
        /// <para>5) next: 后一页.</para>
        /// <para>6) last: 最末页.</para>
        /// <para>7) refresh: 刷新按钮.</para>
        /// <para>8) manual: 允许用户输入页码的文本框.</para>
        /// <para>9) links: 显示10个页码链接.</para>
        /// </param> 
        /// <param name="showParentPanel">是否显示分页外层的easyui-panel</param>
        public void BindPageIdxEasyUi(int totalRecords, string url, int pageIdx, int pageSize = 10, string layout = "'first','prev','sep','manual','sep','next','last'", bool showParentPanel = false)
        {
            this.Add("    <style type='text/css'>"
                     + "        #eupage table,#eupage td"
                     + "        {"
                     + "            border: 0;"
                     + "            padding: 0;"
                     + "            text-align: inherit;"
                     + "            background-color: inherit;"
                     + "            color: inherit;"
                     + "            font-size: inherit;"
                     + "        }"
                     + "    </style>");

            if (showParentPanel)
                this.Add("<div class='easyui-panel'>" + Environment.NewLine);

            this.Add(string.Format("<div id='eupage' class='easyui-pagination' data-options=\"" + Environment.NewLine
                    + "total: {0}," + Environment.NewLine
                    + "pageSize: {1}," + Environment.NewLine
                    + "pageNumber: {2}," + Environment.NewLine
                    + "showPageList: false," + Environment.NewLine
                    + "showRefresh: false," + Environment.NewLine
                    + "layout: [{3}]," + Environment.NewLine
                    + "beforePageText: '第&nbsp;'," + Environment.NewLine
                    + "afterPageText: '&nbsp;/ {{pages}} 页'," + Environment.NewLine
                    + "displayMsg: '显示 {{from}} 到 {{to}} 条，共 {{total}} 条'\"" + Environment.NewLine
                + "></div>" + Environment.NewLine, totalRecords, pageSize, pageIdx, layout));

            if (showParentPanel)
                this.Add("</div>" + Environment.NewLine);

            this.Add("<script type='text/javascript'>" + Environment.NewLine);
            this.Add(string.Format("$('#eupage').pagination({{" + Environment.NewLine
                                   + "	onSelectPage:function(pageNumber, pageSize){{" + Environment.NewLine
                                   + "		location.href='{0}&PageIdx=' + pageNumber" + Environment.NewLine
                                   + "	}}" + Environment.NewLine
                                   + "}});" + Environment.NewLine, url));
            this.Add("</script>" + Environment.NewLine);
        }

        public int BindPageIdx(int recNum, int pageSize, int PageIdx, string url)
        {
            return BindPageIdx(recNum, pageSize, PageIdx, url, 20);
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="recNum">记录个数</param>
        /// <param name="pageSize">叶面大小</param>
        /// <param name="PageIdx"></param>
        /// <param name="url"></param>
        /// <returns>返回最大页面数</returns>
        public int BindPageIdx(int recNum, int pageSize, int PageIdx, string url, int pageSpan)
        {
            if (recNum <= pageSize)
                return 1;

            this.Add("<div style='text-align:center;'>");
            string appPath = this.Request.ApplicationPath;
            int myidx = 0;
            if (PageIdx <= 1)
            {
                //this.Add("《- 《-");
                this.Add("<img style='vertical-align:middle' src='/WF/Img/Arr/LeftEnd.png' border=0/><img style='vertical-align:middle' src='/WF/Img/Arr/Left.png' border=0/>");
            }
            else
            {
                myidx = PageIdx - 1;
                //this.Add("<a href='" + url + "&PageIdx=1' >《-</a> <a href='" + url + "&PageIdx=" + myidx + "'>《-</a>");

                this.Add("<a href='" + url + "&PageIdx=1' ><img style='vertical-align:middle' src='/WF/Img/Arr/LeftEnd.png' border=0/></a><a href='" + url + "&PageIdx=" + myidx + "'><img style='vertical-align:middle' src='/WF/Img/Arr/Left.png' border=0/></a>");

            }

            int pageNum = 0;
            decimal pageCountD = decimal.Parse(recNum.ToString()) / decimal.Parse(pageSize.ToString()); // 页面个数。
            string[] strs = pageCountD.ToString("0.0000").Split('.');
            if (int.Parse(strs[1]) > 0)
                pageNum = int.Parse(strs[0]) + 1;
            else
                pageNum = int.Parse(strs[0]);

            int from = 0;
            int to = 0;

            decimal spanTemp = decimal.Parse(PageIdx.ToString()) / decimal.Parse(pageSpan.ToString()); // 页面个数。

            strs = spanTemp.ToString("0.0000").Split('.');
            from = int.Parse(strs[0]) * pageSpan;
            to = from + pageSpan;
            for (int i = 1; i <= pageNum; i++)
            {
                if (i >= from && i < to)
                {
                }
                else
                {
                    continue;
                }

                if (PageIdx == i)
                    this.Add("&nbsp;<font style='font-weight:bloder;color:#f00'>" + i + "</font>&nbsp;");
                else
                    this.Add("&nbsp;<a href='" + url + "&PageIdx=" + i + "'>" + i + "</a>");
            }

            if (PageIdx != pageNum)
            {
                myidx = PageIdx + 1;
                //this.Add("&nbsp;<a href='" + url + "&PageIdx=" + myidx + "'>-》</a>&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'>-》</a>&nbsp;&nbsp;Page:" + PageIdx + "/" + pageNum + " Total:" + recNum + ".");
                this.Add("&nbsp;<a href='" + url + "&PageIdx=" + myidx + "'><img style='vertical-align:middle' src='/WF/Img/Arr/Right.png' border=0/></a>&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'><img style='vertical-align:middle' src='/WF/Img/Arr/RightEnd.png' border=0/></a>&nbsp;&nbsp;页数:" + PageIdx + "/" + pageNum + "&nbsp;&nbsp;总数:" + recNum);
            }
            else
            {
                //this.Add("&nbsp;<a href='" + url + "&PageIdx=" + pageNum + "'> -》》</a>&nbsp;&nbsp;Page:" + PageIdx + "/" + pageNum + " Totlal:" + recNum + ".");
                this.Add("&nbsp;<img style='vertical-align:middle' src='/WF/Img/Arr/Right.png' border=0/>&nbsp;&nbsp;");
                this.Add("&nbsp;<img style='vertical-align:middle' src='/WF/Img/Arr/RightEnd.png' border=0/>&nbsp;&nbsp;页数:" + PageIdx + "/" + pageNum + "&nbsp;&nbsp;总数:" + recNum);
                //this.Add("<img src='/WF/Img/Page_Down.gif' border=1 />");
            }
            this.Add("</div>");
            return pageNum;
            //this.UCPub3.AddTDEnd();
            //this.UCPub3.AddTREnd();
            //this.UCPub3.AddTableEnd();
        }

        public int BindPageIdx_ver1(int recNum, int pageSize, int PageIdx, string url)
        {
            int pageSpan = 20;
            if (recNum <= pageSize)
            {
                this.Add("<div class=PageIdx><ul><li href=#>首页</li> <li href=#>上一页</li> <li href=#>下一页</li> <li href=#>尾页</li>,<li href=#>共" + recNum + "</li>条.</DIV>");
                return 1;
            }

            //int PageIdx=1;
            //if (this.Request.QueryString["PageIdx"]==null)
            //    PageIdx=1;
            //else
            //    PageIdx = int.Parse(this.Request.QueryString["PageIdx"]);
            //if (recNum < PageIdx*pageSize)
            //	PageIdx= recNum/pageSize;
            //this.UCPub3.Clear();
            //this.UCPub3.AddTableRed();
            //this.UCPub3.AddTR();
            //this.UCPub3.Add("<TD class=TD>");

            int myidx = 0;
            if (PageIdx <= 1)
            {
                this.Add("<div class=PageIdx><li href=#>首页</li> <li href=#>上一页</li>");
                //  this.Add("&nbsp;<img src='/WF/Img/Page_Up.gif' border=1 />");
            }
            else
            {
                myidx = PageIdx - 1;
                //this.Add("&nbsp;<a href='" + url + "&PageIdx=" + myidx + "'><img src='/WF/Img/Page_Up.gif' border=0 /></a>");
                this.Add("<div class=PageIdx><li><a href='" + url + "&PageIdx=1'>首页</a></li> <li><a href='" + url + "&PageIdx=" + myidx + "'>上一页</a></li>");
            }

            int pageNum = 0;
            decimal pageCountD = decimal.Parse(recNum.ToString()) / decimal.Parse(pageSize.ToString()); // 页面个数。
            string[] strs = pageCountD.ToString("0.0000").Split('.');
            if (int.Parse(strs[1]) > 0)
                pageNum = int.Parse(strs[0]) + 1;
            else
                pageNum = int.Parse(strs[0]);

            int from = 0;
            int to = 0;

            decimal spanTemp = decimal.Parse(PageIdx.ToString()) / decimal.Parse(pageSpan.ToString()); // 页面个数。

            strs = spanTemp.ToString("0.0000").Split('.');
            from = int.Parse(strs[0]) * pageSpan;
            to = from + pageSpan;
            for (int i = 1; i <= pageNum; i++)
            {
                if (i >= from && i < to)
                {
                }
                else
                {
                    continue;
                }

                if (PageIdx == i)
                    this.Add("<li><a class=pickred>" + i + "</a></li>");
                else
                    this.Add("<li><a><a href='" + url + "&PageIdx=" + i + "'>" + i + "</a></li>");
            }
            if (PageIdx != pageNum)
            {
                myidx = PageIdx + 1;
                // this.Add("&nbsp;<a href='" + url + "&PageIdx=" + myidx + "'><img src='/WF/Img/Page_Down.gif' border=0 /></a>&nbsp;第" + PageIdx + "/" + pageNum + "页");
                this.Add("<li><a href='" + url + "&PageIdx=" + myidx + "'>下一页</a></li> <li>第" + PageIdx + "/<a href='" + url + "&PageIdx=" + pageNum + "'>" + pageNum + "</a>页</li> <li><a href='" + url + "&PageIdx=" + pageNum + "'>尾页</a>, " + recNum + "条.</li></div>");
            }
            else
            {
                this.Add("<li>下一页</li> <li>第" + PageIdx + "/" + pageNum + "页</li> <li><a href='" + url + "&PageIdx=" + pageNum + "'>尾页</a></li>,<li>共" + recNum + "条.</li></div>");
                // this.Add("<img src='/WF/Img/Page_Down.gif' border=1 />");
            }
            return pageNum;
            //this.UCPub3.AddTDEnd();
            //this.UCPub3.AddTREnd();
            //this.UCPub3.AddTableEnd();
        }
        public RadioBtn GetRadioBtnByID(string id)
        {
            return (RadioBtn)this.FindControl(id);
        }
        public RadioButton GetRadioButtonByID(string id)
        {
            return (RadioButton)this.FindControl(id);
        }
        public CheckBox GetCBByID(string id)
        {
            return (CheckBox)this.FindControl(id);
        }
        public Label GetLabelByID(string id)
        {
            return this.FindControl(id) as Label;
        }
        public TB GetTBByID(string key)
        {
            try
            {
                return (TB)this.FindControl(key);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " 请确认：TB AND TextBox " + key);
            }
        }
        public TextBox GetTextBoxByID(string key)
        {
            return (TextBox)this.FindControl(key);
        }
        public RadioButton GetRBLByID(string id)
        {
            return this.FindControl(id) as RadioButton;
        }
        public RadioButtonList GetRadioButtonListByID(string id)
        {
            return (RadioButtonList)this.FindControl(id);
        }
        public DDL GetDDLByID(string key)
        {
            return (DDL)this.FindControl(key);
        }
        public LB GetLBByID(string key)
        {
            return (LB)this.FindControl(key);
        }
        public DropDownList GetDropDownListByID(string key)
        {
            return (DropDownList)this.FindControl(key);
        }
        public ImageButton GetImageButtonByID(string key)
        {
            return (ImageButton)this.FindControl(key);
        }
        public void EnableAllBtn(bool isEnable)
        {
            foreach (System.Web.UI.Control c in this.Controls)
            {
                Btn btn = c as Btn;
                if (btn != null)
                {
                    btn.Enabled = isEnable;
                    continue;
                }
                Button myBtn = c as Button;
                if (myBtn != null)
                {
                    myBtn.Enabled = isEnable;
                    continue;
                }
            }
        }

        public LinkBtn GetLinkBtnByID(string key)
        {
            return (LinkBtn)this.FindControl(key);
        }

        public Btn GetBtnByID(string key)
        {
            return (Btn)this.FindControl(key);
        }

        public Button GetButtonByID(string key)
        {
            return this.FindControl(key) as Button;
        }

        public void AddContralDDL(DDL ddl)
        {
            this.Add("<TD class=TD >");
            this.Controls.Add(ddl);
            if (ddl.Enabled)
            {
                string srip = "javascript:HalperOfDDL('" + ddl.AppPath + "','" + ddl.SelfBindKey + "','" + ddl.SelfEnsRefKey + "','" + ddl.SelfEnsRefKeyText + "','" + ddl.ClientID.ToString() + "' ); ";
                //this.Controls.Add( new LiteralControl("<a href=\"javascript:"+srip+"\" >aaaa</a></td>") ); 
                this.Add("<input type='button' value='...' onclick=\"" + srip + "\"  name='b" + ddl.ID + "'  >");
            }
            this.Add("</TD>");
        }
        public void AddTDDocCard(string str)
        {
            this.Add("\n<TD   >" + str + "</TD>");
        }
        public void AddContral(string desc, DDL ddl, bool isRefBtn)
        {
            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td><td class=TD nowrap>");
            this.Controls.Add(ddl);
            if (ddl.Enabled)
            {
                if (ddl.SelfBindKey.IndexOf(".") == -1)
                {
                    this.AddTDEnd();
                }
                else
                {
                    if (isRefBtn && ddl.Items.Count > 15)
                    {
                        string srip = "javascript:HalperOfDDL('" + ddl.AppPath + "','" + ddl.SelfBindKey + "','" + ddl.SelfEnsRefKey + "','" + ddl.SelfEnsRefKeyText + "','" + ddl.ClientID.ToString() + "' ); ";
                        this.Add("<input type='button' value='...' onclick=\"" + srip + "\" name='b" + ddl.ID + "' ></td>");
                    }
                    else
                    {
                        this.AddTDEnd();
                    }
                }
            }
            else
            {
                this.AddTDEnd();
            }
        }
        public void AddContral(string desc, DDL ddl, bool isRefBtn, int colspan)
        {
            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td><td  colspan=" + colspan + " nowrap>");
            this.Controls.Add(ddl);
            if (ddl.Enabled)
            {
                if (ddl.SelfBindKey.IndexOf(".") == -1)
                {
                    this.AddTDEnd();
                }
                else
                {
                    if (isRefBtn && ddl.Items.Count > 4)
                    {
                        string srip = "javascript:HalperOfDDL('" + ddl.AppPath + "','" + ddl.SelfBindKey + "','" + ddl.SelfEnsRefKey + "','" + ddl.SelfEnsRefKeyText + "','" + ddl.ClientID.ToString() + "' ); ";
                        this.Add("<input type='button' value='...' onclick=\"" + srip + "\" name='b" + ddl.ID + "' ></td>");
                    }
                    else
                    {
                        this.AddTDEnd();
                    }
                }
            }
            else
            {
                this.AddTDEnd();
            }
        }
        /// <summary>
        /// ParseControl
        /// </summary>
        protected void ParseControl()
        {
            this.Controls.Add(this.ParseControl(this.Text));
        }
        public void AddIframeAutoSize(string url, string frmID, string tdID)
        {
            this.Add("<iframe ID='" + frmID + "' src='" + url + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%'  height='100%' scrolling=no /></iframe>");
            string js = "\t\n<script type='text/javascript' >";
            {
                js += "\t\n window.setInterval(\"ReinitIframe('" + frmID + "','" + tdID + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);
        }
        public void AddIframeAutoSizeAndScroll(string url, string frmID, string tdID)
        {
            this.Add("<iframe ID='" + frmID + "' src='" + url + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%'  height='100%' /></iframe>");
            string js = "\t\n<script type='text/javascript' >";
            {
                js += "\t\n window.setInterval(\"ReinitIframe('" + frmID + "','" + tdID + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);
        }

        public void AddIframeExt(string url, string attrs)
        {
            //this.Add("<iframe frameborder=1 leftMargin='0'  onload=\"onloadIt('fm')\"  topMargin='0' src='" + url + "' width='100%' height='100%' class=iframe name=fm style='border-style:none;' id=fm > </iframe>");
            this.Add("<iframe   src='" + url + "' " + attrs + "   id=fm > </iframe>");
        }

        public void AddIframeWithOnload(string url)
        {
            //this.Add("<iframe frameborder=1 leftMargin='0'  onload=\"onloadIt('fm')\"  topMargin='0' src='" + url + "' width='100%' height='100%' class=iframe name=fm style='border-style:none;' id=fm > </iframe>");
            this.Add("<iframe frameborder=1 leftMargin='0'  topMargin='0' src='" + url + "' width='100%' height='100%' class=iframe name=fm style='border-style:none;' id=fm > </iframe>");
        }
        public void AddIframe(string url)
        {
            this.Add("<iframe frameborder=1 leftMargin='0' topMargin='0' src='" + url + "' width='100%' height='100%' class=iframe name=fm style='border-style:none;' id=fm > </iframe>");
        }
        public void AddIframe(string title, string url)
        {
            this.Add("<span class=TD >" + title + "</span>");
            this.Add("<iframe leftMargin='0' topMargin='0' src='" + url + "' width='100%' height='100%' class=iframe  name=fm > </iframe>");
        }
        public void AddIframeItem3(string enName, string pk, string title)
        {
            AddIframe(title, "Comm/Item3.aspx?EnName=" + enName + "&PK=" + pk);
        }
        public void AddIframeItem3(string enName, string pk)
        {
            AddIframe("Comm/Item3.aspx?EnName=" + enName + "&PK=" + pk);
        }
        public void AddFieldSetRound(string title)
        {
            this.Add("<table cellpadding='0' cellspacing='0'><tr><td><fieldset align='center'><legend>" + title + " </legend>");
        }
        public void AddFieldSetRoundEnd()
        {
            this.Add("</fieldset></td></tr></table>");
        }
        public void AddFieldSet(RadioButton rb)
        {
            this.Add("<fieldset ><legend>");
            this.Add(rb);
            this.Add("</legend>");
        }
        public void AddFieldSet(string title)
        {
            this.Add("<fieldset width='100%' ><legend>&nbsp;" + title + "&nbsp;</legend>");
        }
        public void AddFieldSetGreen(string title)
        {
            this.Add("<fieldset ><legend>&nbsp;<font color=green><b>" + title + "</b></font>&nbsp;</legend>");
        }
        public void AddFieldSet(string title, string doc)
        {
            this.Add("<fieldset class='FieldsetInfo' ><legend>&nbsp;" + title + "&nbsp;</legend>");

            this.Add(doc);

            this.AddFieldSetEnd();
        }
        public void LoadPop_del()
        {

            //          this.Page.RegisterClientScriptBlock("sdds",
            //"<link href='/WF/Comm/JS/jquery-easyui/themes/default/easyui.css' rel='stylesheet' type='text/css' />");

            //          this.Page.RegisterClientScriptBlock("db7",
            //       "<script language='JavaScript' src='" + this.Request.ApplicationPath + "Comm/JS/jquery-easyui/jquery-1.4.4.min.js'></script>");

            //          this.Page.RegisterClientScriptBlock("db8",
            //      "<script language='JavaScript' src='" + this.Request.ApplicationPath + "Comm/JS/jquery-easyui/query.easyui.min.js'></script>");


            //   this.Page.RegisterClientScriptBlock("sds",
            // "<link href='/WF/Comm/JS/pop/skin/qq/ymPrompt.css' rel='stylesheet' type='text/css' />");

            //   this.Page.RegisterClientScriptBlock("db7",
            //"<script language='JavaScript' src='" + this.Request.ApplicationPath + "Comm/JS/pop/ymPrompt.js'></script>");
        }
        public void AlertMsgOpenClose(string title, string msg, string ctrlid)
        {
            this.AddMsgOfInfo(title, msg);
            return;
        }
        public void AlertMsg_Info(string title, string msg)
        {
            this.AddMsgOfInfo(title, msg);
            return;
        }
        public void AlertMsg_Warning(string title, string msg)
        {
            this.AddMsgOfWarning(title, msg);
            return;

            // this.Alert(msg, false);
            // return;

            // this.LoadPop();

            // this.Add("<div id=myMsg style='display:none;'><div style='text-align:left' >" + msg + "</div></div>");
            // string js = "<script language=JavaScript >";
            //// js += "\t\n $.messager.alert('" + title + "', document.getElementById('myMsg').innerHTML ,'warning'); ";
            // //js += "";

            // js += "\t\n  alert('sdsds') ";

            // js += "\t\n $.messager.alert('" + title + "', 'sss','warning'); ";

            // js += "</script>";

            // this.Page.ClientScript.RegisterStartupScript(this.GetType(), "kesy", js);

            //         <script language="JavaScript" src="../Comm/JS/pop/ymPrompt.js" ></script>
            //<link rel="stylesheet" type="text/css" href="../Comm/JS/pop/skin/qq/ymPrompt.css" /> 

            //this.Add("<div id=myMsg style='display:none;'><div style='text-align:left' >" + msg + "</div></div>");
            //string js = "<script language=JavaScript >";
            //js += "\t\n ymPrompt.setDefaultCfg({btn:'ok'}) ; ";
            //js += "\t\n ymPrompt.errorInfo({message: document.getElementById('myMsg').innerHTML,title:'" + title + "',height:380,width:400,fixPosition:true,dragOut:false,allowSelect:true});";
            //js += "</script>";
            //this.Page.ClientScript.RegisterStartupScript(this.GetType(), "kesy", js);
        }

        public void AddFieldSetNone(string title)
        {
            this.Add("<fieldset class=FieldSetNone ><legend>&nbsp;" + title + "&nbsp;</legend>");
        }

        public void AddFieldSetYellow(string title)
        {
            this.Add("<fieldset class=FieldSetYellow ><legend>&nbsp;" + title + "&nbsp;</legend>");
        }

        public void AddFieldSetYellow(string title, string msg)
        {
            this.Add("<fieldset class=FieldSetYellow ><legend>&nbsp;" + title + "&nbsp;</legend>");
            this.Add(msg);
            this.Add("</fieldset>");
        }

        public void AddFieldSetBlue(string title)
        {
            this.Add("<fieldset class=FieldSetBlue ><legend>&nbsp;" + title + "&nbsp;</legend>");
        }
        public void AddFieldSetBlue(string title, string msg)
        {
            this.Add("<fieldset class=FieldSetBlue ><legend>&nbsp;" + title + "&nbsp;</legend>");
            this.Add(msg);
            this.Add("</fieldset>");
        }
        public void AddFieldSetRed(string title, string msg)
        {
            this.Add("<fieldset class=FieldSetRed ><legend>&nbsp;" + title + "&nbsp;</legend>");
            this.Add(msg);
            this.Add("</fieldset>");
        }
        public void AddFieldSetEnd()
        {
            this.Add("</fieldset>");
        }

        public void AddFieldSetEndBR()
        {
            this.Add("</fieldset><BR>");
        }
        public void AddLiInLine(string html)
        {
            this.Add("<li style='display:inline'>" + html + "</li>");
        }
        public void AddUL()
        {
            this.Add("<ul>");
        }
        public void AddUL(string attr)
        {
            this.Add("<ul " + attr + ">");
        }
        public void AddULTagCloud()
        {
            this.Add("<ul class='TagCloud'>");
        }
        public void AddULEnd()
        {
            this.Add("</ul>\t\n");
        }

        public void AddLi(string html)
        {
            this.Add("<li>" + html + "</li> \t\n");
        }
        public void AddLi(string url, string lab)
        {
            this.Add("<li><a href=\"" + url + "\">" + lab + "</a></li>");
        }
        public void AddLiB(string url, string lab)
        {
            this.Add("<li><a href=\"" + url + "\"><b>" + lab + "</b></a></li>");
        }
        public void AddLiB(string lab)
        {
            this.Add("<li><b>" + lab + "</b></li>");
        }
        public void AddLi(string url, string lab, string target)
        {
            this.Add("<li><a href=\"" + url + "\" target=" + target + ">" + lab + "</a></li>");
        }
        public void AddDiv_del(string html)
        {
            this.Add("<div>" + html + "</div>");
        }

        public void AddDivRound(string msg, int width)
        {
            this.AddDivRound();
            this.Add(msg);
            this.AddDivRoundEnd();
        }
        public void AddDivRound()
        {
            this.Add("<div class='r_info'>");
            this.Add("<p><img src='../Style/Img/right_line_t3.jpg' /></p>");
            this.Add("<div class='info_in'>");
        }
        public void AddDivRound(int width)
        {
            this.Add("<div class='r_info'>");
            this.Add("<p><img src='../Style/Img/right_line_t3.jpg' width='" + width + "px' /></p>");
            this.Add("<div class='info_in'>");
        }
        public void AddDivRoundEnd()
        {
            this.Add("</div>");
            this.Add("<p><img src='../Style/Img/right_line_b3.jpg' /></p>");
            this.Add("</div>");
        }

        public void AddDiv()
        {
            this.Add("<div class=RoundedCorner><b class='rtop'><b class='r1'></b><b class='r2'></b><b class='r3'></b><b class='r4'></b></b><p class=divP>");
        }

        public void AddDiv(string title, string html)
        {
            this.AddDiv();
            this.Add("<div><b>" + title + "</b>" + html + "</div>");
            this.AddDivEnd();
        }
        public void AddDivEnd()
        {
            this.Add("</div>");
            //this.Add("</p><b class='rbottom'><b class='r4'></b><b class='r3'></b><b class='r2'></b><b class='r1'></b></b></div>");
        }
        public void AddDivEndBR()
        {
            this.AddDivEnd();
            this.AddBR();
        }

        public void AddH(string url, string lab, string t)
        {
            this.Add("<a href=\"" + url + "\" target=" + t + " >" + lab + "</a>");
        }

        public void AddH(string url, string lab)
        {
            this.Add("<a href=\"" + url + "\" >" + lab + "</a>");
        }
        public void AddLeftRight(string left, string right)
        {
            this.Add("<table border=0 width='100%'><TR><TD align=left>" + left + "</TD><TD align=right>" + right + "</TD></TR></table>");
        }

        public void Add(string s)
        {
            if (s == null || s == "")
                return;
            this.Controls.Add(this.ParseControl(s));
        }
        public void AddB(string s)
        {
            if (s == null || s == "")
                return;
            this.Controls.Add(this.ParseControl("<B>" + s + "</B>"));
        }
        public void AddBR()
        {
            this.Controls.Add(this.ParseControl("<BR>"));
        }
        public void AddHR()
        {
            this.Controls.Add(this.ParseControl("<HR>"));
        }
        public void AddHR(string msg)
        {
            this.Controls.Add(this.ParseControl("<HR>" + msg));
        }
        public void AddP(string s)
        {
            if (s == null)
                return;
            this.Controls.Add(this.ParseControl("<P>" + s + "</P>"));
        }
        public void AddH1(string s)
        {
            if (s == null)
                return;

            this.Add("<H1>" + s + "</H1>");
        }
        public void AddH2(string s)
        {
            if (s == null)
                return;

            this.Add("<H2>" + s + "</H2>");
        }
        public void AddH3(string s)
        {
            if (s == null)
                return;
            this.Add("<H3>" + s + "</H3>");
        }
        public void AddH4(string s)
        {
            if (s == null)
                return;

            this.Add("<H4>" + s + "</H4>");
        }
        public void AddBR(string msg)
        {
            this.Controls.Add(this.ParseControl("<BR>" + msg));
        }
        public void AddSpace(int num)
        {
            this.Controls.Add(this.ParseControl(BP.DA.DataType.GenerSpace(num)));
        }
        public void Add(System.Web.UI.Control ctl)
        {
            this.Controls.Add(ctl);
        }
        public void AddBR(System.Web.UI.Control ctl)
        {
            this.Controls.Add(this.ParseControl("<BR>"));
            this.Controls.Add(ctl);
        }
        public void AddTable()
        {
            this.Add("<Table class='Table' cellpadding='2' cellspacing='2'>");
        }
        public void AddTableEnd()
        {
            this.Add("</Table>");
        }
        public void AddTableEndWithHR()
        {
            this.Add("</Table><HR>");
        }
        public void AddTableEndWithBR()
        {
            this.Add("</Table><Br>");
        }
        public void AddTable(string attr)
        {
            //this.Add("<Table id='table_01' "+attr+" >");
            this.Add("<Table " + attr + " >");
        }
        /// <summary>
        /// 增加一般的table，width=100%
        /// </summary>
        public void AddTableNormal()
        {
            this.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
        }
        public void AddTable(string id, string styleClass)
        {
            this.Add("<Table class='" + id + "'  cellpadding='0' cellspacing='0' class='" + styleClass + "'>");
        }
        public void AddTDNum(TB tb)
        {
            this.Add("\n<TD class='TDNum' nowrap >");
            this.Add(tb);
            this.Add("</TD>");
        }
        public void AddTDNum(TextBox tb)
        {
            this.Add("\n<TD class='TDNum' nowrap >");
            this.Add(tb);
            this.Add("</TD>");
        }
        public void AddTDNum(string str)
        {
            this.Add("\n<TD class='TDNum' nowrap >" + str + "</TD>");
        }
        public void AddTDNum(decimal str)
        {
            this.Add("\n<TD class='TDNum' nowrap >" + str.ToString() + "</TD>");
        }
        public void AddTDJE(decimal str)
        {
            this.Add("\n<TD class='TDNum' nowrap >" + str.ToString("0.00") + "</TD>");
        }
        public void AddTDDate(string str)
        {
            if (str == null || str == "")
                this.Add("\n<TD  nowrap class='TBDate' >&nbsp;</TD>");
            else
                this.Add("\n<TD  nowrap class='TBDate' >" + str + "</TD>");
        }
        public void AddTD(string str)
        {
            if (str == null || str == "")
                this.Add("\n<TD  nowrap >&nbsp;</TD>");
            else
                this.Add("\n<TD  nowrap >" + str + "</TD>");
        }
        public void AddTDA(string href, string str)
        {
            this.Add("\n<TD  nowrap ><a href=\"" + href + "\">" + str + "</a></TD>");
        }
        public void AddTDA(string href, string str, string target)
        {
            this.Add("\n<TD  nowrap ><a href=\"" + href + "\" target=" + target + ">" + str + "</a></TD>");
        }

        public void Href(string href, string str, string target)
        {
            this.Add("<a href=\"" + href + "\" target=" + target + ">" + str + "</A>");
        }
        public void Href(string href, string str, string target, int blank)
        {
            this.Add("<a href=\"" + href + "\" target=" + target + ">" + str + "</A>" + BP.DA.DataType.GenerSpace(blank));
        }
        public void Href(string href, string str)
        {
            this.Add("<a href=\"" + href + "\" >" + str + "</A>");
        }

        public void Href(string href, string str, int blank)
        {
            this.Add("<a href=\"" + href + "\">" + str + "</A>" + BP.DA.DataType.GenerSpace(blank));
        }

        public void AddTDM(string str)
        {
            this.Add("\n<TD class='TDM' nowrap >" + str + "</TD>");
        }
        public void AddTDMS(string str)
        {
            this.Add("\n<TD class='TDMS' nowrap >" + str + "</TD>");
        }

        public void AddTDA(string href, int str)
        {
            this.Add("\n<TD class='TDNum' nowrap ><a href=\"" + href + "\">" + str + "</TD>");
        }
        public void AddTD(bool val)
        {
            if (val)
                this.Add("\n<TD >是</TD>");
            else
                this.Add("\n<TD >否</TD>");
        }
        public void AddTDBegin(string attr)
        {
            this.Add("\n<TD " + attr + " nowrap >");
        }
        public void AddTDBegin()
        {
            this.Add("\n<TD valign=top nowrap >");
        }
        public void AddTDEnd()
        {
            this.Add("\n</TD>");
        }

        public void AddTDInfoBegin()
        {
            this.Add("\n<TD  nowrap bgcolor=InfoBackground >");
        }
        public void AddTDInfo(string str)
        {
            this.Add("\n<TD  nowrap bgcolor=InfoBackground >" + str + "</TD>");
        }
        public void AddTDInfo()
        {
            this.Add("\n<TD  nowrap bgcolor=InfoBackground >&nbsp;</TD>");
        }
        public void AddTDInfo(string attr, System.Web.UI.Control str)
        {
            this.Add("\n<TD  " + attr + "  nowrap bgcolor=InfoBackground>");
            this.Controls.Add(str);
            this.Add("</TD>");
        }
        public void AddTDInfo(System.Web.UI.Control str)
        {
            this.Add("\n<TD   nowrap bgcolor=InfoBackground>");
            this.Controls.Add(str);
            this.Add("</TD>");
        }
        public void AddTDCenter(System.Web.UI.Control ctl)
        {
            this.Add("\n<TD   nowrap >");
            this.Controls.Add(ctl);
            this.Add("</TD>");
        }

        public void AddTDCenter(string str)
        {
            this.Add("\n<TD align=center nowrap >" + str + "</TD>");
        }
        public void AddTD()
        {
            this.Add("\n<TD >&nbsp;</TD>");
        }
        public void AddTDToolbar(string str)
        {
            this.Add("\n<TD class='Toolbar' nowrap >" + str + "</TD>");
        }
        public void AddTR()
        {
            this.Add("\n<TR>");
        }
        public void AddTRHand()
        {
            this.Add("\n<TR class='TRHand' >");
        }
        public void AddTRHand(string attr)
        {
            this.Add("\n<TR class='TRHand' " + attr + " >");
        }
        public void AddTRTXHand()
        {
            this.Add("\n<TR class='TRHand' onmouseover='TROver(this)' onmouseout='TROut(this)' >");
        }
        public void AddTRTXHand(string attr)
        {
            this.Add("\n<TR class='TRHand' onmouseover='TROver(this)' onmouseout='TROut(this)' " + attr + " >");
        }
        public void AddTR(string attr)
        {
            this.Add("\n<TR " + attr + " >");
        }
        public void AddTRSum()
        {
            this.Add("\n<TR class='TRSum' >");
        }
        public void AddTRRed()
        {
            this.Add("\n<TR class='TRRed' >");
        }
        public void AddTR1()
        {
            this.Add("\n<TR class=TR1 >");
        }

        public bool AddTR(bool item, string attr)
        {
            if (item)
                this.Add("\n<TR bgcolor=AliceBlue " + attr + " >");
            else
                this.Add("\n<TR bgcolor=white " + attr + " class=TR>");

            item = !item;
            return item;
        }
        public bool AddTR(ref bool item)
        {
            if (item)
                this.Add("\n<TR bgcolor=AliceBlue >");
            else
                this.Add("\n<TR bgcolor=white class=TR>");

            item = !item;
            return item;
        }
        public bool AddTR(bool item)
        {
            if (item)
                this.Add("\n<TR bgcolor=AliceBlue >");
            else
                this.Add("\n<TR bgcolor=white >");

            item = !item;
            return item;
        }
        public void AddTRTXRed()
        {
            this.Add("\n<TR  bgcolor=red >");
        }
        /// <summary>
        /// 加上特殊效果
        /// </summary>
        public void AddTRTX()
        {
            this.Add("\n<TR onmouseover='TROver(this)' onmouseout='TROut(this)' >");
        }
        public void AddTRTX(string attr)
        {
            this.Add("\n<TR onmouseover='TROver(this)' onmouseout='TROut(this)' " + attr + ">");
        }
        public void AddTREnd()
        {
            this.Add("\n</TR>");
        }
        public void AddTDIdx(int idx)
        {
            this.Add("\n<TD class='Idx' nowrap>" + idx + "</TD>");
        }
        public void AddTDIdx(System.Web.UI.Control ctl)
        {
            this.Add("\n<TD class='Idx' nowrap>");
            this.Add(ctl);
            this.Add("</TD>");
        }
        public void AddTDIdx(int idx, System.Web.UI.Control ctl)
        {
            this.Add("\n<TH class='Idx' nowrap >" + idx);
            this.Add(ctl);
            this.Add("</TH>");
        }
        public void AddTDIdx(string idx)
        {
            this.Add("\n<TH class='Idx' nowrap >" + idx + "</TH>");
        }
        public void AddTDH(string url, string lab)
        {
            this.Add("\n<TD  nowrap ><a href='" + url + "'>" + lab + "</a></TD>");
        }
        public void AddTDH(string url, string lab, string target)
        {
            this.Add("\n<TD  nowrap ><a href='" + url + "' target=" + target + ">" + lab + "</a></TD>");
        }
        public void AddTDH(string url, string lab, string target, string img)
        {
            this.Add("\n<TD  nowrap ><a href='" + url + "' target=" + target + "><img src='" + img + "' border=0/>" + lab + "</a></TD>");
        }

        public void AddCheckBoxsByEntities(Entities ens, string selectVals, string fieldName)
        {
            foreach (Entity en in ens)
            {
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + fieldName + "_" + en.GetValStringByKey("No");
                cb.Text = en.GetValStringByKey("Name");
                if (string.IsNullOrEmpty(selectVals) == false)
                    cb.Checked = selectVals.Contains("," + en.GetValStringByKey("No") + ",");
                this.Add(cb);
            }
        }
        public void AddCheckBoxsByEntities(DataTable dt, string selectVals, string fieldName, int rowNum)
        {
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + fieldName + "_" + dr["No"];
                cb.Text = dr["Name"].ToString();
                if (string.IsNullOrEmpty(selectVals) == false)
                    cb.Checked = selectVals.Contains("," + dr["No"] + ",");
                this.Add(cb);
                if (idx >= rowNum)
                {
                    this.AddBR();
                    idx = 0;
                }
            }
        }

        public void AddCheckBoxsByEntities(string sql, string selectVals, string fieldName)
        {
            AddCheckBoxsByEntities(sql, selectVals, fieldName, 100);
        }
        public void AddCheckBoxsByEntities(string sql, string selectVals, string fieldName, int rowNum)
        {
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            AddCheckBoxsByEntities(dt, selectVals, fieldName, rowNum);
        }
        /// <summary>
        /// 增加选择
        /// </summary>
        /// <param name="enumID"></param>
        /// <param name="selectVals"></param>
        /// <param name="fieldName"></param>
        public void AddCheckBoxsByEnum(string enumID, string selectVals, string fieldName)
        {
            SysEnums ses = new SysEnums(enumID);
            foreach (SysEnum se in ses)
            {
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + fieldName + "_" + se.IntKey;
                cb.Text = se.Lab;
                if (string.IsNullOrEmpty(selectVals) == false)
                    cb.Checked = selectVals.Contains("," + se.IntKey + ",");
                this.Add(cb);
            }
        }
        /// <summary>
        /// 增加选择
        /// </summary>
        /// <param name="enumID"></param>
        /// <param name="selectVals"></param>
        public void AddCheckBoxsByEnum(string enumID, string selectVals)
        {
            AddCheckBoxsByEnum(enumID, selectVals, enumID);
        }
        public void AddTD(CheckBox cb)
        {
            this.Add("\n<TD >");
            this.Add(cb);
            this.Add("</TD>");
        }
        public void AddTD(System.Web.UI.Control ctl)
        {
            this.Add("\n<TD >");
            this.Add(ctl);
            this.Add("</TD>");
        }
        public void AddTD(System.Web.UI.Control ctl, string note)
        {
            this.Add("\n<TD  >");
            this.Add(ctl);
            this.Add(note + "</TD>");
        }
        public void AddTD(string attr, System.Web.UI.Control ctl)
        {
            this.Add("\n<TD " + attr + " >");
            this.Add(ctl);
            this.Add("</TD>");
        }

        public void AddTD(string attr, string msgdec, System.Web.UI.Control ctl)
        {
            msgdec = msgdec.Trim();
            if (msgdec == null || msgdec == "")
            {
            }
            else
            {
                msgdec += "<BR>";
            }

            this.Add("\n<TD " + attr + " >" + msgdec);
            this.Add(ctl);
            this.Add("</TD>");
        }
        public void AddTD(string attr, System.Web.UI.WebControls.WebControl ctl)
        {
            this.Add("\n<TD  nowrap " + attr + "  >");
            this.Add(ctl);
            this.Add("</TD>");
        }
        public void AddTDBar(string str)
        {
            this.Add("\n<TD class='Bar' nowrap=true >" + str + "</TD>");
        }
        public void AddCaption(string str)
        {
            this.Add("\n<Caption >" + str + "</Caption>");
        }
        /// <summary>
        /// 短标题
        /// </summary>
        /// <param name="str"></param>
        public void AddCaptionMsg(string str)
        {
            this.Add("\n<Caption ><div class='CaptionMsg' >" + str + "</div></Caption>");
        }
        /// <summary>
        /// 长标题
        /// </summary>
        /// <param name="str"></param>
        public void AddCaptionMsgLong(string str)
        {
            this.Add("\n<Caption ><div class='CaptionMsgLong' >" + str + "</div></Caption>");
        }
        public void AddTableBarGreen(string title, int col)
        {
            this.Add("\n<TR class='TR'>");
            this.Add("\n<TD class='TitleGreen'  colspan=" + col + " >" + title + "</TD>");
            this.Add("\n</TR>");
        }
        public void AddTableBarGreen(string title)
        {
            AddTableBarGreen(title, 1);
        }
        public void AddTableBarGreen(int col)
        {
            AddTableBarGreen("&nbsp;", col);
        }
        public void AddTableBarBlue(string title, int col)
        {
            this.Add("\n<TR class='TRBlue'>");
            this.Add("\n<TD class='TitleBlue'  colspan=" + col + " >" + title + "</TD>");
            this.Add("\n</TR>");
        }
        public void AddTableBarBlue(int col)
        {
            AddTableBarBlue("&nbsp;", col);
        }

        public void AddTableBarRed(string imgUrl, string title, int col, string leftMore)
        {
            string msg = "";
            if (imgUrl != null)
                msg = "<table border=0 width='100%'  class='BarTable'  ><TR class='BarTableTR' ><TD class='BarTableTD' >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";
            else
                msg = "<table border=0 width='100%'  class='BarTable'  ><TR class='BarTableTR' ><TD class='BarTableTD' >&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";

            //this.AddCaption(msg);

            this.Add("\n<TR class='TR' height='0%' >");
            this.Add("\n<TD class='TitleRed' height='0%' colspan=" + col + " >" + msg + "</TD>");
            this.Add("\n</TR>");
        }
        public void AddTableBarRed(string title, int col)
        {
            this.Add("\n<TR class='TRRed'>");
            this.Add("\n<TD class='TitleRed'  colspan=" + col + " >" + title + "</TD>");
            this.Add("\n</TR>");
        }
        public void AddCaptionLeft(string str)
        {
            this.Add("\n<Caption  >" + str + "</Caption>");
        }
        public void AddCaptionRight(string str)
        {
            this.Add("\n<Caption class='Caption' align=right >" + str + "</Caption>");
        }
        public void AddCaptionLeftTX(string str)
        {
            this.Add("\n<Caption class='Caption' align=left >" + str + "</Caption>");
            return;
        }
        public void AddCaptionLeftTX2(string str)
        {
            string table = "<table class='Caption' width='1px' border=0><tr><td nowrap=true >" + str + "</td></tr></table>";
            this.Add("\n<Caption class='Caption' align=left >" + str + "</Caption>");
        }
        public void AddTDTitle(string attr, System.Web.UI.Control ctl)
        {
            this.Add("\n<TH " + attr + ">");
            this.Add(ctl);
            this.Add("</TH>");
        }
        public void AddTDTitle(System.Web.UI.Control ctl)
        {
            this.Add("\n<TH>");
            this.Add(ctl);
            this.Add("</TH>");
        }
        public void AddTDTitle()
        {
            this.Add("\n<TH>&nbsp;</TH>");
        }
        public void AddTDTitleLeft(string attr, string str)
        {
            this.Add("\n<TH align=left nowrap=true>" + str + "</TH>");
        }
        public void AddTDTitleLeft(string str)
        {
            this.Add("\n<TH align=left nowrap=true>" + str + "</TH>");
        }
        public void AddTDDoc(string str)
        {
            this.Add("\n<TD>" + str + "</TD>");
        }
        public void AddTDBigDoc(string attr, string str)
        {
            this.Add("\n<TD class='BigDoc' " + attr + " valign=top>" + str + "</TD>");
        }
        public void AddTDBigDocBegain()
        {
            this.Add("\n<TD class='BigDoc' valign=top>");
        }
        public void AddTDBigDoc(string attr, TextBox tb)
        {
            this.Add("\n<TD class='BigDoc' " + attr + " valign=top >");
            this.Add(tb);
            this.Add("</TD>");
        }
        public void AddTDBigDoc(string str)
        {
            this.Add("\n<TD class='BigDoc' valign=top>" + str + "</TD>");
        }
        public void AddTDDoc(string str, int len, string title)
        {
            if (str.Length >= len)
                this.Add("\n<TD  nowrap title=\"" + title.Replace("<BR>", "\n") + "\" >" + str.Substring(0, len) + "...</TD>");
            else
                this.Add("\n<TD nowrap>" + str + "</TD>");
        }
        public void AddTDDoc(string str, string title)
        {
            if (str == null || str.Length == 0)
            {
                this.Add("\n<TD nowrap>...</TD>");
            }

            if (str.Length > 20)
                this.Add("\n<TD  nowrap title=\"" + title.Replace("<BR>", "\n") + "\" >" + str.Substring(0, 20) + "...</TD>");
            else
                this.Add("\n<TD  nowrap>" + str + "</TD>");
        }
        public bool IsExit(string ctlID)
        {
            foreach (System.Web.UI.Control ctl in this.Controls)
            {
                if (ctl.ID == null)
                    continue;

                if (ctl.ID == ctlID)
                    return true;
            }
            return false;
        }
        public void AddTDTitleExt(string str)
        {
            this.Add("\n<TD class='TitleExt' nowrap=true >" + str + "</TD>");
        }
        public void AddTDTitle(string attr, string str)
        {
            this.Add("\n<TH class='Title' " + attr + " nowrap=true >" + str + "</TH>");
        }
        public void AddTDB(string str)
        {
            this.Add("\n<TD  nowrap=true ><b>" + str + "</b></TD>");
        }

        public void AddTDB(string attr, string str)
        {
            this.Add("\n<TD  " + attr + " nowrap=true ><b>" + str + "</b></TD>");
        }
        public void AddTDGroupTitle()
        {
            this.Add("\n<TD class='GroupTitle' nowrap ></TD>");

        }

        public void AddTDGroupTitle(string str)
        {
            this.Add("\n<TD class='GroupTitle' nowrap>" + str + "</TD>");
        }

        public void AddTDGroupTitleCenter(string str)
        {
            this.Add("\n<TD class='GroupTitle' style='text-align:center'>" + str + "</TD>");
        }

        public void AddTDGroupTitle(string attr, string str)
        {
            this.Add("\n<TD class='GroupTitle' " + attr + " >" + str + "</TD>");
        }
        public void AddTDGroupTitle(string attr, System.Web.UI.Control ctl)
        {
            this.Add("\n<TD class='GroupTitle'  " + attr + " >");
            this.Add(ctl);
            this.Add("</TD>");
        }
        public void AddTRGroupTitle(string attr, string str)
        {
            this.AddTR();
            this.AddTDGroupTitle(attr, str);
            this.AddTREnd();
        }
        public void AddTRGroupTitle(string str)
        {
            this.AddTR();
            this.AddTDGroupTitle(str);
            this.AddTREnd();
        }
        public void AddTRGroupTitle(int colspan, string str)
        {
            this.AddTR();
            this.AddTDGroupTitle(string.Format("colspan='{0}'", colspan == 0 ? 1 : colspan), str);
            this.AddTREnd();
        }
        public void AddTDTitle(string str)
        {
            this.Add("\n<TH >" + str + "</TH>");
        }
        public void AddTDTitleGroup(string str)
        {
            this.Add("\n<TD class='GroupTitle' >" + str + "</TD>");
        }
        public void AddTDDesc(string str)
        {
            this.Add("\n<TD class='FDesc' nowrap=true >" + str + "</TD>");
        }
        public void AddTDDesc(string str, int colspan)
        {
            this.Add("\n<TD class='FDesc' nowrap=true colspan=" + colspan + " >" + str + "</TD>");
        }
        public void AddTDTitleHelp(string str, string msg)
        {
            string path = this.Request.ApplicationPath;
            this.Add("\n<TH  nowrap=true title=\"" + msg + "\" >" + str + "<img src='./" + this.Request.ApplicationPath + "WF/Img/Btn/Help.gif' onclick=\"javascript:alert( '" + msg + "' )\"  border=0></TH>");
        }
        public void AddTD(int val)
        {
            this.Add("\n<TD class='TDNum'  >" + val + "</TD>");
        }
        public void AddTD(Int64 val)
        {
            this.Add("\n<TD class='TDNum' >" + val + "</TD>");
        }
        public void AddTD(decimal val)
        {
            this.Add("\n<TD class='TDNum' >" + val + "</TD>");
        }
        public void AddTDMoney(decimal val)
        {
            this.Add("\n<TD class='TDNum' >" + val.ToString("0.00") + "</TD>");
        }
        public void AddTD(float val)
        {
            this.Add("\n<TD class='TDNum' >" + val + "</TD>");
        }
        public void AddTD(string attr, string str)
        {
            this.Add("\n<TD " + attr + " >" + str + "</TD>");
        }
        public void AddTD(string attr, int val)
        {
            this.AddTD(attr, val.ToString());
        }
        public void AddTD(string attr, decimal val)
        {
            this.AddTD(attr, val.ToString());
        }

        public void AddTH(string str)
        {
            this.Add("\n<TH >" + str + "</TH>");
        }
        public void AddTH(string attr, string str)
        {
            this.Add("\n<TH class='" + attr + "' >" + str + "</TH>");
        }

        #region AddMsg
        public void AddMsgOfWarning(string title, string doc)
        {
            this.AddFieldSetYellow("<font color=red><b>" + title + "</b></font>");
            this.Add(doc.Replace("@", "<BR>@"));
            this.AddFieldSetEnd();
        }
        public void AddMsgOfInfo(string title, string doc)
        {
            this.AddFieldSet(title);
            if (doc != null)
                this.Add(doc.Replace("@", "<BR>@"));
            this.AddFieldSetEnd();
        }
        public void AddMsgOfInfoV2(string title, string doc)
        {
            this.AddTable("width='100%' align=left");
            this.AddCaptionMsg(title);
            this.AddTR();
            this.AddTDBigDoc("<div style='margin-left:30px;margin-top:20px;font-size:14px;' ><img src='" + SystemConfig.CCFlowWebPath + "WF/Img/info.png' align='middle' />" + doc + "</div>");
            this.AddTREnd();
            this.AddTableEnd();
        }
        public void AddMsgOfInfoV2LongTitle(string title, string doc)
        {
            this.AddTable("width='100%' align=left");
            this.AddCaptionMsgLong(title);
            this.AddTR();
            this.AddTDBigDoc("<div style='margin-left:30px;margin-top:20px;font-size:14px;' ><img src='" + SystemConfig.CCFlowWebPath + "WF/Img/info.png' align='middle' />" + doc + "</div>");
            this.AddTREnd();
            this.AddTableEnd();
        }
        #endregion

        #region EasyUi样式增加
        /// <summary>
        /// 增加EasyUi linkbutton
        /// </summary>
        /// <param name="text">按钮上的文本</param>
        /// <param name="url">按钮点击要转向的URL</param>
        /// <param name="iconCls">按钮上的图标样式，必须是定义的icon class，如:icon-save</param>
        /// <param name="isPlain">按钮是否是plain样式</param>
        /// <param name="target">链接打开方式，默认本窗口打开</param>
        /// <param name="disabled">是否禁用</param>
        public void AddEasyUiLinkButton(string text, string url, string iconCls = null, bool isPlain = false, string target = "_self", bool disabled = false)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = "javascript:void(0)";

            this.Add(
                string.Format(
                    "<a class='easyui-linkbutton' href='{0}' data-options=\"plain:{1},iconCls:'{2}',disabled:{5}\" target='{4}'>{3}</a>&nbsp;",
                    url.Replace("'", "\""), isPlain.ToString().ToLower(), iconCls ?? string.Empty, text, disabled ? "_self" : target, disabled.ToString().ToLower()));
        }
        #endregion

        #region 操作方法
        /// <summary>
        /// showmodaldialog
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        protected void ShowModalDialog(string url, string title, int Height, int Width)
        {
            string script = "<script language='JavaScript'> window.showModalDialog('" + url + "','','dialogHeight: " + Height.ToString() + "px; dialogWidth: " + Width.ToString() + "px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); </script> ";
            this.Response.Write(script);
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void WinClose()
        {
            this.Response.Write("<script language='JavaScript'> window.close()</script>");
        }

        protected void WinClose(string val)
        {
            string clientscript = "<script language='javascript'> window.returnValue = '" + val + "'; window.close(); </script>";
            this.Page.Response.Write(clientscript);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void WinCloseWithMsg(string msg)
        {
            this.Response.Write("<script language='JavaScript'>alert('" + msg + "'); window.close()</script>");
        }

        /// <summary>
        /// 打开一个新的窗口
        /// </summary>
        /// <param name="msg"></param>
        protected void WinOpen(string url)
        {
            this.WinOpen(url, "", "msg", 900, 500);
        }
        protected void WinOpen(string url, string title, string winName, int width, int height)
        {
            this.WinOpen(url, title, winName, width, height, 0, 0);
        }
        protected void WinOpen(string url, string title, int width, int height)
        {
            this.WinOpen(url, title, "ActivePage", width, height, 0, 0);
        }
        protected void WinOpen(string url, string title, string winName, int width, int height, int top, int left)
        {
            url = url.Replace("<", "[");
            url = url.Replace(">", "]");
            url = url.Trim();
            title = title.Replace("<", "[");
            title = title.Replace(">", "]");
            title = title.Replace("\"", "‘");
            this.Response.Write("<script language='JavaScript'> var newWindow =window.open(' " + url + "','" + winName + "','width=" + width + ",top=" + top + ",left=" + left + ",height=" + height + ",scrollbars=yes,resizable=yes') ; newWindow.focus(); </script> ");
        }
        private int MsgFontSize = 1;
        /// <summary>
        /// 输出到页面上红色的警告。
        /// </summary>
        /// <param name="msg">消息</param>
        protected void ResponseWriteRedMsg(string msg)
        {
            //this.Response.Write("<BR><font color='red' size='"+MsgFontSize.ToString()+"' > <b>"+msg+"</b></font>");
            //if (msg.Length < 200)
            //	return ;
            msg = msg.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = msg;
            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/ErrorPage.aspx";
            this.WinOpen(url, "警告", "msg", 500, 400, 150, 270);
        }
        protected void ResponseWriteRedMsg(Exception ex)
        {
            this.ResponseWriteRedMsg(ex.Message);
        }
        /// <summary>
        /// 输出到页面上蓝色的信息。
        /// </summary>
        /// <param name="msg">消息</param>
        protected void ResponseWriteBlueMsg(string msg)
        {
            this.Response.Write("<BR><font color='Blue' size='" + MsgFontSize.ToString() + "' ><b>" + msg + "</b></font>");

            msg = msg.Replace("@", "<BR>@");
            System.Web.HttpContext.Current.Session["info"] = msg;
            string url = SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx";
            this.WinOpen(url, "错误信息", "msg", 500, 400, 150, 270);
        }
        /// <summary>
        /// 保存成功
        /// </summary>
        protected void ResponseWriteBlueMsg_SaveOK()
        {
            ResponseWriteBlueMsg("保存成功!");
        }
        /// <summary>
        /// ResponseWriteBlueMsg_DeleteOK
        /// </summary>
        protected void ResponseWriteBlueMsg_DeleteOK()
        {
            ResponseWriteBlueMsg("删除成功!");
        }
        /// <summary>
        /// ResponseWriteBlueMsg_UpdataOK
        /// </summary>
        protected void ResponseWriteBlueMsg_UpdataOK()
        {
            ResponseWriteBlueMsg("更新成功!");
        }
        /// <summary>
        /// 输出到页面上黑色的信息。
        /// </summary>
        /// <param name="msg">消息</param>
        protected void ResponseWriteBlackMsg(string msg)
        {
            this.Response.Write("<font color='Black' size=5 ><b>" + msg + "</b></font>");
        }
        protected void ToSignInPage()
        {

            System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.ApplicationPath + "SignIn.aspx?url=/Wel.aspx");
        }
        protected void ToWelPage()
        {
            System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.ApplicationPath + "Wel.aspx");
        }
        /// <summary>
        /// 切换到信息也面。
        /// </summary>
        /// <param name="mess"></param>
        protected void ToErrorPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/InfoPage.aspx");
        }
        /// <summary>
        /// 切换到信息。
        /// </summary>
        /// <param name="mess"></param>
        protected void ToMsgPage(string mess)
        {
            System.Web.HttpContext.Current.Session["info"] = mess;
            System.Web.HttpContext.Current.Response.Redirect(SystemConfig.CCFlowWebPath + "WF/Comm/Port/MsgPage.aspx");
        }
        #endregion

        #region AddHyperLink
        public void AddHyperLink(string text, string url, string target, string imgs)
        {
            BPHyperLink hl = new BPHyperLink();
            hl.Text = text;
            hl.NavigateUrl = url;
            hl.Target = target;
            hl.ImageUrl = imgs;
            this.Controls.Add(hl);
        }
        public void AddHyperLink(string text, string url, string target)
        {
            this.AddHyperLink(text, url, target, "");
        }
        public void AddHyperLink(string text, string url)
        {
            this.AddHyperLink(text, url, "", "");
        }
        #endregion

        protected override void OnInit(EventArgs e)
        {
            //ShowRuning();
            base.OnInit(e);
        }
        private void Page_Load(object sender, System.EventArgs e)
        {
            //CreateControl();
        }
        /// <summary>
        /// 清空字体。
        /// </summary>
        public void Clear()
        {
            this._Text = null;
            this.Controls.Clear();
        }
        /// <summary>
        /// _Text
        /// </summary>
        protected string _Text = String.Empty;
        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get
            {
                return this._Text;

            }
            set
            {
                _Text = value;
            }
        }

        #region bind entity
        public void BindEntity3ItemReadonly(Entity en, bool isShowDtl)
        {
            Map map = en.EnMap;
            AttrDescs ads = new AttrDescs(en.ToString());

            this.Add("<table border=0 >");
            foreach (AttrDesc ad in ads)
            {
                Attr attr = map.GetAttrByKey(ad.Attr);
                this.AddTR();
                this.AddTD("valign=top  align=right ", attr.Desc + "：");
                switch (attr.MyDataType)
                {
                    case DataType.AppString:
                        if (attr.UIHeight != 0)
                            this.AddTD("valign=top ", en.GetValHtmlStringByKey(ad.Attr));
                        else
                            this.AddTD("valign=top ", en.GetValStringByKey(ad.Attr));
                        break;
                    case DataType.AppDateTime:
                    case DataType.AppDate:
                        this.AddTD("valign=top ", en.GetValStringByKey(ad.Attr));
                        break;
                    case DataType.AppBoolean:
                        this.AddTD("valign=top ", en.GetValBoolStrByKey(ad.Attr));
                        break;
                    case DataType.AppFloat:
                    case DataType.AppInt:
                        this.AddTD("valign=top class='TDNum'", en.GetValStringByKey(ad.Attr));
                        break;
                    case DataType.AppMoney:
                        this.AddTD("valign=top class='TDNum'", en.GetValDecimalByKey(ad.Attr).ToString("0.00"));
                        break;
                    case DataType.AppDouble:
                    case DataType.AppRate:
                        this.AddTD("valign=top class='TDNum'", en.GetValStringByKey(ad.Attr));
                        break;
                    default:
                        break;
                }
                this.AddTD("valign=right  ", ad.Desc);
                this.AddTREnd();
            }
            this.AddTableEnd();
        }
        #endregion


        #region Web 窗体设计器生成的代码

        /// <summary>
        ///		设计器支持所需的方法 - 不要使用代码编辑器
        ///		修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion
    }
}
