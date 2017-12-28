using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.IO;
using System.Drawing;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.WF.Template;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace CCFlow.WF.UC
{
    public partial class ToolWap : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "设置";
            if (WebUser.IsWap == true && this.RefNo == null)
            {
                this.BindTools();
                return;
            }
            switch (this.RefNo)
            {
                case "AthFlows":
                    this.AthFlows();
                    break;
                case "Skin":
                    this.Skin();
                    break;
                case "MyWorks":
                    this.MyWorks();
                    break;
                case "Siganture":
                    this.Siganture();
                    break;
                case "AdminSet":
                    AdminSet();
                    break;
                case "AutoLog":
                    BindAutoLog();
                    break;
                case "Pass":
                    BindPass();
                    break;
                case "Profile":
                    BindProfile();
                    break;
                case "Auto":
                    BindAuto();
                    break;
                case "AutoDtl":
                    BindAutoDtl();
                    break;
                case "Times": // 时效分析
                    BindTimes();
                    break;
                case "FtpSet": // 时效分析
                    BindFtpSet();
                    break;
                case "PerPng":
                    BingPerPng();
                    break;
                case "BitmapCutter":
                    this.BitmapCutter();
                    break;
                case "Per":
                default:
                    BindPer();
                    break;
            }
        }
        public void AthFlows()
        {
            FlowSorts sorts = new FlowSorts();
            sorts.RetrieveAll();
            Flows fls = new Flows();
            fls.RetrieveAll();

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);

            this.AddTable();
            this.AddCaptionLeft("授权流程范围");
            this.AddTR();
            this.AddTDTitle("IDX");
            this.AddTDTitle("类别");
            this.AddTDTitle("流程");
            this.AddTREnd();
            int i = 0;
            foreach (FlowSort sort in sorts)
            {
                i++;
                this.AddTRSum();
                this.AddTDIdx(i);
                this.AddTDB(sort.Name);
                CheckBox cbAll = new CheckBox();
                cbAll.Text = "选择类别下全部";
                cbAll.ID = "CB_d" + sort.No;
                this.AddTD(cbAll);
                this.AddTREnd();

                string ctlIDs = "";
                foreach (Flow fl in fls)
                {
                    if (fl.FK_FlowSort != sort.No)
                        continue;

                    i++;
                    this.AddTR();
                    this.AddTDIdx(i);
                    this.AddTD("");
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + fl.No;
                    cb.Text = fl.Name;
                    if (emp.AuthorFlows.Contains(fl.No))
                        cb.Checked = true;
                    ctlIDs += cb.ID + ",";
                    this.AddTD(cb);
                    this.AddTREnd();
                }
                cbAll.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";
            }

            this.AddTR();
            this.AddTDTitle("");
            Button btnSaveAthFlows = new Button();
            btnSaveAthFlows.CssClass = "Btn";
            btnSaveAthFlows.ID = "Btn_Save";
            btnSaveAthFlows.Text = "Save";
            btnSaveAthFlows.Click += new EventHandler(btnSaveAthFlows_Click);
            this.Add(btnSaveAthFlows);
            this.AddTD("colspan=2", btnSaveAthFlows);
            this.AddTREnd();
            this.AddTableEnd();
        }
        void btnSaveAthFlows_Click(object sender, EventArgs e)
        {
            Flows fls = new Flows();
            fls.RetrieveAll();
            string strs = "";
            foreach (Flow fl in fls)
            {
                CheckBox check = this.GetCBByID("CB_" + fl.No);

                if (check == null)
                    continue;

                if (check.Checked == false)
                    continue;
                strs += "," + fl.No;
            }

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            emp.AuthorFlows = strs;
            emp.Update();

            BP.Sys.Glo.WriteUserLog("Auth", WebUser.No, "授权:" + strs);
            this.WinCloseWithMsg("保存成功.");
        }
        public void MyWorks()
        {
            Flows fls = new Flows();
            fls.RetrieveAll();

            Nodes nds = new Nodes();
            nds.RetrieveAll();

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("IDX");
            this.AddTDTitle("流程");
            this.AddTDTitle("节点");
            this.AddTDTitle("查询");
            this.AddTREnd();

            int idx = 0;
            foreach (Flow fl in fls)
            {
                Nodes mynds = new Nodes();
                foreach (BP.WF.Node nd in nds)
                {
                    if (nd.FK_Flow != fl.No)
                        continue;

                    if (nd.NodeEmps.Contains(WebUser.No))
                        mynds.AddEntity(nd);
                }

                if (mynds.Count == 0)
                    continue;


                bool isFirst = true;
                foreach (BP.WF.Node mynd in mynds)
                {
                    if (isFirst)
                        this.AddTRSum();
                    else
                        this.AddTR();

                    this.AddTDIdx(idx);
                    if (isFirst)
                        this.AddTD(mynd.FlowName);
                    else
                        this.AddTD();

                    this.AddTD(mynd.Name);

                    this.AddTD("<a href=\"javascript:WinOpen('FlowSearchSmallSingle.aspx?FK_Node=" + mynd.NodeID + "');\">工作查询</a>");
                    this.AddTREnd();

                    idx++;
                    isFirst = false;
                }
            }
            this.AddTableEnd();
        }
        public void BindTools()
        {
            BP.WF.XML.Tools tools = new BP.WF.XML.Tools();
            tools.RetrieveAll();

            this.AddFieldSet("<a href='Home.aspx'><img src='../Img/Home.gif' border=0/>Home</a>");
            this.AddUL();
            foreach (BP.WF.XML.Tool tool in tools)
            {
                this.AddLi("" + this.PageID + ".aspx?RefNo=" + tool.No, tool.Name, "_self");
            }
            this.AddULEnd();
            this.AddFieldSetEnd();
        }
        public void Skin()
        {
            string pageID = this.PageID;
            string setNo = this.Request.QueryString["SetNo"];
            if (setNo != null)
            {
                BP.WF.Port.WFEmp em = new BP.WF.Port.WFEmp(BP.Web.WebUser.No);
                em.Style = setNo;
                em.Update();
                WebUser.Style = setNo;
                this.Response.Redirect(pageID + ".aspx?RefNo=Skin", true);
                return;
            }

            this.AddFieldSet("风格设置");

            BP.WF.XML.Skins sks = new BP.WF.XML.Skins();
            sks.RetrieveAll();

            this.AddUL();
            foreach (BP.WF.XML.Skin item in sks)
            {

                // modified by ZhouYue 2013-05-20 ignore 'if (WebUser.Style == item.No)' case.
                //if (WebUser.Style == item.No)
                //    this.AddLi(item.Name + "&nbsp;&nbsp;<span style='background:" + item.CSS + "' ><i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</i></span>");
                //else
                this.AddLi(pageID + ".aspx?RefNo=Skin&SetNo=" + item.No, item.Name + "&nbsp;&nbsp;<span style='background:" + item.CSS + "' ><i>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</i></span>");

                //System.Web.UI.WebControls.RadioButton rb = new RadioButton();
                //rb.ID = "RB_" + item.No;
                //rb.Text = item.Name;
                //rb.GroupName = "s";
                //if (WebUser.Style == item.No)
                //    rb.Checked=true;
                //this.Add(rb);
                //this.AddBR();
            }
            this.AddULEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_SaveSkin_Click);
            this.AddFieldSetEnd(); // ("风格设置");
        }

        void btn_SaveSkin_Click(object sender, EventArgs e)
        {
            BP.WF.XML.Skins sks = new BP.WF.XML.Skins();
            sks.RetrieveAll();
            foreach (BP.WF.XML.Skin item in sks)
            {
                if (this.GetRadioButtonByID("RB_" + item.No).Checked)
                {
                    WebUser.Style = item.No;
                    BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
                    emp.Style = item.No;
                    emp.Update();
                    this.Response.Redirect(this.Request.RawUrl, true);
                    return;
                }
            }
        }

        public void BindFtpSet()
        {
            this.AddFieldSet("ftp setting");

            this.AddTable();
            this.AddTR();
            this.AddTDTitle();
            this.AddTDTitle();
            this.AddTDTitle();
            this.AddTREnd();

            this.AddTR();
            this.AddTD("用户名");
            TextBox tb = new TextBox();

            tb.ID = "TB_UserNo";
            this.AddTD(tb);
            this.AddTD();
            this.AddTREnd();

            this.AddTR();
            this.AddTD("密码");
            tb = new TextBox();
            tb.TextMode = TextBoxMode.Password;
            tb.ID = "TB_Pass1";
            this.AddTD(tb);
            this.AddTD();
            this.AddTREnd();

            //this.AddTR();
            //this.AddTD("重输新密码");
            //tb = new TextBox();
            //tb.TextMode = TextBoxMode.Password;
            //tb.ID = "TB_Pass3";
            //this.AddTD(tb);
            //this.AddTD();
            //this.AddTREnd();


            this.AddTR();
            this.AddTD("");

            Btn btn = new Btn();
            btn.Text = "确定";
            btn.Click += new EventHandler(btn_Click);
            this.AddTD(btn);
            this.AddTD();
            this.AddTREnd();
            this.AddTableEnd();
            this.AddFieldSetEnd();
        }
        public void Siganture()
        {
            string path = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\T.JPG";
            if (this.DoType != null || System.IO.File.Exists(path) == false)
            {
                string pathMe = BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + WebUser.No + ".JPG";
                File.Copy(BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\Templete.JPG",
                    path, true);

                string fontName = "宋体";
                switch (this.DoType)
                {
                    case "ST":
                        fontName = "宋体";
                        break;
                    case "LS":
                        fontName = "隶书";
                        break;
                    default:
                        break;
                }

                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                Font font = new Font(fontName, 15);
                Graphics g = Graphics.FromImage(img);
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat(StringFormatFlags.DirectionVertical);//文本
                g.DrawString(WebUser.Name, font, drawBrush, 3, 3);
                try
                {
                    File.Delete(pathMe);
                }
                catch
                {
                }
                img.Save(pathMe);
                img.Dispose();
                g.Dispose();

                File.Copy(pathMe, BP.Sys.SystemConfig.PathOfDataUser + "\\Siganture\\" + WebUser.Name + ".JPG", true);
            }

            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='../Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-电子签名设置" + WebUser.Auth);
            else
                this.AddFieldSet("电子签名设置" + WebUser.Auth);

            // this.AddFieldSet("电子签名设置");

            this.Add("<p align=center><img src='../DataUser/Siganture/" + WebUser.No + ".jpg' style='wdith:120px;height:30px;' border=1 onerror=\"this.src='../DataUser/Siganture/UnName.jpg'\"/> </p>");

            this.Add("上传");

            System.Web.UI.WebControls.FileUpload fu = new System.Web.UI.WebControls.FileUpload();
            fu.ID = "F";
            this.Add(fu);

            Btn btn = new Btn();
            btn.Text = "确定";
            btn.Click += new EventHandler(btn_Siganture_Click);
            this.Add(btn);

            this.AddHR();

            this.AddB("利用扫描仪设置步骤:");
            this.AddUL();
            this.AddLi("在白纸上写下您的签名");
            this.AddLi("送入扫描仪扫描，并得到jpg文件。");
            this.AddLi("利用图片处理工具把他们处理缩小到 90*30像素大小。");
            this.AddULEnd();

            this.AddB("手写设置:");
            this.AddUL();
            this.AddLi("启动画板程序，写下您的签名。");
            this.AddLi("保存成.jpg文件，设置文件为90*30像素大小。");
            this.AddULEnd();

            this.AddB("让系统自动为您创建（请选择字体）:");
            this.AddUL();
            this.AddLi("<a href='" + this.PageID + ".aspx?RefNo=Siganture&DoType=ST'>宋体</a>");
            this.AddLi("<a href='" + this.PageID + ".aspx?RefNo=Siganture&DoType=LS'>隶书</a>");
            this.AddULEnd();

            this.AddFieldSetEnd();
        }

        void btn_Siganture_Click(object sender, EventArgs e)
        {
            FileUpload f = (FileUpload)this.FindControl("F");
            if (f.HasFile == false)
                return;

            //if (f.FileName.EndsW

            //判断文件类型.
            string fileExt = ",bpm,jpg,jpeg,png,gif,";
            string ext = f.FileName.Substring(f.FileName.LastIndexOf('.')+1).ToLower();
            if (fileExt.IndexOf(ext + ",") == -1)
            {
                Alert("err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext);
                return;
            }

            try
            {
                string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/T" + WebUser.No + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                f.SaveAs(tempFile);
                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
                return;
            }

            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.No + ".jpg");
            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + WebUser.Name + ".jpg");

            f.PostedFile.InputStream.Close();
            f.PostedFile.InputStream.Dispose();
            f.Dispose();

            this.Response.Redirect(this.Request.RawUrl, true);
            //this.Alert("保存成功。");
        }
        public void AdminSet()
        {
            this.AddFieldSet("系统设置");

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("项目");
            this.AddTDTitle("项目值");
            this.AddTDTitle("描述");
            this.AddTREnd();


            this.AddTR();
            this.AddTD("标题图片");
            FileUpload fu = new FileUpload();
            fu.ID = "F";
            this.AddTD(fu);
            this.AddTD("系统顶部的标题图片");
            //   this.AddTDBigDoc("请您自己调整好图片大小，然后把它上传上去。在系统设置里可以控制标题图片是否显示。");
            this.AddTREnd();

            this.AddTR();
            this.AddTD("ftp URL");
            TextBox tb = new TextBox();
            tb.Width = 200;
            tb.ID = "TB_FtpUrl";
            this.AddTD(tb);
            this.AddTD();
            this.AddTREnd();

            this.AddTR();
            this.AddTD("");

            Btn btn = new Btn();
            btn.Text = " OK ";
            btn.Click += new EventHandler(btn_AdminSet_Click);
            this.AddTD(btn);
            this.AddTD();
            this.AddTREnd();

            this.AddTR();
            this.AddTD();
            this.AddTD("<a href=\"javascript:WinOpen('../Comm/Sys/EditWebConfig.aspx')\" >System Setting</a>-<a href=\"javascript:WinOpen('../OA/FtpSet.aspx')\" >FTP Services</a>-<a href=\"javascript:WinOpen('/WF/Comm/Ens.aspx?EnsName=BP.OA.Links')\" >Link</a>");
            this.AddTD("");
            //  this.AddTD("<a href=\"javascript:WinOpen('./../WF/ClearDatabase.aspx')\" >" + this.ToE("ClearDB", "清除流程数据") + "</a>");

            this.AddTD();
            this.AddTREnd();

            this.AddTableEnd();

            this.AddFieldSetEnd();
        }
        void btn_AdminSet_Click(object sender, EventArgs e)
        {
            FileUpload f = (FileUpload)this.FindControl("F");
            if (f.HasFile == false)
                return;

            f.SaveAs(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Title.gif");
            this.Response.Redirect(this.Request.RawUrl, true);
            //this.Alert("保存成功。");
        }
        public void BindPass()
        {
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='/WF/Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-密码修改");
            else
                this.AddFieldSet("密码修改");

            this.AddBR();

            this.Add("<table border=0 width=80% align=center > ");
            this.AddTR();
            this.AddTDTitle();
            this.AddTDTitle();
            this.AddTREnd();

            this.AddTR();
            this.AddTD("旧密码：");
            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.Password;
            tb.ID = "TB_Pass1";
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("新密码：");
            tb = new TextBox();
            tb.TextMode = TextBoxMode.Password;
            tb.ID = "TB_Pass2";
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("重输新密码：");
            tb = new TextBox();
            tb.TextMode = TextBoxMode.Password;
            tb.ID = "TB_Pass3";
            this.AddTD(tb);
            this.AddTREnd();


            this.AddTR();
            this.AddTD("");





            Btn btn = new Btn();
            btn.Text = "确定";
            btn.Click += new EventHandler(btn_Click);
            this.AddTD(btn);
            this.AddTREnd();
            this.AddTableEnd();

            this.AddBR();
            this.AddFieldSetEnd();

        }
        public void BindProfile()
        {
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='/WF/Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-" + "基本信息" + WebUser.Auth);
            else
                this.AddFieldSet("基本信息" + WebUser.Auth);

            this.Add("<br><table border=0 width='80%' align=center >");
            this.AddTR();
            this.AddTD("手机");
            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.SingleLine;
            tb.ID = "TB_Tel";
            tb.Text = emp.Tel;
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("Email");
            tb = new TextBox();
            tb.TextMode = TextBoxMode.SingleLine;
            tb.ID = "TB_Email";
            tb.Text = emp.Email;
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("QQ/RTX/MSN");
            tb = new TextBox();
            tb.TextMode = TextBoxMode.SingleLine;
            tb.ID = "TB_TM";
            tb.Text = emp.Email;
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("信息接收方式");
            DDL ddl = new DDL();
            ddl.ID = "DDL_Way";
            ddl.BindSysEnum("AlertWay");
            //ddl.Items.Add(new ListItem("不接收", "0"));
            //ddl.Items.Add(new ListItem("手机短信", "1"));
            //ddl.Items.Add(new ListItem("邮件", "2"));
            //ddl.Items.Add(new ListItem("手机短信+邮件", "3"));
            ddl.SetSelectItem((int)emp.HisAlertWay);
            this.AddTD(ddl);
            this.AddTREnd();

            this.AddTR();
            Btn btn = new Btn();
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Profile_Click);
            this.AddTD("colspan=2 align=center", btn);
            this.AddTREnd();
            this.AddTableEnd();
            this.AddBR();
            this.AddFieldSetEnd();
        }
        void btn_Profile_Click(object sender, EventArgs e)
        {
            string tel = this.GetTextBoxByID("TB_Tel").Text;
            string mail = this.GetTextBoxByID("TB_Email").Text;
            int way = this.GetDDLByID("DDL_Way").SelectedItemIntVal;
 
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            emp.Tel = tel;
            emp.Email = mail;
            emp.HisAlertWay = (BP.WF.Port.AlertWay)way;

            try
            {
                emp.Update();
                this.Alert("设置生效，谢谢使用。");
            }
            catch (Exception ex)
            {
                emp.CheckPhysicsTable();
                this.Alert("设置错误：" + ex.Message);
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            string p1 = this.GetTextBoxByID("TB_Pass1").Text;
            string p2 = this.GetTextBoxByID("TB_Pass2").Text;
            string p3 = this.GetTextBoxByID("TB_Pass3").Text;




            if (p2.Length == 0 || p1.Length == 0)
            {
                this.Alert("密码不能为空");
                return;
            }

            if (p2 != p3)
            {
                this.Alert("两次密码不一致。");
                return;
            }


            Emp emp = new Emp(WebUser.No);
            string appNo = BP.Sys.SystemConfig.AppSettings["CCPortal.AppNo"];
            //OA系统需要加密
            if ((!string.IsNullOrEmpty(appNo) && appNo == "CCOA") || SystemConfig.IsEnablePasswordEncryption)
            {
                p1 = BP.Tools.Cryptography.EncryptString(p1);
                p2 = BP.Tools.Cryptography.EncryptString(p2);
            }
            if (emp.Pass == p1)
            {
                emp.Pass = p2;
                emp.Update();
                this.Alert("密码修改成功，请牢记新密码。");
            }
            else
            {
                this.Alert("老密码错误，不允许您修改它。");
            }
        }
        /// <summary>
        /// 时效分析
        /// </summary>
        public void BindTimes()
        {
            if (this.Request.QueryString["FK_Node"] != null)
            {
                this.BindTimesND();
                return;
            }
            if (this.Request.QueryString["FK_Flow"] != null)
            {
                this.BindTimesFlow();
                return;
            }

            FlowSorts sorts = new FlowSorts();
            sorts.RetrieveAll();

            Flows fls = new Flows();
            fls.RetrieveAll();

            Nodes nds = new Nodes();
            nds.RetrieveAll();

            this.AddTable();

         


            foreach (FlowSort sort in sorts)
            {
                this.AddTRSum();
                this.AddTDB(sort.Name);
                this.AddTD("");
                this.AddTD();
                this.AddTD();
                this.AddTD();
                this.AddTD();

                this.AddTREnd();

                foreach (Flow fl in fls)
                {
                    if (sort.No != fl.FK_FlowSort)
                        continue;

                    this.AddTRSum();
                    this.AddTD();
                    this.AddTDB(fl.Name);
                    //  this.AddTD("<a href='"+this.PageID+".aspx?DoType=Times&FK_Flow=" + fl.No + "'>分析</a>");
                    this.AddTD("工作数");
                    this.AddTD("平均天" + fl.AvgDay.ToString("0.00"));

                    this.AddTD("我参与的工作数");
                    this.AddTD("工作总数");

                    this.AddTREnd();

                    decimal avgDay = 0;
                    foreach (BP.WF.Node nd in nds)
                    {
                        if (nd.FK_Flow != fl.No)
                            continue;

                        this.AddTR();
                        this.AddTD();
                        this.AddTD(nd.Name);
                        //  this.AddTD("<a href='"+this.PageID+".aspx?DoType=Times&FK_Node=" + nd.NodeID + "'>分析</a>");
                        string sql = "";

                        sql = "SELECT  COUNT(*) FROM ND" + nd.NodeID;

                        try
                        {
                            int num = DBAccess.RunSQLReturnValInt(sql);
                            this.AddTD(num);
                        }
                        catch
                        {
                            nd.CheckPhysicsTable();
                            this.AddTD("无效");
                        }

                        sql = "SELECT AVG( DateDiff(d, cast(RDT as datetime),  cast(CDT as datetime) ) ) FROM ND" + nd.NodeID;
                        try
                        {
                            decimal day = DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                            avgDay += day;
                            this.AddTD(day.ToString("0.00"));
                        }
                        catch
                        {
                            nd.CheckPhysicsTable();
                            this.AddTD("无效");
                        }

                        // day = DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                        //this.AddTD(DBAccess.RunSQLReturnValInt(""));
                        this.AddTD("无效");

                        this.AddTREnd();
                    }

                    if (avgDay != fl.AvgDay)
                    {
                        fl.AvgDay = avgDay;
                        fl.Update();
                    }
                }
            }
            this.AddTableEnd();
        }
        public void BindTimesFlow()
        {

        }
        public void BindTimesND()
        {
            int nodeid = int.Parse(this.Request.QueryString["FK_Node"]);
            BP.WF.Node nd = new BP.WF.Node(nodeid);
            this.AddTable();
            this.AddCaptionLeft("<a href='" + this.PageID + ".aspx?DoType=Times&FK_Flow=" + nd.FK_Flow + "'>" + nd.FlowName + "</a> => " + nd.Name);
            this.AddTR();
            this.AddTDTitle("IDX");
            this.AddTDTitle("人员");
            this.AddTDTitle("Average time");
            this.AddTDTitle("Participation times");
            this.AddTREnd();
            this.AddTableEnd();
        }
        public void BindAutoLog()
        {
            string sql = "";

            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    sql = "SELECT a.No || a.Name as Empstr,AuthorDate, a.No,AuthorToDate FROM WF_Emp a WHERE Author='" + WebUser.No + "' AND AuthorWay >= 1";
                    break;
                default:
                    sql = "SELECT a.No + a.Name as Empstr,AuthorDate, a.No ,AuthorToDate FROM WF_Emp a WHERE Author='" + WebUser.No + "' AND AuthorWay >= 1";
                    break;
            }

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                if (WebUser.IsWap)
                {

                    this.AddFieldSet("<a href=Home.aspx ><img src='/WF/Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-密码修改");
                    this.AddBR();
                    this.AddMsgGreen("提示", "没有同事授权给您，您不能使用授权方式登陆。");
                    this.AddFieldSetEnd();
                }
                else
                {
                    this.AddMsgOfInfoV2LongTitle("提示","没有同事授权给您，您不能使用授权方式登陆。");
                }
                return;
            }

            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='/WF/Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-下列同事授权给您");
            else
                this.AddFieldSet("下列同事授权给您");


            this.Add("<ul>");
            foreach (DataRow dr in dt.Rows)
            {
                this.AddLi("<a href=\"javascript:LogAs('" + dr[2] + "')\">" + "授权人" + ":" + dr["Empstr"] + "</a> - 授权日期:" + dr["AuthorDate"] + "，有效日期：" + dr["AuthorToDate"]);
            }
            this.Add("</ul>");
            this.AddFieldSetEnd();
        }
        public void BindAuto()
        {
            string sql = "SELECT a.No,a.Name,b.Name as DeptName FROM Port_Emp a, Port_Dept b WHERE a.FK_Dept=b.No ORDER  BY a.FK_Dept ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='/WF/Img/Home.gif' border=0 >Home</a>-<a href='" + this.PageID + ".aspx'>设置</a>-请选择您要授权的人员");
            else
                this.AddFieldSet("请选择您要授权的人员");

            string deptName = null;
            this.AddBR();
            this.Add(" <table width='80%' align=center border=1 > ");
            this.AddTR();
            this.AddTDTitle("编号");
            this.AddTDTitle("部门");
            this.AddTDTitle("要执行授权的人员");
            this.AddTREnd();

            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string fk_emp = dr["No"].ToString();
                if (fk_emp == "admin" || fk_emp == WebUser.No)
                    continue;
                idx++;
                if (dr["DeptName"].ToString() != deptName)
                {
                    deptName = dr["DeptName"].ToString();
                    this.AddTRSum();
                    this.AddTDIdx(idx);
                    this.AddTD(deptName);
                }
                else
                {
                    this.AddTR();
                    this.AddTDIdx(idx);
                    this.AddTD();
                }

                string str = BP.WF.Glo.DealUserInfoShowModel(fk_emp, dr["Name"].ToString());
                //this.AddTD("<a href=\"" + this.PageID + ".aspx?RefNo=AutoDtl&FK_Emp=" + fk_emp + "\" >" + str + "</a>");
                this.AddTD("<a href=\"" + this.PageID + ".aspx?RefNo=AutoDtl&FK_Emp=" + fk_emp + "\" >" + str + "</a>");
                this.AddTREnd();
            }
            this.AddTableEnd();
            this.AddBR();
            this.AddFieldSetEnd();
        }
        /// <summary>
        /// 授权明细
        /// </summary>
        public void BindAutoDtl()
        {
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='/WF/Img/Home.gif' border=0 >Home</a>-<a href='" + this.PageID + ".aspx'>设置</a>-授权详细信息");
            else
                this.AddFieldSet("授权详细信息");

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            BP.WF.Port.WFEmp empAu = new BP.WF.Port.WFEmp(this.Request["FK_Emp"]);

            this.AddBR();
            this.AddTable();
            this.AddTR();
            this.AddTDTitle("项目");
            this.AddTDTitle("内容");
            this.AddTREnd();

            this.AddTR();
            this.AddTD("授权给:");
            this.AddTD(empAu.No + "    " + empAu.Name);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("收回授权日期:");
            TB tb = new TB();
            tb.ID = "TB_DT";
            System.DateTime dtNow = System.DateTime.Now;
            dtNow = dtNow.AddDays(14);
            tb.Text = dtNow.ToString(DataType.SysDataTimeFormat);
            tb.ShowType = TBType.DateTime;
            tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
            this.AddTD(tb);
            this.AddTREnd();

            this.AddTR();
            this.AddTD("授权方式:");
            DDL ddl = new DDL();
            ddl.ID = "DDL_AuthorWay";
            ddl.BindSysEnum(BP.WF.Port.WFEmpAttr.AuthorWay);
            ddl.SetSelectItem(emp.AuthorWay);
            this.AddTD(ddl);
            this.AddTREnd();

            Button btnSaveIt = new Button();
            btnSaveIt.ID = "Btn_Save";
            btnSaveIt.CssClass = "Btn";
            btnSaveIt.Text = "保存";
            btnSaveIt.Click += new EventHandler(btnSaveIt_Click);
            this.AddTR();
            this.AddTD("colspan=1", "<b><a href=\"javascript:WinShowModalDialog('ToolsSet.aspx?RefNo=AthFlows&d=" + DateTime.Now.ToString() + "')\" >设置要授权的流程范围</a></b>");
            this.AddTD("colspan=1", btnSaveIt);
            this.AddTREnd();

            this.AddTR();
            this.AddTDBigDoc("colspan=2", "说明:在您确定了收回授权日期后，被授权人不能再以您的身份登陆，<br>如果未到指定的日期您可以取回授权。");
            this.AddTREnd();
            this.AddTableEndWithBR();
            this.AddFieldSetEnd();
        }
        void btnSaveIt_Click(object sender, EventArgs e)
        {
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            emp.AuthorDate = BP.DA.DataType.CurrentData;
            emp.Author = this.Request["FK_Emp"];
            emp.AuthorToDate = this.GetTBByID("TB_DT").Text;
            emp.AuthorWay = this.GetDDLByID("DDL_AuthorWay").SelectedItemIntVal;
            if (emp.AuthorWay == 2 && emp.AuthorFlows.Length < 3)
            {
                this.Alert("您指定授权方式是按指定的流程范围授权，但是您没有指定流程的授权范围.");
                return;
            }
            emp.Update();
            //BP.Sys.UserLog.AddLog("Auth", WebUser.No, "全部授权");
            BP.Sys.Glo.WriteUserLog("Auth", WebUser.No, "全部授权");
            this.Response.Redirect(this.PageID + ".aspx", true);
        }
        public void BindPer()
        {
            if (WebUser.Auth != null)
            {
                this.AddFieldSet("提示");
                this.AddBR();
                this.Add("您的登陆是授权模式，您不能查看个人信息。");
                this.AddUL();
                this.AddLi("<a href=\"javascript:ExitAuth('" + WebUser.Auth + "')\">退出授权模式</a>");
                this.AddLi("<a href=" + this.PageID + ".aspx >设置</a>");
                if (WebUser.IsWap)
                    this.AddLi("<a href='Home.aspx'>返回主页</a>");

                this.AddULEnd();
                this.AddFieldSetEnd();
                return;
            }


            BP.WF.Port.WFEmp au = new BP.WF.Port.WFEmp(WebUser.No);

            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='../Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-" + "基本信息" + WebUser.Auth);
            else
                this.AddFieldSet("基本信息" + WebUser.Auth);

            this.Add("<p class=BigDoc >");

            this.Add("用户帐号:&nbsp;&nbsp;<font color=green>" + WebUser.No + "</font>&nbsp;&nbsp;");
            this.Add("<br>用户名:&nbsp;&nbsp;<font color=green>" + WebUser.Name + "</font>&nbsp;&nbsp;");
            this.AddHR();

            this.AddB("电子签字:<img src='../DataUser/Siganture/" + WebUser.No + ".jpg' border=1 onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"/><a href='" + this.PageID + ".aspx?RefNo=Siganture' >设置/修改</a>。");

            this.AddBR();
            this.AddHR();

            this.Add("主部门 : <font color=green>" + WebUser.FK_DeptName + "</font>");

            this.AddBR();
            this.AddBR();


            // this.Add(au.AuthorIsOK.ToString());
            if (au.AuthorIsOK == false)
                this.Add("授权情况：未授权 - <a href='" + this.PageID + ".aspx?RefNo=Auto' >执行授权</a>。");
            else
            {
                string way = "";
                if (au.AuthorWay == 1)
                    way = "全部授权";
                else
                    way = "指定流程范围授权";
                this.Add("授权情况：授权给：<font color=green>" + au.Author + "</font>，授权日期: <font color=green>" + au.AuthorDate + "</font>，收回授权日期:<font color=green>" + au.AuthorToDate + "</font>。<br>我要:<a href=\"javascript:TakeBack('" + au.Author + "')\" >取消授权</a>；授权方式：<font color=green>" + way + "</font>，<a href=\"" + this.PageID + ".aspx?RefNo=AutoDtl&FK_Emp=" + au.Author + "\">我要修改授权信息</a>。");
            }

            this.Add("&nbsp;我要:<a href='" + this.PageID + ".aspx?RefNo=Pass'>修改密码</a>");

            this.AddBR("<hr><b>信息提示：</b><a href='" + this.PageID + ".aspx?RefNo=Profile'>设置/修改</a>");
            this.Add("<br><br>接受短消息提醒手机号 : <font color=green>" + au.TelHtml + "</font>");
            this.Add("<br><br>接受E-mail提醒 : <font color=green>" + au.EmailHtml + "</font>");
           
            this.AddHR();
            Stations sts = WebUser.HisStations;
            this.AddB("岗位/部门-权限");
            this.AddBR();
            this.AddBR("岗位权限");
            foreach (Station st in sts)
            {
                this.Add(" - <font color=green>" + st.Name + "</font>");
            }

            Depts depts = WebUser.HisDepts;
            this.AddBR();
            this.AddBR();
            this.Add("部门权限");
            foreach (Dept st in depts)
                this.Add(" - <font color=green>" + st.Name + "</font>");

            this.Add("</p>");
            this.AddFieldSetEnd();
        }
        public void BingPerPng()
        {
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='../Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-" + "图标信息");
            else
                this.AddFieldSet("图标信息");

            string picfile = BP.Sys.SystemConfig.PathOfWebApp + "DataUser\\UserIcon";

            this.AddHR();
            this.AddBR();

            this.AddB("Icon设置：<a href='" + this.PageID + ".aspx?RefNo=BitmapCutter' >设置/图标设置</a>");
            this.AddHR();
            this.AddBR();
            bool isNo = false;

            System.Drawing.Image myImage = null;
            int phWidth = 0;
            int phHeight = 0;
            DirectoryInfo di = new DirectoryInfo(picfile);
            if (di.Exists)
            {
                string[] subpic = Directory.GetFiles(picfile);
                if (subpic.Length > 0)
                {
                    foreach (string PicPath in subpic)
                    {
                        string[] tempFilePath = PicPath.Split('\\');

                        if (tempFilePath[tempFilePath.Length - 1].StartsWith(WebUser.No + "BigerCon"))
                        {
                            myImage = System.Drawing.Image.FromFile(picfile + "/" + WebUser.No + "BigerCon.png");
                            phWidth = myImage.Width;
                            phHeight = myImage.Height;

                            if (phWidth > 510)
                            {
                                isNo = false;
                            }
                            else
                            {
                                isNo = true;
                            }
                            myImage.Dispose();
                            break;
                        }
                    }
                }
            }
            if (isNo)
            {
                this.Add("<div id='Container'>");
                this.Add("<div id='Content'>");
                this.Add("<div id='Content-Left'><img src='../DataUser/UserIcon/" + WebUser.No + "BigerCon.png'/></div>");
                this.Add("<div id='Content-Main2'><img src='../DataUser/UserIcon/" + WebUser.No + "Smaller.png' width='40px' height='40px'/></div>");
                this.Add("<div id='Content-Main3'>32*32</div>");
                this.Add("<div id='Content-Main'><img src='../DataUser/UserIcon/" + WebUser.No + ".png' width='60px' height='60px'/></div>");
                this.Add("<div id='Content-Main3'>60*60</div>");
                this.Add("<div id='Content-Main1'><img src='../DataUser/UserIcon/" + WebUser.No + "Biger.png' width='100px' height='100px'/></div>");
                this.Add("<div id='Content-Main3'>100*100</div>");
                this.Add("</div>");
                this.Add("</div>");
            }
            else
            {
                this.Add("<div id='Container'>");
                this.Add("<div id='Content'>");
                this.Add("<div id='Content-Left' style='border: solid 1px #7d9edb;padding: 1px;'></div>");
                this.Add("<div id='Content-Main2'><img src='../DataUser/UserIcon/Default.png' border=1 width='32px' height='32px'/></div>");
                this.Add("<div id='Content-Main3'>32*32</div>");
                this.Add("<div id='Content-Main'><img src='../DataUser/UserIcon/Default.png' border=1 width='60px' height='60px'/></div>");
                this.Add("<div id='Content-Main3'>60*60</div>");
                this.Add("<div id='Content-Main1'><img src='../DataUser/UserIcon/Default.png' border=1 width='100px' height='100px'/></div>");
                this.Add("<div id='Content-Main3'>100*100</div>");
                this.Add("</div>");
                this.Add("</div>");
            }
            this.AddTableEnd();
            this.AddBR();
            this.AddFieldSetEnd();
        }

        public void BitmapCutter()
        {
            if (WebUser.IsWap)
                this.AddFieldSet("<a href=Home.aspx ><img src='../Img/Home.gif' border=0 >主页</a>-<a href='" + this.PageID + ".aspx'>设置</a>-设置图标");
            else
                this.AddFieldSet("设置图标");

            string picfile = BP.Sys.SystemConfig.PathOfWebApp + "DataUser\\UserIcon";
            bool isNo = false;

            System.Drawing.Image myImage = null;
            int phWidth = 0;
            int phHeight = 0;
            DirectoryInfo di = new DirectoryInfo(picfile);
            if (di.Exists)
            {
                string[] subpic = Directory.GetFiles(picfile);
                if (subpic.Length > 0)
                {
                    foreach (string PicPath in subpic)
                    {
                        string[] tempFilePath = PicPath.Split('\\');

                        if (tempFilePath[tempFilePath.Length - 1].StartsWith(WebUser.No + "BigerCon"))
                        {
                            myImage = System.Drawing.Image.FromFile(picfile + "/" + WebUser.No + "BigerCon.png");
                            phWidth = myImage.Width;
                            phHeight = myImage.Height;
                            if (phWidth > 510)
                            {
                                isNo = false;
                            }
                            else
                            {
                                isNo = true;
                            }
                            myImage.Dispose();
                            break;
                        }
                    }
                }
            }
            this.Add("<div id='Player'>");
            TextBox tb = new TextBox();
            if (Request["Image"] == null)
            {
                if (isNo)
                {
                    tb.Text = WebUser.No + "BigerCon.png";
                }
                else
                {
                    tb.Text = "Default.png";
                }
            }
            else
            {
                tb.Text = Request["Image"];
            }
            tb.ID = "ImageName";
            tb.Attributes["style"] = "display:none";
            this.Add(tb);

            tb = new TextBox();
            if (Request["Width"] == null)
            {
                if (isNo)
                {

                    if (phWidth > 510)
                    {
                        tb.Text = "500";
                    }
                    else
                    {
                        tb.Text = phWidth.ToString();
                    }
                }
                else
                {
                    tb.Text = "500";
                }
            }
            else
            {
                tb.Text = Request["Width"];
            }
            tb.ID = "WSize";
            tb.Attributes["style"] = "display:none";
            this.Add(tb);

            tb = new TextBox();
            if (Request["Height"] == null)
            {
                if (isNo)
                {

                    if (phHeight > 800)
                    {
                        tb.Text = "400";
                    }
                    else
                    {
                        tb.Text = phHeight.ToString();
                    }
                }
                else
                {
                    tb.Text = "400";
                }
            }
            else
            {
                tb.Text = Request["Height"];
            }
            tb.ID = "HSize";
            tb.Attributes["style"] = "display:none";
            this.Add(tb);

            tb = new TextBox();
            if (Request["cHg"] == null)
            {
                if (isNo)
                {
                    tb.Text = (phWidth / 2).ToString();
                }
                else
                {
                    tb.Text = "200";
                }
            }
            else
            {
                tb.Text = Request["cHg"];
            }
            tb.ID = "Chg";
            tb.Attributes["style"] = "display:none";
            this.Add(tb);

            System.Web.UI.WebControls.FileUpload fu = new System.Web.UI.WebControls.FileUpload();
            fu.ID = "F";
            this.Add(fu);

            Btn btn = new Btn();
            btn.Text = "确定";
            btn.Click += new EventHandler(btn_Sure_Click);
            this.Add(btn);

            this.Add("</div>");
            this.Add("<div id='Container'>");
            this.Add("</div>");
            this.AddBR();
            this.AddFieldSetEnd();
        }

        void btn_Sure_Click(object sender, EventArgs e)
        {
            string guidName = Guid.NewGuid().ToString();
            FileUpload f = (FileUpload)this.FindControl("F");
            string imName = f.FileName;
            if (f.HasFile == false)
                return;
            string imgPath = Server.MapPath("~/DataUser/UserIcon/Model/" + WebUser.No + "Model.png");
            f.SaveAs(imgPath);
            string fileName = "Model.png";

            System.Drawing.Image myImage = System.Drawing.Image.FromStream(f.PostedFile.InputStream);
            int phWidth = myImage.Width;
            int phHeight = myImage.Height;
            int widths = 0;
            int heights = 0;
            int chg = 0;
            if (phWidth > 510)
            {

                if ((phWidth * 510) > (phHeight * 500))
                {
                    widths = 500;
                    heights = (510 * phHeight) / phWidth;
                }
                else
                {
                    heights = 500;
                }

                GetPicThumbnail(imgPath, Server.MapPath("~/DataUser/UserIcon/" + WebUser.No + "Model.png"), heights, 510);
                widths = 510;
            }
            else
            {
                if (phWidth < 450)
                {
                    if ((phWidth * 510) > (phHeight * 500))
                    {
                        widths = 510;
                        heights = (510 * phHeight) / phWidth;
                    }
                    else
                    {
                        heights = 500;
                    }

                    GetPicThumbnail(imgPath, Server.MapPath("~/DataUser/UserIcon/" + WebUser.No + "Model.png"), heights, 510);
                    widths = 510;
                }
                else
                {
                    widths = phWidth;
                    heights = phHeight;
                    GetPicThumbnail(imgPath, Server.MapPath("~/DataUser/UserIcon/" + WebUser.No + "Model.png"), heights, widths);
                    chg = Convert.ToInt32(widths * 0.5);
                    if (chg > 250)
                    {
                        chg = 250;
                    }
                    myImage.Dispose();
                }
            }
            if (File.Exists(imgPath))
            {
                File.SetAttributes(imgPath, FileAttributes.Normal);
                File.Delete(imgPath);
            }
            //System.Drawing.Image.GetThumbnailImageAbort callb = null;
            //System.Drawing.Image newImage = myImage.GetThumbnailImage(widths, heights, callb, new IntPtr());
            //newImage.Save(BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/UserIcon/" + WebUser.No + "Model.png");
            //newImage.Dispose();
            f.PostedFile.InputStream.Close();
            f.PostedFile.InputStream.Dispose();
            f.Dispose();

            string s = this.Request.QueryString["RefNo"];
            this.Response.Redirect("/WF/Tools.aspx?RefNo=" + s + "&Image=" + WebUser.No + fileName + "&Width=" + widths + "&Height=" + heights + "&cHg=" + chg, true);

        }
        /// <summary>
        /// 重新定义一个图片
        /// </summary>
        /// <param name="fromImg">Image类型的图片的路径</param>
        /// <param name="width">重新定义图片的宽度</param>
        /// <param name="height">重新定义图片的高度</param>
        /// <returns></returns>
        public static Bitmap MakeThumbnail(System.Drawing.Image fromImg, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            int ow = fromImg.Width;
            int oh = fromImg.Height;

            //新建一个画板
            Graphics g = Graphics.FromImage(bmp);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            g.DrawImage(fromImg, new Rectangle(0, 0, width, height),
                new Rectangle(0, 0, ow, oh),
                GraphicsUnit.Pixel);

            return bmp;

        }
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">图片的原始路径</param>
        /// <param name="dFile">缩放后的保存路径</param>
        /// <param name="dHeight">缩放后图片的高度</param>
        /// <param name="dWidth">缩放后图片的宽带</param>
        /// <returns></returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);//从指定的文件创建Image
            ImageFormat tFormat = iSource.RawFormat;//指定文件的格式并获取
            int sW = 0, sH = 0;//记录宽度和高度
            Size tem_size = new Size(iSource.Width, iSource.Height);//实例化size。知矩形的高度和宽度
            if (tem_size.Height > dHeight || tem_size.Width > dWidth)//判断原图大小是否大于指定大小
            {
                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else//如果原图大小小于指定的大小
            {
                sW = dWidth;//原图宽度等于指定宽度
                sH = dHeight;//原图高度等于指定高度
            }
            //得到重新定义的图片
            Bitmap thumBnail = MakeThumbnail(iSource, sH, 510);

            Bitmap oB = new Bitmap(dWidth, dHeight);//实例化
            Graphics g = Graphics.FromImage(oB);//从指定的Image中创建Graphics
            Rectangle destRect = new Rectangle(new Point(0, 0), new Size(510, sH));//目标位置
            Rectangle origRect = new Rectangle(new Point(0, 0), new Size(sH, 510));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）
            Graphics G = Graphics.FromImage(oB);

            G.Clear(Color.White);
            // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
            G.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 指定高质量、低速度呈现。 
            G.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawImage(thumBnail, destRect, origRect, GraphicsUnit.Pixel);
            //G.DrawString("Xuanye", f, b, 0, 0);
            G.Dispose();
            //保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();//用于向图像编码器传递值
            long[] qy = new long[1];
            qy[0] = 100;
            EncoderParameter eParm = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParm;
            try
            {
                oB.Save(dFile, tFormat);// 已指定格式保存到指定文件
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();//释放资源
                oB.Dispose();
            }
        }
    }
}