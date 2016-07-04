using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;

namespace CCFlow.WF.Comm.Sys
{
    public partial class SFDBSrcUI : System.Web.UI.Page
    {
        #region property
        /// <summary>
        /// 操作类型
        /// </summary>
        public string DoType
        {
            get { return this.Request.QueryString["DoType"]; }
        }
        /// <summary>
        /// 修改的数据源编号
        /// </summary>
        public string No
        {
            get { return this.Request.QueryString["No"]; }
        }
        /// <summary>
        /// 改变的数据源类型
        /// </summary>
        public int SrcType
        {
            get
            {
                return
                    int.Parse(string.IsNullOrWhiteSpace(Request.QueryString["SrcType"])
                                  ? "-1"
                                  : Request.QueryString["SrcType"]);
            }
        }
        /// <summary>
        /// 是否改变数据源类型
        /// </summary>
        public bool IsChange
        {
            get { return Request.QueryString["IsChange"] == "1"; }
        }

        public string DateTimeSpan
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmssffffff"); }
        }
        #endregion

        #region private property
        private SFDBSrc src = null;
        private bool isEdit;
        private int srcType = -1;
        #endregion

        #region page load
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebUser.No != "admin")
            {
                Alert("只有管理员才能进行数据源的维护！");
                return;
            }

            switch (DoType)
            {
                case "New":
                case "Edit":
                    LoadSrc();
                    break;
                default:
                    BindList();
                    break;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// 加载数据源编辑区
        /// </summary>
        private void LoadSrc()
        {
            src = new SFDBSrc();

            if (!string.IsNullOrWhiteSpace(No))
                src = new SFDBSrc(No);

            isEdit = !string.IsNullOrWhiteSpace(src.No);
            srcType = isEdit ? (IsChange ? SrcType : (int)src.DBSrcType) : (SrcType == -1 ? 1 : SrcType);

            this.Title = (isEdit ? "修改" : "新建") + "数据源";

            if(isEdit && src.No == "local")
            {
                Alert(src.Name + "不允许修改！");
                return;
            }

            DDL ddl;
            TB tb;
            SysEnums enums = new SysEnums();
            enums.Retrieve(SysEnumAttr.EnumKey, SFDBSrcAttr.DBSrcType, SysEnumAttr.IntKey);

            pub1.AddTableNormal();

            pub1.AddTR();
            pub1.AddTDGroupTitle("style='width:140px'", "类型：");

            ddl = new DDL();
            ddl.ID = "DDL_" + SFDBSrcAttr.DBSrcType;

            foreach (SysEnum en in enums)
                ddl.Items.Add(new ListItem(en.Lab, en.IntKey.ToString(), en.IntKey != 0));

            ddl.SetSelectItem(srcType.ToString());
            ddl.Attributes["onchange"] = "location.href='./SFDBSrcNewGuide.aspx?DoType=" + DoType + "&IsChange=1&No=" + No + "&t=" + DateTimeSpan + "&SrcType=' + this.options[this.selectedIndex].value";
            pub1.AddTD(ddl);
            pub1.AddTREnd();

            //编号
            pub1.AddTR();
            pub1.AddTDGroupTitle("编号(英文)：");

            tb = new TB();
            tb.ID = "TB_" + SFDBSrcAttr.No;
            tb.Enabled = !isEdit;

            if (isEdit)
                tb.Text = src.No;

            pub1.AddTD(tb);
            pub1.AddTREnd();

            //名称
            pub1.AddTR();
            pub1.AddTDGroupTitle("名称：");

            tb = new TB();
            tb.ID = "TB_" + SFDBSrcAttr.Name;

            if (isEdit)
                tb.Text = src.Name;

            pub1.AddTD(tb);
            pub1.AddTREnd();

            switch ((DBSrcType)srcType)
            {
                case DBSrcType.SQLServer:
                case DBSrcType.Oracle:
                case DBSrcType.MySQL:
                case DBSrcType.Informix:
                    //数据库名称
                    if (SrcType != (int)DBSrcType.Oracle)
                    {
                        pub1.AddTR();
                        pub1.AddTDGroupTitle("数据库名称：");

                        tb = new TB();
                        tb.ID = "TB_" + SFDBSrcAttr.DBName;

                        if (isEdit)
                            tb.Text = src.DBName;

                        pub1.AddTD(tb);
                        pub1.AddTREnd();
                    }

                    //数据库服务器IP地址/数据库实例名称
                    pub1.AddTR();
                    pub1.AddTDGroupTitle(SrcType == (int)DBSrcType.Oracle ? "数据库实例名称：" : "数据库IP地址：");

                    tb = new TB();
                    tb.ID = "TB_" + SFDBSrcAttr.IP;

                    if (isEdit)
                        tb.Text = src.IP;

                    pub1.AddTD(tb);
                    pub1.AddTREnd();

                    //数据库登录帐号
                    pub1.AddTR();
                    pub1.AddTDGroupTitle("数据库登录帐号：");

                    tb = new TB();
                    tb.ID = "TB_" + SFDBSrcAttr.UserID;

                    if (isEdit)
                        tb.Text = src.UserID;

                    pub1.AddTD(tb);
                    pub1.AddTREnd();

                    //数据库登录密码
                    pub1.AddTR();
                    pub1.AddTDGroupTitle("数据库登录密码：");

                    tb = new TB();
                    tb.ID = "TB_" + SFDBSrcAttr.Password;

                    if (isEdit)
                        tb.Text = src.Password;

                    pub1.AddTD(tb);
                    pub1.AddTREnd();
                    break;
                case DBSrcType.WebServices:
                    //WebService地址
                    pub1.AddTR();
                    pub1.AddTDGroupTitle("WebService地址(Url)：");

                    tb = new TB();
                    tb.ID = "TB_" + SFDBSrcAttr.IP;
                    tb.Style.Add("width", "480px");

                    if (isEdit)
                        tb.Text = src.IP;

                    pub1.AddTD(tb);
                    pub1.AddTREnd();
                    break;
                default:
                    throw new Exception("未涉及的数据源类型！");
            }

            pub1.AddTableEnd();
            pub1.AddBR();
            pub1.AddSpace(1);

            LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Click);
            pub1.Add(btn);

            if (isEdit)
            {
                pub1.AddSpace(1);
                btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
                btn.OnClientClick = "return confirm('确定要删除本数据源吗？数据源一经使用，不允许删除！');";
                btn.Click += new EventHandler(btnDelete_Click);
                pub1.Add(btn);
                pub1.AddSpace(1);

                pub1.AddEasyUiLinkButton("测试连接", "../RefMethod.aspx?Index=0&EnsName=BP.Sys.SFDBSrcs&No=" + No +
                                    "&r=" + DateTimeSpan, "icon-rights", false, "_blank");
            }

            pub1.AddSpace(1);
            pub1.AddEasyUiLinkButton("返回", "./SFDBSrcNewGuide.aspx?t=" + DateTimeSpan, "icon-back");
        }
        /// <summary>
        /// 加载所有数据源列表
        /// </summary>
        private void BindList()
        {
            this.Title = "数据源管理";

            SysEnums enums = new SysEnums(SFDBSrcAttr.DBSrcType);
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();

            pub1.AddTableNormal();

            pub1.AddTR();
            pub1.AddTDGroupTitle("style='text-align:center;width:40px;'", "序");
            pub1.AddTDGroupTitle("style='width:120px'", "编号");
            pub1.AddTDGroupTitle("style='width:120px'", "名称");
            pub1.AddTDGroupTitle("style='width:100px'", "类型");
            pub1.AddTDGroupTitle("操作");
            pub1.AddTREnd();

            int i = 1;

            foreach (SFDBSrc src in srcs)
            {
                pub1.AddTR();
                pub1.AddTDIdx(i++);
                pub1.AddTD(src.No);
                pub1.AddTD(src.Name);
                pub1.AddTD((enums.GetEntityByKey(SysEnumAttr.IntKey, (int)src.DBSrcType) as SysEnum).Lab);
                pub1.AddTDBegin();

                if (src.No != "local")
                    pub1.AddEasyUiLinkButton("修改", "./SFDBSrcNewGuide.aspx?DoType=Edit&No=" + src.No + "&t=" + DateTimeSpan,
                                             "icon-edit");
                pub1.AddTDEnd();
                pub1.AddTREnd();
            }

            pub1.AddTableEnd();
            pub1.AddBR();
            pub1.AddEasyUiLinkButton("新建数据源", "./SFDBSrcNewGuide.aspx?DoType=New", "icon-new");
        }

        /// <summary>
        /// 弹出alert消息框
        /// </summary>
        /// <param name="msg">消息</param>
        private void Alert(string msg)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "msg",
                                                        "alert('" + msg.Replace("'", "\"") + "');", true);
        }

        /// <summary>
        /// 弹出alert消息框，然后页面转向指定url
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="url">url</param>
        private void AlertAndGo(string msg, string url)
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "msg",
                                                        "alert('" + msg.Replace("'", "\"") + "');location.href='" + url + "';", true);
        }
        #endregion

        #region btn click
        //保存
        void btn_Click(object sender, EventArgs e)
        {
            int dbSrcType = this.pub1.GetDDLByID("DDL_" + SFDBSrcAttr.DBSrcType).SelectedItemIntVal;
            string no = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.No).Text;
            string name = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.Name).Text;

            //新建数据源
            if (string.IsNullOrWhiteSpace(no) || string.IsNullOrWhiteSpace(name))
            {
                Alert("数据源编号/名称不能为空！");
                return;
            }

            if (!isEdit)
            {
                src = new SFDBSrc();
                src.No = no;

                if (src.RetrieveFromDBSources() > 0)
                {
                    Alert("已经存在数据源编号为“" + no + "”的数据源，编号不能重复！");
                    return;
                }

                src.No = no;
            }

            src.Name = name;
            src.DBSrcType = (DBSrcType)dbSrcType;

            switch (src.DBSrcType)
            {
                case DBSrcType.SQLServer:
                case DBSrcType.Oracle:
                case DBSrcType.MySQL:
                case DBSrcType.Informix:
                    if (src.DBSrcType != DBSrcType.Oracle)
                        src.DBName = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.DBName).Text;
                    else
                        src.DBName = string.Empty;

                    src.IP = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.IP).Text;
                    src.UserID = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.UserID).Text;
                    src.Password = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.Password).Text;
                    break;
                case DBSrcType.WebServices:
                    src.DBName = string.Empty;
                    src.IP = this.pub1.GetTBByID("TB_" + SFDBSrcAttr.IP).Text;
                    src.UserID = string.Empty;
                    src.Password = string.Empty;
                    break;
                default:
                    break;
            }

            src.Save();
            AlertAndGo("保存成功！", "./SFDBSrcNewGuide.aspx?DoType=Edit&No=" + src.No + "&t=" + DateTimeSpan);
        }
        //删除
        void btnDelete_Click(object sender, EventArgs e)
        {
            //检验要删除的数据源是否有引用
            SFTables sfs = new SFTables();
            sfs.Retrieve(SFTableAttr.FK_SFDBSrc, src.No);

            if (sfs.Count > 0)
            {
                Alert("当前数据源已经使用，不能删除！");
                return;
            }

            src.Delete();
            AlertAndGo("删除成功！", "./SFDBSrcNewGuide.aspx?t=" + DateTimeSpan);
        }
        #endregion
    }
}