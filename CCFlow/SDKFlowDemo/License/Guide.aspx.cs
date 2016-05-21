using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.GovServices;
using BP.WF.Template;
using BP.WF;
using BP.Sys;

namespace CCFlow.app
{
    public partial class Guide : System.Web.UI.Page
    {
        #region 外部参数.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        #endregion 外部参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                this.BindData();
            }
        }

        public void BindData()
        {
        }

        protected void Btn_Search_Click(object sender, EventArgs e)
        {
            string sfz = this.TB_SFZ.Text;
            if (sfz.Length <= 15)
            {
                this.Response.Write("请输入正确的身份证号....");
            }

            BP.GovServices.People en = new BP.GovServices.People();
            en.No = sfz;
            if (en.RetrieveFromDBSources() == 0)
            {
                this.Response.Write("证件号码错误.");
                return;
            }

            #region 开始绑定基础信息。
            this.TB_Name.Text = en.Name;
            this.TB_Addr.Text = en.Addr;

            #endregion 开始绑定基础信息

            #region 开始 证照 信息。

            Licenses ens = new Licenses(sfz);
            foreach (License item in ens)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD(item.Name+item.RDT);
                this.Pub1.AddTD(item.RDT);

                this.Pub1.AddTD("文件:______[上传]");
                this.Pub1.AddTD("<a href=''>删除</a>");
                this.Pub1.AddTREnd();
            }

            #endregion 开始 证照 信息。

        }

        protected void Btn_Start_Click(object sender, EventArgs e)
        {
            BP.GovServices.People en = new BP.GovServices.People();
            en.No = this.TB_SFZ.Text;
            if (en.RetrieveFromDBSources() == 0)
            {
                this.Response.Write("证件号码错误.....");
                return;
            }

            //获得该节点需要的证照信息,并把证照写入到附件表里.
            Node  nd=new Node( int.Parse(this.FK_Flow+"01"));
            Work wk=nd.HisWork;

            //获得附件描述.
            FrmAttachment ath = new FrmAttachment("ND" + nd.NodeID + "_AttachM1");
            string[] sort=ath.Sort.Split(','); //获得附件类型.
            if (sort.Length == 1)
                throw new Exception("@该流程不需要上传证照信息.");

            //生成Url把其他的参数带入里面去.
            string url = "/WF/MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&IsCheckGuide=1&Addr="+en.Addr;
            this.Response.Redirect(url, true);
        }
    }
}