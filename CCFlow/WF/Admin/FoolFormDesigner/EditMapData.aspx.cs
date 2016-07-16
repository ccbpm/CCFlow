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
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_EditMapData : System.Web.UI.Page
    {
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            this.Pub1.AddTable();
            //   this.Pub1.AddCaptionLeft("傻瓜表单属性");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("信息");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("编号");
            this.Pub1.AddTD(this.FK_MapData);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("名称");
            TextBox tb = new TextBox();
            tb.ID = "TB_" + MapDataAttr.Name;
            tb.Text = md.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表单主表");
            tb = new TextBox();
            tb.ID = "TB_" + MapDataAttr.PTable;
            tb.Text = md.PTable;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("显示宽度(单位px)");
            tb = new TextBox();
            tb.ID = "TB_" + MapDataAttr.TableWidth;
            tb.Text = md.GetValStringByKey(MapDataAttr.TableWidth);
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("如果设置为0 则认为是100%.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("表格呈现列数");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_Col";
            ddl.Items.Add(new ListItem("4", "4"));
            ddl.Items.Add(new ListItem("6", "6"));
            ddl.Items.Add(new ListItem("8", "8"));
            ddl.Items.Add(new ListItem("10", "10"));
            ddl.Items.Add(new ListItem("12", "12"));
            ddl.SetSelectItem(md.TableCol);
            this.Pub1.AddTD(ddl);

            this.Pub1.AddTD("用于控制表单字段的横向布局.");
            this.Pub1.AddTREnd();

            if (md.No.Contains("ND") == true)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD("审核组件");
                FrmWorkCheck fwc = new FrmWorkCheck(md.No);
                ddl = new BP.Web.Controls.DDL();
                ddl.ID = "DDL_FWC";
                ddl.Items.Add(new ListItem("不可用", "0"));
                ddl.Items.Add(new ListItem("可填写", "1"));
                ddl.Items.Add(new ListItem("只读", "2"));
                ddl.SetSelectItem((int)fwc.HisFrmWorkCheckSta);
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTD("用户节点工作审核.");
                this.Pub1.AddTREnd();
            }

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("表单权限控制方案");
            //tb = new TextBox();
            //tb.ID = "TB_" + MapDataAttr.Slns;
            //tb.Text = md.Slns;
            //tb.Columns = 60;
            //this.Pub1.AddTD("colspan=2", tb);
            //this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=3", "方案说明:格式为:@0=默认@1=第1套方案@2=第2套方案@3=第3套方案");
            this.Pub1.AddTREnd();

            this.Pub1.AddTableEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            md = this.Pub1.Copy(md) as MapData;
            md.TableCol = this.Pub1.GetDDLByID("DDL_Col").SelectedItemIntVal;
            md.Update();

            try
            {
                FrmWorkCheck fwc = new FrmWorkCheck(md.No);
                fwc.HisFrmWorkCheckSta = (FrmWorkCheckSta)this.Pub1.GetDDLByID("DDL_FWC").SelectedItemIntVal;
                fwc.Update();
            }
            catch
            {
            }
            
            BP.Sys.PubClass.WinClose();
        }
    }
}
