using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using BP.Demo;
using BP.Demo.License;

namespace CCFlow.app
{
    public partial class Guide : BP.Web.WebPage
    {
        #region 外部参数.
        /// <summary>
        /// 身份证
        /// </summary>
        public string SFZ
        {
            get
            {
                string str= this.Request.QueryString["SFZ"];
                if (string.IsNullOrEmpty(str))
                    str = this.TB_SFZ.Text;

                return str;
            }
        }
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
                this.BindData();
        }

        public void BindData()
        {
            if (this.SFZ == null || this.SFZ.Length < 15)
                return;

            People pl = new People();
            pl.No = this.SFZ;
            pl.RetrieveFromDBSources();

                
            this.TB_Name.Text = pl.Name;
            this.TB_SFZ.Text = pl.No;
            this.TB_Tel.Text = pl.Tel;
            this.TB_Addr.Text = pl.Addr;
            this.TB_Email.Text = pl.Email;
            this.TB_BDT.Text = pl.BDT;
            this.TB_XB.Text = pl.XB;


            this.Pub1.AddTR();
            this.Pub1.AddTH("类型");
            this.Pub1.AddTH("上传日期/上传人/编码");
            this.Pub1.AddTH("上传");
            this.Pub1.AddTH("操作");
            this.Pub1.AddTREnd();

            #region 绑定已经存在的证照.
            Licenses ens = new Licenses(this.SFZ);
            string types = "";
            foreach (License en in ens)
            {
                types += en.Name+",";
                this.Pub1.AddTR();
                this.Pub1.AddTD(en.Name);
                this.Pub1.AddTD( en.RDT+"/"+en.Rec+"/"+en.ZJCode);
                FileUpload fu = new FileUpload();
                fu.ID = en.MyPK;
                this.Pub1.AddTD(fu);
                //this.Pub1.AddTD("[<a href=\"javascript:Del('" + en.MyPK + "','" + en.SFZ + "')\'>删除</a>]");
                this.Pub1.AddTD("[删除][查看]");
                this.Pub1.AddTREnd();
            }
            #endregion 绑定已经存在的证照.

            #region 绑定节点需要的证照.
            //获得该节点需要的证照信息,并把证照写入到附件表里.
            Node nd = new Node(int.Parse(this.FK_Flow + "01"));

            //获得附件描述.
            FrmAttachment ath = new FrmAttachment("ND" + nd.NodeID + "_AttachM1");
            string[] sorts = ath.Sort.Split(','); //获得附件类型.
            if (sorts.Length == 1)
                throw new Exception("@该流程不需要上传证照信息,或者设计人员配置错误，没有设计要上传的证照类型.");

            foreach (string str in sorts)
            {
                if (types.Contains(str))
                    continue;

                this.Pub1.AddTR();
                this.Pub1.AddTD(str); //名称
                this.Pub1.AddTD("无/无/无");  //代码

                FileUpload fu = new FileUpload();
                fu.ID = "N_" + LicenseType.GetIDByName(str);
                this.Pub1.AddTD(fu); //上传
                this.Pub1.AddTD("无"); // 操作
                this.Pub1.AddTREnd();
            }
            #endregion 绑定节点需要的证照.

        }

        protected void Btn_Search_Click(object sender, EventArgs e)
        {
            string sfz = this.TB_SFZ.Text;
            if (sfz.Length <= 15)
            {
                this.Response.Write("证件号码格式错误错误.");
                return;
            }

            People pe = new People();
            pe.No = sfz;

            if (pe.RetrieveFromDBSources() == 0)
            {
                this.Response.Write("没有查询到该身份证号的数据，您可以完善信息执行保存....");
                return;
            }

            string paras = this.RequestParas;
            if (paras.Contains("SFZ=") == false)
                paras += "&SFZ=" + pe.No;
            //转到当前界面.
            this.Response.Redirect("StartGuideSelfUrlEntity.aspx?1=1" + paras, true);
        }

