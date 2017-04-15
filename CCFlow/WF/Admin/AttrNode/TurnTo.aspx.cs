using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_TurnTo : BP.Web.WebPage
    {
        #region 属性
        public DDL DDL_Attr
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Attr");
            }
        }
        public string FK_Attr
        {
            get
            {
                return this.Request.QueryString["FK_Attr"];
            }
        }
        public DDL DDL_Node
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Node");
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public int FK_NodeInt
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.FK_Node == null)
            {
                this.BindFlow();
            }
            else
            {
                this.BindNode();
            }
        }
        /// <summary>
        /// 绑定节点
        /// </summary>
        public void BindNode()
        {
            if (this.DoType == "Del")
            {
                TurnTo condDel = new TurnTo();
                condDel.MyPK = this.MyPK;
                condDel.Delete();
                this.Response.Redirect("TurnTo.aspx?FK_Node=" + this.FK_Node, true);
                return;
            }

            BP.WF.Node nd = new BP.WF.Node(this.FK_NodeInt);
            TurnTos conds = new TurnTos();
            conds.Retrieve(TurnToAttr.FK_Node, this.FK_Node);

            TurnTo cond = new TurnTo();
            if (this.MyPK != null)
            {
                cond.MyPK = this.MyPK;
                cond.RetrieveFromDBSources();
                if (this.FK_Attr != null)
                    cond.FK_Attr = this.FK_Attr;
            }
            if (this.FK_Attr != null)
                cond.FK_Attr = this.FK_Attr;

            this.Title = "节点完成后转向条件";
            this.Pub1.AddTable("align=center");
            this.Pub1.AddCaptionLeft("节点完成后转向条件" + nd.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTREnd();

            // 属性/字段
            MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);
            MapAttrs attrNs = new MapAttrs();
            foreach (MapAttr attr in attrs)
            {
                if (attr.IsBigDoc)
                    continue;

                switch (attr.KeyOfEn)
                {
                    case "Title":
                    case "FK_Emp":
                    case "MyNum":
                    case "FK_NY":
                    case WorkAttr.Emps:
                    case WorkAttr.OID:
                    case StartWorkAttr.Rec:
                    case StartWorkAttr.FID:
                        continue;
                    default:
                        break;
                }
                attrNs.AddEntity(attr);
            }

            DDL ddl = new DDL();
            ddl.ID = "DDL_Attr";
            ddl.BindEntities(attrNs, MapAttrAttr.MyPK, MapAttrAttr.Name);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_Node_SelectedIndexChanged);
            ddl.SetSelectItem(cond.FK_Attr);
            if (attrNs.Count == 0)
            {
                BP.WF.Node tempND = new BP.WF.Node(cond.FK_Node);
                nd.CreateMap();
                this.Pub1.AddTR();
                this.Pub1.AddTD("");
                this.Pub1.AddTD("colspan=2", "节点没有找到合适的条件");
                this.Pub1.AddTREnd();
                return;
            }
            this.Pub1.AddTR();
            this.Pub1.AddTD(  "属性/字段");
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("请选择节点表单字段。");
            this.Pub1.AddTREnd();

            MapAttr attrS = new MapAttr(this.DDL_Attr.SelectedItemStringVal);
            this.Pub1.AddTR();
            this.Pub1.AddTD("操作符");
            ddl = new DDL();
            ddl.ID = "DDL_Oper";
            switch (attrS.LGType)
            {
                case BP.En.FieldTypeS.Enum:
                case BP.En.FieldTypeS.FK:
                    ddl.Items.Add(new ListItem("=", "="));
                    break;
                case BP.En.FieldTypeS.Normal:
                    switch (attrS.MyDataType)
                    {
                        case BP.DA.DataType.AppString:
                        case BP.DA.DataType.AppDate:
                        case BP.DA.DataType.AppDateTime:
                            ddl.Items.Add(new ListItem("=", "="));
                            ddl.Items.Add(new ListItem("LIKE", "LIKE"));
                            break;
                        case BP.DA.DataType.AppBoolean:
                            ddl.Items.Add(new ListItem("=", "="));
                            break;
                        default:
                            ddl.Items.Add(new ListItem("=", "="));
                            ddl.Items.Add(new ListItem(">", ">"));
                            ddl.Items.Add(new ListItem(">=", ">="));
                            ddl.Items.Add(new ListItem("<", "<"));
                            ddl.Items.Add(new ListItem("<=", "<="));
                            break;
                    }
                    break;
                default:
                    break;
            }
            ddl.SetSelectItem(cond.FK_Operator.ToString());
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("操作符号");
            this.Pub1.AddTREnd();
            switch (attrS.LGType)
            {
                case BP.En.FieldTypeS.Enum:
                    this.Pub1.AddTR();
                    this.Pub1.AddTD("值");
                    ddl = new DDL();
                    ddl.ID = "DDL_Val";
                    ddl.BindSysEnum(attrS.KeyOfEn);
                    if (cond != null)
                    {
                        try
                        {
                            ddl.SetSelectItem(cond.OperatorValueInt);
                        }
                        catch
                        {
                        }
                    }
                    this.Pub1.AddTD(ddl);
                    this.Pub1.AddTD("");
                    this.Pub1.AddTREnd();
                    break;
                case BP.En.FieldTypeS.FK:
                    this.Pub1.AddTR();
                    this.Pub1.AddTD("值");
                    ddl = new DDL();
                    ddl.ID = "DDL_Val";
                    ddl.BindEntities(attrS.HisEntitiesNoName);
                    if (cond != null)
                    {
                        try
                        {
                            ddl.SetSelectItem(cond.OperatorValueStr);
                        }
                        catch
                        {
                        }
                    }
                    this.Pub1.AddTD(ddl);
                    this.Pub1.AddTD("");
                    this.Pub1.AddTREnd();
                    break;
                default:
                    if (attrS.MyDataType == BP.DA.DataType.AppBoolean)
                    {
                        this.Pub1.AddTR();
                        this.Pub1.AddTD("值");
                        ddl = new DDL();
                        ddl.ID = "DDL_Val";
                        ddl.BindAppYesOrNo(0);
                        if (cond != null)
                        {
                            try
                            {
                                ddl.SetSelectItem(cond.OperatorValueInt);
                            }
                            catch
                            {
                            }
                        }
                        this.Pub1.AddTD(ddl);
                        this.Pub1.AddTD();
                        this.Pub1.AddTREnd();
                    }
                    else
                    {
                        this.Pub1.AddTR();
                        this.Pub1.AddTD("值");
                        TB tb = new TB();
                        tb.ID = "TB_Val";
                        if (cond != null)
                            tb.Text = cond.OperatorValueStr;
                        this.Pub1.AddTD(tb);
                        this.Pub1.AddTD();
                        this.Pub1.AddTREnd();
                    }
                    break;
            }

            this.Pub1.AddTR();
            this.Pub1.AddTD("转向Url");
            TextBox mytb = new TextBox();
            mytb.ID = "TB_TurnToUrl";
            mytb.Text = cond.TurnToURL;
            mytb.Columns = 90;
            this.Pub1.AddTD("colspan=3", mytb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD class=TD colspan=3 align=center>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保 存 ";
            btn.Click += new EventHandler(btn_Save_Node_Click);
            this.Pub1.Add(btn);

            if (cond.IsExits == true)
            {
                Btn btnN = new Btn();
                btnN.ShowType = BP.Web.Controls.BtnType.Confirm;
                btnN.ID = "Btn_Del";
                btnN.Text = " 删 除 ";
                btnN.Click += new EventHandler(btn_Del_Node_Click);
                this.Pub1.Add(btnN);
            }

            this.Pub1.AddBR();
            this.Pub1.AddBR("提示:Url中除系统的参数(FromFlow,FromNode,SID,WebUser.No)外，您还可以增加约定的变量。");
            this.Pub1.AddBR("&nbsp;&nbsp;&nbsp;例如: ../EIP/aaa.aspx?Jiner=@jiner，@jiner为表单字段");
            this.Pub1.AddBR("&nbsp;&nbsp;&nbsp;系统处理后的转向url为: <br>../EIP/aaa.aspx?Jiner=123&UserNo=abc&SID=xxxx&FromFlow=010&FromNode=108。");

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEndWithHR();

            if (conds.Count > 0)
            {
                this.Pub1.AddTable();
                this.Pub1.AddCaption("方向条件列表");

                this.Pub1.AddTR();
                this.Pub1.AddTDTitle("IDX");
                this.Pub1.AddTDTitle("属性键");
                this.Pub1.AddTDTitle("名称");
                this.Pub1.AddTDTitle("操作符号");
                this.Pub1.AddTDTitle("值");
                this.Pub1.AddTDTitle("值描述");
                this.Pub1.AddTDTitle("Url");
                this.Pub1.AddTDTitle("编辑");
                this.Pub1.AddTDTitle("删除");
                this.Pub1.AddTREnd();

                int idx = 0;
                foreach (TurnTo tt in conds)
                {
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD(tt.AttrKey);
                    this.Pub1.AddTD(tt.AttrT);

                    this.Pub1.AddTD(tt.FK_Operator);
                    this.Pub1.AddTD(tt.OperatorValueStr);
                    this.Pub1.AddTD(tt.OperatorValueT);
                    this.Pub1.AddTDBigDoc(tt.TurnToURL);

                    this.Pub1.AddTDA("TurnTo.aspx?MyPK=" + tt.MyPK + "&FK_Node=" + tt.FK_Node, "<img src='../Img/Btn/Edit.gif' />");
                    this.Pub1.AddTDA("TurnTo.aspx?MyPK=" + tt.MyPK + "&FK_Node=" + tt.FK_Node + "&DoType=Del", "<img src='../Img/Btn/Delete.gif' />");

                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
            }
        }
        void ddl_Node_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Response.Redirect("TurnTo.aspx?FK_Node=" + this.FK_Node + "&FK_Attr=" + this.DDL_Attr.SelectedItemStringVal, true);
        }
        public DDL DDL_Oper
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Oper");
            }
        }
        public string GetOperVal
        {
            get
            {
                if (this.Pub1.IsExit("TB_Val"))
                    return this.Pub1.GetTBByID("TB_Val").Text;
                return this.Pub1.GetDDLByID("DDL_Val").SelectedItemStringVal;
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
        void btn_Del_Node_Click(object sender, EventArgs e)
        {
            TurnTo cond = new TurnTo();
            cond.MyPK = this.FK_Node;
            cond.Delete();
            this.Response.Redirect("TurnTo.aspx?FK_Node=" + this.FK_Node, true);
        }
        void btn_Save_Node_Click(object sender, EventArgs e)
        {
            TurnTo cond = new TurnTo();
            BP.WF.Node nd = new BP.WF.Node(this.FK_NodeInt);
            cond.FK_Flow = nd.FK_Flow;
            cond.FK_Node = this.FK_NodeInt;
            cond.FK_Attr = this.DDL_Attr.SelectedItemStringVal;
            cond.FK_Operator = this.DDL_Oper.SelectedItemStringVal;
            cond.OperatorValue = this.GetOperVal;
            cond.OperatorValueT = this.GetOperValText;
            cond.TurnToURL = this.Pub1.GetTextBoxByID("TB_TurnToURL").Text;
            cond.MyPK = this.FK_Node + "_" + this.FK_Attr + "_" + cond.FK_OperatorExt + "_" + cond.OperatorValue;
            cond.Save();
            this.Response.Redirect("TurnTo.aspx?FK_Node=" + this.FK_Node + "&FK_Attr=" + cond.FK_Attr + "&MyPK=" + cond.MyPK, true);
            return;
        }
        /// <summary>
        /// 绑定流程
        /// </summary>
        public void BindFlow()
        {

        }
    }
}