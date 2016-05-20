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
using BP.WF.Template;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.Admin.UC
{
    public partial class WF_Admin_UC_CondDept : BP.Web.UC.UCBase3
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Attr
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Attr"]);
                }
                catch
                {
                    try
                    {
                        return this.DDL_Attr.SelectedItemIntVal;
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
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
                    return this.FK_MainNode;
                }
            }
        }
        public int FK_MainNode
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_MainNode"]);
            }
        }
        public int ToNodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["ToNodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CondType HisCondType
        {
            get
            {
                return (CondType)int.Parse(this.Request.QueryString["CondType"]);
            }
        }
        public string GetOperValText
        {
            get
            {
                if (this.Pub1.IsExit("TB_Val"))
                    return this.Pub1.GetTBByID("TB_Val").Text;
                return this.Pub1.GetDDLByID("DDL_Val").SelectedItem.Text;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["DoType"] == "Del")
            {
                Cond nd = new Cond(this.MyPK);
                nd.Delete();
                this.Response.Redirect("CondDept.aspx?CondType=" + (int)this.HisCondType + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + nd.NodeID + "&FK_Node=" + this.FK_MainNode + "&ToNodeID=" + nd.ToNodeID, true);
                return;
            }

            this.BindCond();
        }

        public void BindCond()
        {
            Cond cond = new Cond();
            cond.MyPK = this.GenerMyPK;
            cond.RetrieveFromDBSources();

            BP.WF.Node nd = new BP.WF.Node(this.FK_MainNode);
            BP.WF.Node tond = new BP.WF.Node(this.ToNodeID);

            this.Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=3 class='GroupTitle'", "部门选择");
            this.Pub1.AddTREnd();

            Depts sts = new Depts();
            sts.RetrieveAllFromDBSource();

            int i = 0;

            foreach (Dept st in sts)
            {
                i++;

                if (i == 4)
                    i = 1;

                if (i == 1)
                    Pub1.AddTR();

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + st.No;
                cb.Text = st.Name;
                if (cond.OperatorValue.ToString().Contains("@" + st.No + "@"))
                    cb.Checked = true;

                this.Pub1.AddTD(cb);

                if (i == 3)
                    Pub1.AddTREnd();
            }

            switch (i)
            {
                case 1:
                    Pub1.AddTD();
                    Pub1.AddTD();
                    Pub1.AddTREnd();
                    break;
                case 2:
                    Pub1.AddTD();
                    Pub1.AddTREnd();
                    break;
                default:
                    break;
            }

            this.Pub1.AddTableEnd();
            Pub1.AddBR();

            #region //增加“指定的操作员”选项，added by liuxc,2015-10-7
            var ddl = new DDL();
            ddl.ID = "DDL_" + CondAttr.SpecOperWay;
            ddl.Width = 200;
            ddl.Items.Add(new ListItem("当前操作员", "0"));
            ddl.Items.Add(new ListItem("指定节点的操作员", "1"));
            ddl.Items.Add(new ListItem("指定表单字段作为操作员", "2"));
            ddl.Items.Add(new ListItem("指定操作员编号", "3"));
            ddl.SetSelectItem((int)cond.SpecOperWay);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            Pub1.Add("指定的操作员：");
            Pub1.Add(ddl);
            Pub1.AddBR();
            Pub1.AddBR();

            var lbl = new Label();
            lbl.ID = "LBL1";

            switch (cond.SpecOperWay)
            {
                case SpecOperWay.SpecNodeOper:
                    lbl.Text = "节点编号：";
                    break;
                case SpecOperWay.SpecSheetField:
                    lbl.Text = "表单字段：";
                    break;
                case SpecOperWay.SpenEmpNo:
                    lbl.Text = "操作员编号：";
                    break;
                case SpecOperWay.CurrOper:
                    lbl.Text = "参数：";
                    break;
            }

            Pub1.Add(lbl);

            var tb = new TB();
            tb.ID = "TB_" + CondAttr.SpecOperPara;
            tb.Width = 200;
            tb.Text = cond.SpecOperPara;
            tb.Enabled = cond.SpecOperWay != SpecOperWay.CurrOper;
            Pub1.Add(tb);
            Pub1.AddSpace(1);
            Pub1.Add("多个值请用英文“逗号”来分隔。");
            Pub1.AddBR();
            Pub1.AddBR();
            #endregion

            Pub1.AddSpace(1);
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);
            Pub1.AddSpace(1);

            btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
            btn.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var openway = (SpecOperWay)Pub1.GetDDLByID("DDL_" + CondAttr.SpecOperWay).SelectedItemIntVal;
            var lbl = Pub1.GetLabelByID("LBL1");
            var tb = Pub1.GetTBByID("TB_" + CondAttr.SpecOperPara);

            switch (openway)
            {
                case SpecOperWay.SpecNodeOper:
                    lbl.Text = "节点编号：";
                    break;
                case SpecOperWay.SpecSheetField:
                    lbl.Text = "表单字段：";
                    break;
                case SpecOperWay.SpenEmpNo:
                    lbl.Text = "操作员编号：";
                    break;
                case SpecOperWay.CurrOper:
                    lbl.Text = "参数：";
                    break;
            }

            tb.Text = string.Empty;
            tb.Enabled = openway != SpecOperWay.CurrOper;
        }

        public Label Lab_Msg
        {
            get
            {
                return this.Pub1.GetLabelByID("Lab_Msg");
            }
        }

        public Label Lab_Note
        {
            get
            {
                return this.Pub1.GetLabelByID("Lab_Note");
            }
        }

        /// <summary>
        /// 属性
        /// </summary>
        public DDL DDL_Attr
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Attr");
            }
        }

        public DDL DDL_Oper
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Oper");
            }
        }

        public DDL DDL_ConnJudgeWay
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_ConnJudgeWay");
            }
        }

        public string GenerMyPK
        {
            get
            {
                return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.Depts.ToString();
            }
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, this.FK_MainNode,
               CondAttr.ToNodeID, this.ToNodeID,
               CondAttr.CondType, (int)this.HisCondType);

            var btn = sender as LinkBtn;

            if (btn.ID == NamesOfBtn.Delete)
            {
                this.Response.Redirect(this.Request.RawUrl, true);
                return;
            }

            cond.MyPK = this.GenerMyPK;

            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Depts;
                cond.NodeID = this.FK_MainNode;
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = this.ToNodeID;
                cond.Insert();
            }

            string val = "";
            Depts sts = new Depts();
            sts.RetrieveAllFromDBSource();

            foreach (Dept st in sts)
            {
                if (this.Pub1.IsExit("CB_" + st.No) == false)
                    continue;
                if (this.Pub1.GetCBByID("CB_" + st.No).Checked)
                    val += "@" + st.No;
            }

            if (val == "")
            {
                cond.Delete();
                return;
            }

            val += "@";
            cond.OperatorValue = val;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = this.HisCondType;
            cond.FK_Node = this.FK_Node;
            cond.ToNodeID = this.ToNodeID;
            
            #region //获取“指定的操作员”设置，added by liuxc,2015-10-7
            cond.SpecOperWay = (SpecOperWay)Pub1.GetDDLByID("DDL_" + CondAttr.SpecOperWay).SelectedItemIntVal;

            if (cond.SpecOperWay != SpecOperWay.CurrOper)
            {
                cond.SpecOperPara = Pub1.GetTBByID("TB_" + CondAttr.SpecOperPara).Text;
            }
            else
            {
                cond.SpecOperPara = string.Empty;
            }
            #endregion

            switch (this.HisCondType)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.Update();
                    this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                case CondType.Dir:
                    cond.ToNodeID = this.ToNodeID;
                    cond.Update();
                    this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                case CondType.SubFlow:
                    cond.ToNodeID = this.ToNodeID;
                    cond.Update();
                    this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                default:
                    throw new Exception("未设计的情况。");
            }

            EasyUiHelper.AddEasyUiMessager(this, "保存成功！");
        }
    }
}