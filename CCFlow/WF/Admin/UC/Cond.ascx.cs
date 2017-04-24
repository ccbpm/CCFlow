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
using CCFlow.WF.Admin.UC;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_UC_Cond : BP.Web.UC.UCBase3
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
        public string FK_Attr
        {
            get
            {
                string s = this.Request.QueryString["FK_Attr"];
                if (s == null || s == "")
                    s = ViewState["FK_Attr"] as string;

                if (s == null || s == "")
                {
                    try
                    {
                        s = this.DDL_Attr.SelectedItemStringVal;
                    }
                    catch
                    {
                        return null;
                    }
                }
                if (s == "")
                    return null;
                return s;
            }
            set
            {
                ViewState["FK_Attr"] = value;
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
        public string GetOperVal
        {
            get
            {
                if (this.IsExit("TB_Val"))
                    return this.GetTBByID("TB_Val").Text;
                return this.GetDDLByID("DDL_Val").SelectedItemStringVal;
            }
        }
        public string GetOperValText
        {
            get
            {
                if (this.IsExit("TB_Val"))
                    return this.GetTBByID("TB_Val").Text;
                return this.GetDDLByID("DDL_Val").SelectedItem.Text;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["DoType"] == "Del")
            {
                Cond nd = new Cond(this.MyPK);
                nd.Delete();
                this.Response.Redirect("Cond.htm?CondType=" + (int)this.HisCondType + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + nd.NodeID + "&FK_Node=" + this.FK_MainNode + "&ToNodeID=" + nd.ToNodeID, true);
                return;
            }

            this.BindCond();
            if (this.FK_Attr == null)
                this.FK_Attr = this.DDL_Attr.SelectedItemStringVal;
        }
        public void BindCond()
        {
            int idx = 0;

            Cond cond = new Cond();
            cond.MyPK = this.MyPK;
            if (cond.RetrieveFromDBSources() == 0)
            {
                if (this.FK_Attr != null)
                    cond.FK_Attr = this.FK_Attr;
                if (this.FK_MainNode != 0)
                    cond.NodeID = this.FK_MainNode;
                if (this.FK_Node != 0)
                    cond.FK_Node = this.FK_Node;
                if (this.FK_Flow != null)
                    cond.FK_Flow = this.FK_Flow;
            }
            //this.AddTable("border=0 widht='500px'");
            this.AddTable("class='Table' cellpadding='2' cellspacing='2' style='width:100%;'");
            this.AddCaptionMsg("流程完成条件");
            this.AddTR();
            this.AddTH("序");
            this.AddTH("项目");
            this.AddTH("采集");
            this.AddTH("描述");
            this.AddTREnd();

            this.AddTR();
            this.AddTDIdx(idx++);
            this.AddTD("节点");
            Nodes nds = new Nodes(cond.FK_Flow);
            Nodes ndsN = new Nodes();
            foreach (BP.WF.Node mynd in nds)
            {
                ndsN.AddEntity(mynd);
            }
            DDL ddl = new DDL();
            ddl.ID = "DDL_Node";
            ddl.BindEntities(ndsN, "NodeID", "Name");
            ddl.SetSelectItem(cond.FK_Node);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            this.AddTD(ddl);
            this.AddTD("节点");
            this.AddTREnd();

            // 属性/字段
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, "ND" + ddl.SelectedItemStringVal);

            MapAttrs attrNs = new MapAttrs();
            foreach (MapAttr attr in attrs)
            {
                if (attr.IsBigDoc)
                    continue;

                switch (attr.KeyOfEn)
                {
                    case "Title":
                    //case "RDT":
                    //case "CDT":
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
            ddl = new DDL();
            ddl.ID = "DDL_Attr";
            if (attrNs.Count == 0)
            {
                BP.WF.Node nd = new BP.WF.Node(cond.FK_Node);
                nd.RepareMap();
                this.AddTR();
                this.AddTDIdx(idx++);
                this.AddTD("");
                this.AddTD("colspan=2", "节点没有找到合适的条件");
                this.AddTREnd();
                this.AddTableEnd();
                return;
            }
            else
            {
                ddl.BindEntities(attrNs, MapAttrAttr.MyPK, MapAttrAttr.Name);
                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
                ddl.SetSelectItem(cond.FK_Attr);
            }

            this.AddTR();
            this.AddTDIdx(idx++);
            this.AddTD("属性/字段");
            this.AddTD(ddl);
            this.AddTD("");
            this.AddTREnd();

            MapAttr attrS = new MapAttr(this.DDL_Attr.SelectedItemStringVal);
            this.AddTR();
            this.AddTDIdx(idx++);
            this.AddTD("操作符");
            ddl = new DDL();
            ddl.ID = "DDL_Oper";
            switch (attrS.LGType)
            {
                case BP.En.FieldTypeS.Enum:
                case BP.En.FieldTypeS.FK:
                    ddl.Items.Add(new ListItem("=", "="));
                    ddl.Items.Add(new ListItem("<>", "<>"));
                    break;
                case BP.En.FieldTypeS.Normal:
                    switch (attrS.MyDataType)
                    {
                        case BP.DA.DataType.AppString:
                        case BP.DA.DataType.AppDate:
                        case BP.DA.DataType.AppDateTime:
                            ddl.Items.Add(new ListItem("=", "="));
                            ddl.Items.Add(new ListItem("LIKE", "LIKE"));
                            ddl.Items.Add(new ListItem("<>", "<>"));
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
                            ddl.Items.Add(new ListItem("<>", "<>"));
                            break;
                    }
                    break;
                default:
                    break;
            }

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
            this.AddTD(ddl);
            this.AddTD("");
            this.AddTREnd();
            switch (attrS.LGType)
            {
                case BP.En.FieldTypeS.Enum:
                    this.AddTR();
                    this.AddTDIdx(idx++);
                    this.AddTD("值");
                    ddl = new DDL();
                    ddl.ID = "DDL_Val";
                    ddl.BindSysEnum(attrS.UIBindKey);
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
                    this.AddTD(ddl);
                    this.AddTD("");
                    this.AddTREnd();
                    break;
                case BP.En.FieldTypeS.FK:
                    this.AddTR();
                    this.AddTDIdx(idx++);
                    this.AddTD("值");

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
                    this.AddTD(ddl);
                    this.AddTD("");
                    this.AddTREnd();
                    break;
                default:
                    if (attrS.MyDataType == BP.DA.DataType.AppBoolean)
                    {
                        this.AddTR();
                        this.AddTDIdx(idx++);
                        this.AddTD("值");
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
                        this.AddTD(ddl);
                        this.AddTD();
                        this.AddTREnd();
                    }
                    else
                    {
                        this.AddTR();
                        this.AddTDIdx(idx++);
                        this.AddTD("值");
                        TB tb = new TB();
                        tb.ID = "TB_Val";
                        if (cond != null)
                            tb.Text = cond.OperatorValueStr;
                        this.AddTD(tb);
                        this.AddTD();
                        this.AddTREnd();
                    }
                    break;
            }


            Conds conds = new Conds();
            QueryObject qo = new QueryObject(conds);
            qo.AddWhere(CondAttr.NodeID, this.FK_MainNode);
            qo.addAnd();
            qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.Form);
            qo.addAnd();
            qo.AddWhere(CondAttr.CondType, (int)this.HisCondType);

            if (this.ToNodeID != 0)
            {
                qo.addAnd();
                qo.AddWhere(CondAttr.ToNodeID, this.ToNodeID);
            }
            int num = qo.DoQuery();

            this.AddTableEnd();
            this.AddBR();
            this.AddSpace(1);

            var btn = new LinkBtn(false, "Btn_SaveAnd", "保存为And条件");
            btn.SetDataOption("iconCls", "'icon-save'");
            btn.Click += new EventHandler(btn_Save_Click);
            this.Add(btn);
            this.AddSpace(1);

            btn = new LinkBtn(false, "Btn_SaveOr", "保存为Or条件");
            btn.SetDataOption("iconCls", "'icon-save'");
            btn.Click += new EventHandler(btn_Save_Click);
            this.Add(btn);
            this.AddSpace(1);

            btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
            btn.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Add(btn);

            this.AddBR();
            this.AddBR();

            if (num == 0)
                return;

            #region 条件
            this.AddTable("class='Table' cellpadding='2' cellspacing='2' style='width:100%;'");
            this.AddTR();
            this.AddTDTitleGroup("序");
            this.AddTDTitleGroup("节点");
            this.AddTDTitleGroup("字段的英文名");
            this.AddTDTitleGroup("字段的中文名");
            this.AddTDTitleGroup("操作符");
            this.AddTDTitleGroup("值");
            this.AddTDTitleGroup("标签");
            this.AddTDTitleGroup("运算关系");
            this.AddTDTitleGroup("操作");
            this.AddTREnd();

            int i = 0;
            foreach (Cond mync in conds)
            {
                if (mync.HisDataFrom != ConnDataFrom.Form)
                    continue;

                i++;

                this.AddTR();
                this.AddTDIdx(i);
                //  this.AddTD(mync.HisDataFrom.ToString());
                this.AddTD(mync.FK_NodeT);
                this.AddTD(mync.AttrKey);
                this.AddTD(mync.AttrName);
                this.AddTDCenter(mync.FK_Operator);
                this.AddTD(mync.OperatorValueStr);
                this.AddTD(mync.OperatorValueT);

                if (mync.CondOrAnd == CondOrAnd.ByAnd)
                    this.AddTD("AND");
                else
                    this.AddTD("OR");

                //if (num > 1)
                //    this.AddTD(mync.HisConnJudgeWayT);
                this.AddTD("<a href='Cond.aspx?MyPK=" + mync.MyPK + "&CondType=" + (int)this.HisCondType + "&FK_Flow=" + this.FK_Flow + "&FK_Attr=" + mync.FK_Attr + "&FK_MainNode=" + mync.NodeID + "&OperatorValue=" + mync.OperatorValueStr + "&FK_Node=" + mync.FK_Node + "&DoType=Del&ToNodeID=" + mync.ToNodeID + "' class='easyui-linkbutton' data-options=\"iconCls:'icon-remove'\" onclick=\"return confirm('确定删除此条件吗?')\">删除</a>");
                this.AddTREnd();
            }
            this.AddTableEnd();
            this.AddBR();

            AddEasyUiPanelInfo("说明","在上面的条件集合中ccflow仅仅支持要么是And,要么是OR的两种情形,高级的开发就需要事件来支持条件转向,或者采用其他的方式。");

            #endregion
        }
        public DDL DDL_Node
        {
            get
            {
                return this.GetDDLByID("DDL_Node");
            }
        }
        public Label Lab_Msg
        {
            get
            {
                return this.GetLabelByID("Lab_Msg");
            }
        }
        public Label Lab_Note
        {
            get
            {
                return this.GetLabelByID("Lab_Note");
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public DDL DDL_Attr
        {
            get
            {
                return this.GetDDLByID("DDL_Attr");
            }
        }
        public DDL DDL_Oper
        {
            get
            {
                return this.GetDDLByID("DDL_Oper");
            }
        }
        public DDL DDL_ConnJudgeWay
        {
            get
            {
                return this.GetDDLByID("DDL_ConnJudgeWay");
            }
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Response.Redirect("Cond.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.DDL_Node.SelectedItemStringVal + "&FK_MainNode=" + this.FK_MainNode + "&CondType=" + (int)this.HisCondType + "&FK_Attr=" + this.DDL_Attr.SelectedItemStringVal + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            LinkBtn btn = sender as LinkBtn;
            if (btn.ID == NamesOfBtn.Delete)
            {
                DBAccess.RunSQL("DELETE FROM WF_Cond WHERE  ToNodeID=" + this.ToNodeID + " AND DataFrom=" + (int)ConnDataFrom.Form);
                this.Response.Redirect(this.Request.RawUrl, true);
                return;
            }

            string oper = this.DDL_Oper.SelectedItemStringVal;
            string val = this.GetOperVal;

            if (val == null)
                val = "";

            if (val == "" && (oper != "=" || oper != "<>" || oper != "!="))
            {
                this.Alert("您没有设置条件，请在值文本框中输入值。");
                return;
            }

            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE (" + CondAttr.NodeID + "=" + this.FK_Node + "  AND ToNodeID=" + this.ToNodeID + ") AND DataFrom!=" + (int)ConnDataFrom.Form);

            Cond cond = new Cond();
            cond.HisDataFrom = ConnDataFrom.Form;
            cond.NodeID = this.FK_MainNode;
            cond.ToNodeID = this.FK_MainNode;

            cond.FK_Attr = this.FK_Attr;
            cond.FK_Node = this.DDL_Node.SelectedItemIntVal;
            cond.FK_Operator = this.DDL_Oper.SelectedItemStringVal;
            cond.OperatorValue = this.GetOperVal;
            cond.OperatorValueT = this.GetOperValText;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = this.HisCondType;

            if (btn.ID == "Btn_SaveAnd")
                cond.CondOrAnd = CondOrAnd.ByAnd;
            else
                cond.CondOrAnd = CondOrAnd.ByOr;


            #region 方向条件，全部更新.
            Conds conds = new Conds();
            QueryObject qo = new QueryObject(conds);
            qo.AddWhere(CondAttr.NodeID, this.FK_MainNode);
            qo.addAnd();
            qo.AddWhere(CondAttr.DataFrom, (int)ConnDataFrom.Form);
            qo.addAnd();
            qo.AddWhere(CondAttr.CondType, (int)this.HisCondType);

            if (this.ToNodeID != 0)
            {
                qo.addAnd();
                qo.AddWhere(CondAttr.ToNodeID, this.ToNodeID);
            }
            int num = qo.DoQuery();
            foreach (Cond item in conds)
            {
                item.CondOrAnd = cond.CondOrAnd;
                item.Update();
            }
            #endregion

            /* 执行同步*/
            string sqls = "UPDATE WF_Node SET IsCCFlow=0";
            sqls += "@UPDATE WF_Node  SET IsCCFlow=1 WHERE NodeID IN (SELECT NODEID FROM WF_Cond a WHERE a.NodeID= NodeID AND CondType=1 )";
            BP.DA.DBAccess.RunSQLs(sqls);

            string sql = "UPDATE WF_Cond SET DataFrom=" + (int)ConnDataFrom.Form + " WHERE NodeID=" + cond.NodeID + "  AND FK_Node=" + cond.FK_Node + " AND ToNodeID=" + this.ToNodeID;
            switch (this.HisCondType)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.MyPK = BP.DA.DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.Insert();
                    BP.DA.DBAccess.RunSQL(sql);
                    this.Response.Redirect("Cond.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                case CondType.Dir:
                    // cond.MyPK = cond.NodeID +"_"+ this.Request.QueryString["ToNodeID"]+"_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.MyPK = BP.DA.DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = this.ToNodeID;
                    cond.Insert();
                    BP.DA.DBAccess.RunSQL(sql);
                    this.Response.Redirect("Cond.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                case CondType.SubFlow: //启动子流程.
                    // cond.MyPK = cond.NodeID +"_"+ this.Request.QueryString["ToNodeID"]+"_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.MyPK = BP.DA.DBAccess.GenerOID().ToString();   //cond.NodeID + "_" + cond.FK_Node + "_" + cond.FK_Attr + "_" + cond.OperatorValue;
                    cond.ToNodeID = this.ToNodeID;
                    cond.Insert();
                    BP.DA.DBAccess.RunSQL(sql);
                    this.Response.Redirect("Cond.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                default:
                    throw new Exception("未设计的情况。"+this.HisCondType.ToString());
            }
            EasyUiHelper.AddEasyUiMessager(this, "保存成功！");
        }
    }
}