        protected void Btn_Start_Click(object sender, EventArgs e)
        {
            People en = new People();
            en.No = this.TB_SFZ.Text;
            if (en.RetrieveFromDBSources() == 0)
            {
                this.Response.Write("证件号码错误.....");
                return;
            }

            //获得该节点需要的证照信息,并把证照写入到附件表里.
            Node nd = new Node(int.Parse(this.FK_Flow + "01"));
            Work wk = nd.HisWork;

            //获得附件描述.
            FrmAttachment ath = new FrmAttachment("ND" + nd.NodeID + "_AttachM1");
            string[] sort = ath.Sort.Split(','); //获得附件类型.
            if (sort.Length == 1)
                throw new Exception("@该流程不需要上传证照信息.");

            //删除原来的数据，如果有。
            FrmAttachmentDBs dbs = new FrmAttachmentDBs();
            dbs.Delete(FrmAttachmentDBAttr.RefPKVal, this.WorkID);

            //求出证件库下的证照集合.
            Licenses lis = new Licenses(this.SFZ);

            //开始象流程的开始节点写附件数据.
            foreach (string str in sort)
            {
                foreach (License li in lis)
                {
                    if (li.Name != str)
                        continue; //不是continue.

                    FrmAttachmentDB db = new FrmAttachmentDB();
                    db.MyPK = BP.DA.DBAccess.GenerGUID();
                    db.UploadGUID = li.MyPK;
                    db.Rec = BP.Web.WebUser.No;
                    db.RecName = BP.Web.WebUser.Name;

                    db.RDT = BP.DA.DataType.CurrentDataTime;
                    db.MyNote = "从证照库导入";
                    db.FID = 0;
                    db.RefPKVal = this.WorkID.ToString();
                    db.FK_FrmAttachment = "ND" + nd.NodeID + "_AttachM1";
                    db.FK_MapData = "ND" + nd.NodeID;
                    db.FileFullName = li.FilePath;
                    db.FileName = str;
                    db.FileExts = li.Ext;
                    db.FileSize = li.FileSize;
                    db.Sort = str;
                    db.Insert();
                }
            }

            //生成Url把其他的参数带入里面去.
            string url = "/WF/MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&IsCheckGuide=1&DiZhi=" + en.Addr + "&SFZH=" + en.No + "&DianHua=" + en.Tel + "&XingBie=" + en.XB + "&YouJian=" + en.Email + "&XingMing=" + en.Name;
            this.Response.Redirect(url, true);
        }
        /// <summary>
        /// 保存到证照数据库.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_SaveToEntity_Click(object sender, EventArgs e)
        {
            People pe = new People();
            pe.No = this.TB_SFZ.Text;

            if (pe.No.Length <= 15)
            {
                this.Response.Write("@身份证，不符合要求.");
                return;
            }

            pe.Name = this.TB_Name.Text;
            pe.Addr = this.TB_Addr.Text;
            pe.Email = this.TB_Email.Text;
            pe.Tel = this.TB_Tel.Text;
            pe.XB = this.TB_XB.Text;
            pe.Age = this.TB_Age.Text;
            pe.BDT = this.TB_BDT.Text;
            pe.Save();


            #region 保存附件.
            //获得该节点需要的证照信息,并把证照写入到附件表里.
            Node nd = new Node(int.Parse(this.FK_Flow + "01"));
            Work wk = nd.HisWork;

            //获得附件描述.
            FrmAttachment ath = new FrmAttachment("ND" + nd.NodeID + "_AttachM1");
            string[] sorts = ath.Sort.Split(','); //获得附件类型.
            if (sorts.Length == 1)
                throw new Exception("@该流程不需要上传证照信息,或者设计人员配置错误，没有设计要上传的证照类型.");

            Licenses ens = new Licenses(this.SFZ);
            string types = "";
            foreach (License en in ens)
            {
                types += en.Name + ",";
            }

            foreach (string str in sorts)
            {
                if (types.Contains(str))
                    continue;

                string id = "N_" + LicenseType.GetIDByName(str);

                FileUpload fu = (FileUpload)this.Pub1.FindControl(id);
                if (fu == null)
                    throw new Exception("@没有找到ID"+id+"的上传控件.");
                if (fu.HasFile == false)
                    continue;
             
                //处理路径.
                string path = SystemConfig.PathOfDataUser + "\\UploadFile\\"+BP.DA.DataType.CurrentYear+"\\"+this.FK_Flow;
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                //保存一个临时文件.
                string tempFile=path + "\\" + fu.FileName;
                fu.SaveAs(tempFile);

                System.IO.FileInfo finfo = new System.IO.FileInfo(tempFile);

                string guid = BP.DA.DBAccess.GenerGUID();
                License li = new License();
                li.MyPK = guid;
                li.SFZ = this.SFZ;
                li.ZJCode = "";
                li.ZJLX = str;

                //证件类型.
                li.ZJLX = LicenseType.GetIDByName(str);

                //按照Guid + 扩展名存储临时文件.
                li.FilePath = path + "\\" + guid + "." + finfo.Extension;
                finfo.MoveTo(li.FilePath); //把当前的文件重命名.

                li.Rec = BP.Web.WebUser.No;
                li.RDT =BP.DA.DataType.CurrentData;
                li.Ext = finfo.Extension;
                li.FileSize = (float)finfo.Length;
                li.Insert();
            }
            #endregion 保存附件.


            string paras = this.RequestParas;
            if (paras.Contains("SFZ=") == false)
                paras += "&SFZ=" + pe.No;
               //转到当前界面.
            this.Response.Redirect("StartGuideSelfUrlEntity.aspx?1=1" + paras, true);
        }
    }
}