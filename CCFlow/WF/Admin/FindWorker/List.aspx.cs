using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.En;

namespace CCFlow.WF.Admin.FindWorker
{
    public partial class UIFindWorkerRoles :BP.Web.WebPage
    {
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 101;
                }
            }
        }
        public string FK_Flow
        {
            get
            {
                string str= this.Request.QueryString["FK_Flow"];
                if (string.IsNullOrEmpty(str))
                    return "001";
                return str;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 处理功能.
            FindWorkerRole en = new FindWorkerRole();
            switch (this.DoType)
            {
                case "Del": //删除.
                    en.OID = this.RefOID;
                    en.Delete();
                    this.WinClose();
                    return;
                case "Up": //Up.
                    en.OID = this.RefOID;
                    en.Retrieve();
                    en.DoUp();
                    this.WinClose();
                    return;
                case "Down": //Down.
                    en.OID = this.RefOID;
                    en.Retrieve();
                    en.DoDown();
                    this.WinClose();
                    return;
                case "UnEnable": //Down.
                    en.OID = this.RefOID;
                    en.Retrieve();
                    en.IsEnable = false;
                    en.Update();
                    this.WinClose();
                    return;
                case "Enable": //Down.
                    en.OID = this.RefOID;
                    en.Retrieve();
                    en.IsEnable = true;
                    en.Update();
                    this.WinClose();
                    return;
                default:
                    break;
            }
            #endregion 处理功能.


          

          //  this.Pub1.AddH2(nd.Name);

            this.Pub1.AddTable();
            this.Pub1.AddCaption("确定接收人范围规则，可以多个规则并行使用。");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");

            this.Pub1.AddTDTitle("主规则");
            this.Pub1.AddTDTitle("2级规则");
            this.Pub1.AddTDTitle("2级参数");

            this.Pub1.AddTDTitle("3级规则");
            this.Pub1.AddTDTitle("3级参数");

            this.Pub1.AddTDTitle("4级规则");
            this.Pub1.AddTDTitle("4级参数");

            this.Pub1.AddTDTitle("启用否?");
            this.Pub1.AddTDTitle("移动");
            this.Pub1.AddTDTitle("删除");
            this.Pub1.AddTDTitle("编辑");
            this.Pub1.AddTREnd();

            FindWorkerRoles ens = new FindWorkerRoles(this.FK_Node);
            int idx = 0;
            foreach (FindWorkerRole myen in ens)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);

                this.Pub1.AddTD(myen.SortText0);

                this.Pub1.AddTD(myen.SortText1);
                this.Pub1.AddTD(myen.TagText1);

                this.Pub1.AddTD(myen.SortText2);
                this.Pub1.AddTD(myen.TagText2);

                this.Pub1.AddTD(myen.SortText3);
                this.Pub1.AddTD(myen.TagText3);

                if (myen.IsEnable == true)
                    this.Pub1.AddTD(myen.IsEnable + "<a href=\"javascript:UnEnable('" + myen.OID + "')\" >禁用</a>");
                else
                    this.Pub1.AddTD(myen.IsEnable + "<a href=\"javascript:Enable('" + myen.OID + "')\" >启用</a>");

                this.Pub1.AddTD("<a href=\"javascript:Up('" + myen.OID + "')\" ><img src='../../Img/Btn/Up.gif' border=0 />上移</a>|<a href=\"javascript:Down('" + myen.OID + "')\" ><img src='../../Img/Btn/Down.gif' border=0 />下移</a>");
                this.Pub1.AddTD("<a href=\"javascript:Del('" + myen.OID + "')\" ><img src='../../Img/Btn/Delete.gif' border=0 />删除</a>");
                this.Pub1.AddTD("<a href=\"javascript:Edit('" + myen.SortVal0 + "','" + this.FK_Flow + "','" + this.FK_Node + "','" + myen.OID + "')\" ><img src='../../Img/Btn/Edit.gif' border=0 />编辑</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();

            this.Pub1.Add("<a href=\"javascript:New('" + this.FK_Flow + "','" + this.FK_Node + "');\" ><img src='../../Img/Btn/New.gif' border=0 />新建找人规则</a>");


        }
    }
}