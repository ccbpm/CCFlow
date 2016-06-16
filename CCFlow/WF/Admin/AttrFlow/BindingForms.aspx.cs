using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF;
using BP.WF.Template;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class BindingForms : System.Web.UI.Page
    {
        #region 属性.
        private string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        private string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        private MapData md
        {
            get
            {
                MapData mapData = new MapData(this.FK_MapData);

                return mapData;
            }
        }
        private Nodes nds
        {
            get
            {
                Nodes nodes = new Nodes(this.FK_Flow);
                return nodes;
            }
        }
        private Flow flow
        {
            get
            {
                Flow f = new Flow(this.FK_Flow);
                return f;
            }
        }
        #endregion 属性.
        protected void Page_Load(object sender, EventArgs e)
        {


            //没有指定父容器高宽，宽度100%不可用，
            //指定固定width，页面放大缩小，布局不会乱
            this.Pub1.AddTable(" style='width:100%;' ");
            this.Pub1.AddCaption("表单[" + md.Name + "]与流程[" + flow.Name + "]上全部节点的绑定");

            this.Pub1.AddTR();
            string thCenter = "' style='text-align:center;'";
            this.Pub1.AddTH(thCenter, "节点编号");
            this.Pub1.AddTH(thCenter, "是否绑定");
            this.Pub1.AddTH(thCenter, "可否编辑");
            this.Pub1.AddTH(thCenter, "可否打印");
            this.Pub1.AddTH(thCenter, "是否启用装载填充事件");
            this.Pub1.AddTH(thCenter, "权限控制方案");
            this.Pub1.AddTH(thCenter, "表单元素控制方案");
            this.Pub1.AddTH(thCenter, "谁是主键");
            this.Pub1.AddTREnd();


            int idx = 1;
            FrmNode fn = null;
            CheckBox cb = null;
            BP.Web.Controls.DDL ddl = null;

            #region 循环添加Node
            foreach (Node nd in nds)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(nd.NodeID);

                //检索出来的数据是唯一的
                fn = new FrmNode(this.FK_Flow,
                    nd.NodeID, this.FK_MapData);

                cb = new CheckBox();
                cb.ID = "CB_NodeName_" + nd.NodeID;
                cb.Text = nd.Name;

                if (fn.FK_Node == nd.NodeID)
                    cb.Checked = true;
                else
                    cb.Checked = false;

                this.Pub1.AddTD(" ' style='with:100px;' ", cb);


                cb = new CheckBox();
                cb.ID = "CB_IsEdit_" + nd.NodeID;
                cb.Text = "可否编辑";
                cb.Checked = fn.IsEdit;
                this.Pub1.AddTD(thCenter, cb);

                cb = new CheckBox();
                cb.ID = "CB_IsPrint_" + nd.NodeID;
                cb.Text = "可否打印";
                cb.Checked = fn.IsPrint;
                this.Pub1.AddTD(thCenter, cb);

                cb = new CheckBox();
                cb.ID = "CB_IsEnableLoadData_" + nd.NodeID;
                cb.Text = "是否启用";
                cb.Checked = fn.IsEnableLoadData;
                this.Pub1.AddTD(thCenter, cb);

                ddl = new BP.Web.Controls.DDL();
                ddl.ID = "DDL_Sln_" + nd.NodeID;
                ddl.Items.Add(new ListItem("默认方案", "0"));
                ddl.Items.Add(new ListItem("自定义", nd.NodeID.ToString()));
                ddl.SetSelectItem(fn.FrmSln); //设置权限控制方案.
                this.Pub1.AddTD(thCenter, ddl);


                this.Pub1.AddTDBegin(" style='text-align:center;' ");
                this.Pub1.Add("<a href=\"javascript:WinField('" + md.No + "','" + nd.NodeID + "','" + this.FK_Flow + "')\" >字段</a>");
                this.Pub1.Add("-<a href=\"javascript:WinFJ('" + md.No + "','" + nd.NodeID + "','" + this.FK_Flow + "')\" >附件</a>");
                this.Pub1.Add("-<a href=\"javascript:WinDtl('" + md.No + "','" + nd.NodeID + "','" + this.FK_Flow + "')\" >从表</a>");

                if (md.HisFrmType == FrmType.ExcelFrm)
                    this.Pub1.Add("-<a href=\"javascript:ToolbarExcel('" + md.No + "','" + nd.NodeID + "','" + this.FK_Flow + "')\" >ToolbarExcel</a>");

                if (md.HisFrmType == FrmType.WordFrm)
                    this.Pub1.Add("-<a href=\"javascript:ToolbarWord('" + md.No + "','" + nd.NodeID + "','" + this.FK_Flow + "')\" >ToolbarWord</a>");

                this.Pub1.AddTDEnd();


                ddl = new BP.Web.Controls.DDL();
                ddl.ID = "DDL_WhoIsPK_" + nd.NodeID;
                ddl.BindSysEnum("WhoIsPK");
                ddl.SetSelectItem((int)fn.WhoIsPK); //谁是主键？.
                this.Pub1.AddTD(thCenter, ddl);

                this.Pub1.AddTREnd();
                idx += 1;
            }
            #endregion  循环添加Node

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin(" colspan='8' style='text-align:right;border:none;padding-top:20px;' ");

            Button btn = new Button();
            btn.ID = "Save";
            btn.Text = "保存";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SavePowerOrders_Click);

            this.Pub1.Add(btn);

            string text = "<input style='margin-left:20px;margin-right:40px;' type=button " +
                          "onclick=\"javascript:closeCurTab(\'" + md.Name + "\');\" value='关闭'  class=Btn />";
            this.Pub1.Add(text);

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        void btn_SavePowerOrders_Click(object sender, EventArgs e)
        {
            //删除以前绑定的值.
            FrmNodes fnds = new FrmNodes();
            fnds.Delete(FrmNodeAttr.FK_Flow, this.FK_Flow,
                FrmNodeAttr.FK_Frm, this.FK_MapData);

            FrmNode fn = null;
            foreach (Node nd in nds)
            {
                //节点没有选中,后面的明细项目选中也不执行保存
                bool isCheckCurNd = this.Pub1.GetCBByID("CB_NodeName_" + nd.NodeID.ToString()).Checked;
                if (!isCheckCurNd)
                    continue;

                fn = new FrmNode();

            //    fn.IsEdit = this.Pub1.GetCBByID("CB_IsEdit_" + nd.NodeID.ToString()).Checked;
                fn.IsPrint = this.Pub1.GetCBByID("CB_IsPrint_" + nd.NodeID.ToString()).Checked;

                //是否启
                fn.IsEnableLoadData = this.Pub1.GetCBByID("CB_IsEnableLoadData_" + nd.NodeID.ToString()).Checked;

                //权限控制方案.
                fn.FrmSln = this.Pub1.GetDDLByID("DDL_Sln_" + nd.NodeID.ToString()).SelectedItemIntVal;
                fn.WhoIsPK = (WhoIsPK)this.Pub1.GetDDLByID("DDL_WhoIsPK_" + nd.NodeID.ToString()).SelectedItemIntVal;

                fn.FK_Flow = this.FK_Flow;
                fn.FK_Node = nd.NodeID;
                fn.FK_Frm = this.FK_MapData;

                fn.MyPK = fn.FK_Frm + "_" + fn.FK_Node + "_" + fn.FK_Flow;

                fn.Insert();
            }

            this.Response.Redirect("BindingForms.aspx?FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow);
        }
    }
}