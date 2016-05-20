using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;
using BP.WF;
using BP.WF.Template;
using BP.Port;
using BP.DA;

namespace CCFlow.WF.Admin
{
    public partial class ActionPush2Spec : BP.Web.WebPage
    {
        #region 属性

        public string Event
        {
            get
            {
                return this.Request.QueryString["Event"];
            }
        }

        public string NodeID
        {
            get
            {
                return this.Request.QueryString["NodeID"];
            }
        }

        public string FK_MapData
        {
            get
            {
                return "ND" + this.Request.QueryString["NodeID"];
            }
        }

        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }

        public string ThePushWay
        {
            get
            {
                return this.Request.QueryString["ThePushWay"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            PushMsgs msgs = new PushMsgs(this.FK_Flow);
            var msg = msgs.GetEntityByKey(PushMsgAttr.FK_Event, this.Event, PushMsgAttr.FK_Node, int.Parse(this.NodeID)) as PushMsg;

            if (msg == null)
            {
                msg = new PushMsg();
                msg.FK_Event = this.Event;
                msg.FK_Node = int.Parse(this.NodeID);
            }

            if (!string.IsNullOrWhiteSpace(this.ThePushWay))
            {
                if (this.ThePushWay != msg.PushWay.ToString())
                {
                    msg.PushDoc = string.Empty;
                    msg.Tag = string.Empty;
                }

                msg.PushWay = int.Parse(this.ThePushWay);
            }

            this.Pub1.AddTable("class='Table' cellspacing='1' cellpadding='1' border='1' style='width:100%'");

            this.Pub1.AddTR();
            this.Pub1.AddTD("style='width:100px'", "推送设置方式：");
            var ddl = new DDL();
            ddl.BindSysEnum(PushMsgAttr.PushWay);
            ddl.ID = "DDL_" + PushMsgAttr.PushWay;
            ddl.SetSelectItem((int)msg.PushWay);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTREnd();

            switch ((PushWay)msg.PushWay)
            {
                case PushWay.ByParas:

                    #region 按照系统指定参数

                    Pub1.AddTR();
                    Pub1.AddTD("输入参数名：");
                    Pub1.AddTDBegin();

                    var rad = new RadioBtn();
                    rad.GroupName = "Para";
                    rad.ID = "RB_0";
                    rad.Text = "系统参数";
                    rad.Checked = msg.PushDoc == "0";

                    Pub1.Add(rad);

                    var tb = new TB();
                    tb.ID = "TB_" + PushMsgAttr.Tag;

                    if (msg.PushDoc == "0")
                        tb.Text = msg.Tag;
                    else
                        tb.Text = "NoticeTo";

                    Pub1.Add(tb);

                    Pub1.Add("&nbsp;默认为NoticeTo");
                    Pub1.AddBR();

                    rad = new RadioBtn();
                    rad.GroupName = "Para";
                    rad.ID = "RB_1";
                    rad.Text = "表单字段参数";
                    rad.Checked = msg.PushDoc == "1";

                    Pub1.Add(rad);

                    MapAttrs attrs = new MapAttrs();
                    attrs.Retrieve(MapAttrAttr.FK_MapData, "ND" + this.NodeID);

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

                    ddl = new DDL();
                    ddl.ID = "DDL_" + PushMsgAttr.Tag;
                    ddl.BindEntities(attrNs, MapAttrAttr.MyPK, MapAttrAttr.Name);
                    ddl.AutoPostBack = false;

                    if (msg.PushDoc == "1")
                        ddl.SetSelectItem(msg.Tag);

                    Pub1.Add(ddl);
                    Pub1.AddTREnd();
                    #endregion

                    break;
                case PushWay.NodeWorker:

                    #region 按照指定结点的工作人员

                    Pub1.AddTR();
                    Pub1.AddTDBegin("colspan='2'");

                    Pub1.Add("请选择要推送到的节点工作人员：<br />");
                    Nodes nds = new Nodes(this.FK_Flow);
                    CheckBox cb = null;

                    foreach (BP.WF.Node nd in nds)
                    {
                        if (nd.NodeID == int.Parse(this.NodeID))
                            continue;

                        cb = new CheckBox();
                        cb.ID = "CB_" + nd.NodeID;
                        cb.Text = nd.NodeID + " &nbsp;" + nd.Name;
                        cb.Checked = msg.PushDoc.Contains("@" + nd.NodeID + "@");
                        Pub1.Add(cb);
                        Pub1.AddBR();
                    }

                    Pub1.AddTDEnd();
                    Pub1.AddTREnd();
                    #endregion

                    break;
                case PushWay.SpecDepts:

                    #region 按照指定的部门

                    Pub1.AddTR();
                    Pub1.AddTDBegin("colspan='2'");

                    this.Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
                    this.Pub1.AddTR();
                    this.Pub1.AddTD("colspan='3' class='GroupTitle'", "部门选择");
                    this.Pub1.AddTREnd();

                    //NodeDepts ndepts = new NodeDepts(int.Parse(this.NodeID));
                    Depts depts = new Depts();
                    depts.RetrieveAll();
                    int i = 0;

                    //foreach (NodeDept dept in ndepts)
                    foreach (Dept dept in depts)
                    {
                        i++;

                        if (i == 4)
                            i = 1;

                        if (i == 1)
                            Pub1.AddTR();

                        cb = new CheckBox();
                        //cb.ID = "CB_" + dept.FK_Dept;
                        //cb.Text = (depts.GetEntityByKey(dept.FK_Dept) as Dept).Name;
                        cb.ID = "CB_" + dept.No;
                        cb.Text = dept.Name;

                        //if (msg.PushDoc.Contains("@" + dept.FK_Dept + "@"))
                        if (msg.PushDoc.Contains("@" + dept.No + "@"))
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
                    Pub1.AddTDEnd();
                    Pub1.AddTREnd();
                    #endregion

                    break;
                case PushWay.SpecEmps:

                    #region 按照指定的人员

                    Pub1.AddTR();
                    //Pub1.AddTDBegin("colspan='2'");

                    Pub1.AddTD("选择人员：");
                    Pub1.AddTDBegin();

                    tb = new TB();
                    tb.ID = "TB_Users";
                    tb.TextMode = TextBoxMode.MultiLine;
                    tb.Style.Add("width", "99%");
                    tb.Rows = 4;
                    tb.ReadOnly = true;

                    var hf = new HiddenField();
                    hf.ID = "HID_Users";

                    //加载已经选择的人员
                    if (!string.IsNullOrWhiteSpace(msg.PushDoc))
                    {
                        hf.Value = msg.PushDoc.Replace("@@", ",").Trim('@');

                        var emps = new Emps();
                        emps.RetrieveAll();

                        tb.Text =
                            hf.Value.Split(',').Select(o => (emps.GetEntityByKey(o) as Emp).Name).Aggregate(
                                string.Empty, (curr, next) => curr + next + ",").TrimEnd(',');
                    }

                    Pub1.Add(tb);
                    Pub1.Add(hf);
                    Pub1.AddBR();
                    Pub1.AddBR();

                    Pub1.Add(
                        "<a class='easyui-linkbutton' data-options=\"iconCls:'icon-user'\" href='javascript:void(0)' onclick=\"showWin('../Comm/Port/SelectUser_Jq.aspx','" +
                        tb.ClientID + "','" + hf.ClientID + "');\">选择人员...</a>");
                    Pub1.AddTDEnd();
                    //Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
                    //depts = new Depts();
                    //depts.RetrieveAll();
                    //var emps = new Emps();
                    //emps.RetrieveAll();
                    //var empDepts = new EmpDepts();
                    //empDepts.RetrieveAll();
                    //var nemps = new NodeEmps(int.Parse(this.NodeID));

                    //Emp emp = null;

                    //foreach (Dept dept in depts)
                    //{
                    //    this.Pub1.AddTR();
                    //    var mycb = new CheckBox();
                    //    mycb.Text = dept.Name;
                    //    mycb.ID = "CB_D_" + dept.No;
                    //    this.Pub1.AddTD("colspan='3' class='GroupTitle'", mycb);
                    //    this.Pub1.AddTREnd();

                    //    i = 0;
                    //    string ctlIDs = "";

                    //    foreach (EmpDept ed in empDepts)
                    //    {
                    //        if (ed.FK_Dept != dept.No)
                    //            continue;

                    //        //排除非当前结点绑定的人员
                    //        if (nemps.GetEntityByKey(NodeEmpAttr.FK_Emp, ed.FK_Emp) == null)
                    //            continue;

                    //        i++;

                    //        if (i == 4)
                    //            i = 1;

                    //        if (i == 1)
                    //            Pub1.AddTR();

                    //        emp = emps.GetEntityByKey(ed.FK_Emp) as Emp;

                    //        cb = new CheckBox();
                    //        cb.ID = "CB_E_" + emp.No;
                    //        ctlIDs += cb.ID + ",";
                    //        cb.Text = emp.Name;
                    //        if (msg.PushDoc.Contains("@" + emp.No + "@"))
                    //            cb.Checked = true;

                    //        Pub1.AddTD(cb);

                    //        if (i == 3)
                    //            Pub1.AddTREnd();
                    //    }

                    //    mycb.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                    //    switch (i)
                    //    {
                    //        case 1:
                    //            Pub1.AddTD();
                    //            Pub1.AddTD();
                    //            Pub1.AddTREnd();
                    //            break;
                    //        case 2:
                    //            Pub1.AddTD();
                    //            Pub1.AddTREnd();
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}

                    //Pub1.AddTableEnd();

                    //Pub1.AddTDEnd();
                    Pub1.AddTREnd();
                    #endregion

                    break;
                case PushWay.SpecSQL:

                    #region 按照指定的SQL查询语句

                    Pub1.AddTR();

                    this.Pub1.AddTDBegin("colspan='2'");
                    this.Pub1.Add("SQL查询语句：<br />");
                    tb = new TB();
                    tb.ID = "TB_" + PushMsgAttr.PushDoc;
                    tb.Columns = 50;
                    tb.Style.Add("width", "99%");
                    tb.TextMode = TextBoxMode.MultiLine;
                    tb.Rows = 4;
                    tb.Text = msg.PushDoc;
                    this.Pub1.Add(tb);
                    this.Pub1.AddTDEnd();
                    Pub1.AddTREnd();
                    #endregion

                    break;
                case PushWay.SpecStations:

                    #region 按照指定的岗位

                    Pub1.AddTR();
                    Pub1.AddTDBegin("colspan='2'");

                    /*BPM 模式*/
                    BP.GPM.StationTypes tps = new BP.GPM.StationTypes();
                    tps.RetrieveAll();

                    BP.GPM.Stations sts = new BP.GPM.Stations();
                    sts.RetrieveAll();

                    string sql = "SELECT No,Name FROM Port_Station WHERE FK_StationType NOT IN (SELECT No FROM Port_StationType)";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count != 0)
                    {
                        if (tps.Count == 0)
                        {
                            var stp = new BP.GPM.StationType { No = "01", Name = "普通岗" };
                            stp.Save();

                            tps.AddEntity(stp);
                        }

                        //更新所有对不上岗位类型的岗位，岗位类型为01或第一个
                        foreach (BP.GPM.Station st in sts)
                        {
                            st.FK_StationType = tps[0].No;
                            st.Update();
                        }
                    }

                    this.Pub1.AddTable("class='Table' cellSpacing='0' cellPadding='0'  border='0' style='width:100%'");

                    foreach (BP.GPM.StationType tp in tps)
                    {
                        this.Pub1.AddTR();
                        CheckBox mycb = new CheckBox();
                        mycb.Text = tp.Name;
                        mycb.ID = "CB_ST_" + tp.No;
                        this.Pub1.AddTD("colspan='3' class='GroupTitle'", mycb);
                        this.Pub1.AddTREnd();

                        i = 0;
                        string ctlIDs = "";

                        foreach (BP.GPM.Station st in sts)
                        {
                            if (st.FK_StationType != tp.No)
                                continue;

                            i++;

                            if (i == 4)
                                i = 1;

                            if (i == 1)
                            {
                                Pub1.AddTR();
                            }

                            cb = new CheckBox();
                            cb.ID = "CB_S_" + st.No;
                            ctlIDs += cb.ID + ",";
                            cb.Text = st.Name;

                            if (msg.PushDoc.Contains("@" + st.No + "@"))
                                cb.Checked = true;

                            this.Pub1.AddTD(cb);

                            if (i == 3)
                                Pub1.AddTREnd();
                        }

                        mycb.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

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
                    }

                    this.Pub1.AddTableEnd();



                    #region 原逻辑，只考虑了一种模式，停用
                    //Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
                    //SysEnums ses = new SysEnums("StaGrade");
                    //Stations sts = new Stations();
                    //NodeStations nsts = new NodeStations(int.Parse(this.NodeID));
                    //sts.RetrieveAll();

                    //foreach (SysEnum se in ses)
                    //{
                    //    this.Pub1.AddTR();
                    //    var mycb = new CheckBox();
                    //    mycb.Text = se.Lab;
                    //    mycb.ID = "CB_SG_" + se.IntKey;
                    //    this.Pub1.AddTD("colspan=3 class='GroupTitle'", mycb);
                    //    this.Pub1.AddTREnd();

                    //    i = 0;
                    //    string ctlIDs = "";

                    //    foreach (Station st in sts)
                    //    {
                    //        if (st.StaGrade != se.IntKey)
                    //            continue;

                    //        //排除非当前结点的岗位
                    //        if (nsts.GetEntityByKey(NodeStationAttr.FK_Station, st.No) == null)
                    //            continue;

                    //        i++;

                    //        if (i == 4)
                    //            i = 1;

                    //        if (i == 1)
                    //            Pub1.AddTR();

                    //        cb = new CheckBox();
                    //        cb.ID = "CB_S_" + st.No;
                    //        ctlIDs += cb.ID + ",";
                    //        cb.Text = st.Name;
                    //        if (msg.PushDoc.Contains("@" + st.No + "@"))
                    //            cb.Checked = true;

                    //        Pub1.AddTD(cb);

                    //        if (i == 3)
                    //            Pub1.AddTREnd();
                    //    }

                    //    mycb.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                    //    switch (i)
                    //    {
                    //        case 1:
                    //            Pub1.AddTD();
                    //            Pub1.AddTD();
                    //            Pub1.AddTREnd();
                    //            break;
                    //        case 2:
                    //            Pub1.AddTD();
                    //            Pub1.AddTREnd();
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                    //Pub1.AddTableEnd();
                    #endregion

                    Pub1.AddTDEnd();
                    Pub1.AddTREnd();
                    #endregion

                    break;
            }

            Pub1.AddTableEnd();

            Pub1.AddBR();
            Pub1.AddSpace(1);

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Click);
            Pub1.Add(btn);

            if (!string.IsNullOrWhiteSpace(msg.MyPK))
            {
                Pub1.AddSpace(1);
                btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
                btn.Click += new EventHandler(btn_Delete_Click);
                btn.Attributes["onclick"] = "return confirm('你确定要删除此消息推送设置吗？');";
                Pub1.Add(btn);
            }
        }

        void btn_Delete_Click(object sender, EventArgs e)
        {
            var pm = new PushMsg();
            pm.Retrieve(PushMsgAttr.FK_Event, this.Event, PushMsgAttr.FK_Node, this.NodeID);
            pm.Delete();

            Response.Redirect(string.Format("ActionPush2Spec.aspx?NodeID={0}&MyPK={1}&Event={2}&FK_Flow={3}&tk={4}", NodeID, MyPK, Event,
                                            FK_Flow, new Random().NextDouble()), true);
        }

        void btn_Click(object sender, EventArgs e)
        {
            var pm = new PushMsg();
            pm.Retrieve(PushMsgAttr.FK_Event, this.Event, PushMsgAttr.FK_Node, this.NodeID);

            if (!string.IsNullOrWhiteSpace(pm.MyPK))
                pm.Delete();

            pm.FK_Event = this.Event;
            pm.FK_Node = int.Parse(this.NodeID);

            var ddl = Pub1.GetDDLByID("DDL_" + PushMsgAttr.PushWay);
            pm.PushWay = ddl.SelectedItemIntVal;
            pm.PushDoc = string.Empty;

            switch ((PushWay)pm.PushWay)
            {
                case PushWay.ByParas:

                    #region 按照系统指定参数

                    var rb = Pub1.GetRadioBtnByID("RB_0");

                    if (rb.Checked)
                    {
                        pm.PushDoc = "0";
                        pm.Tag = Pub1.GetTBByID("TB_" + PushMsgAttr.Tag).Text;
                    }
                    else
                    {
                        rb = Pub1.GetRadioBtnByID("RB_1");

                        if (rb.Checked)
                            pm.PushDoc = "1";

                        pm.Tag = Pub1.GetDDLByID("DDL_" + PushMsgAttr.Tag).SelectedItemStringVal;
                    }

                    #endregion

                    break;
                case PushWay.NodeWorker:

                    #region 按照指定结点的工作人员

                    CheckBox cb = null;

                    foreach (var ctrl in Pub1.Controls)
                    {
                        cb = ctrl as CheckBox;
                        if (cb == null || !cb.ID.StartsWith("CB_") || !cb.Checked) continue;

                        pm.PushDoc += "@" + cb.ID.Substring(3) + "@";
                    }

                    #endregion

                    break;
                case PushWay.SpecDepts:

                    #region 按照指定的部门

                    foreach (var ctrl in Pub1.Controls)
                    {
                        cb = ctrl as CheckBox;
                        if (cb == null || !cb.ID.StartsWith("CB_") || !cb.Checked) continue;

                        pm.PushDoc += "@" + cb.ID.Substring(3) + "@";
                    }

                    #endregion

                    break;
                case PushWay.SpecEmps:

                    #region 按照指定的人员

                    var hid = Pub1.FindControl("HID_Users") as HiddenField;

                    if(!string.IsNullOrWhiteSpace(hid.Value))
                    {
                        pm.PushDoc = hid.Value.Split(',').Select(o => "@" + o + "@").Aggregate(string.Empty,
                                                                                               (curr, next) =>
                                                                                               curr + next);
                    }
                    //foreach (var ctrl in Pub1.Controls)
                    //{
                    //    cb = ctrl as CheckBox;
                    //    if (cb == null || !cb.ID.StartsWith("CB_E_") || !cb.Checked) continue;

                    //    pm.PushDoc += "@" + cb.ID.Substring(5) + "@";
                    //}

                    #endregion

                    break;
                case PushWay.SpecSQL:

                    #region 按照指定的SQL查询语句

                    pm.PushDoc = Pub1.GetTBByID("TB_" + PushMsgAttr.PushDoc).Text;

                    #endregion

                    break;
                case PushWay.SpecStations:

                    #region 按照指定的岗位

                    foreach (var ctrl in Pub1.Controls)
                    {
                        cb = ctrl as CheckBox;
                        if (cb == null || !cb.ID.StartsWith("CB_S_") || !cb.Checked) continue;

                        pm.PushDoc += "@" + cb.ID.Substring(5) + "@";
                    }

                    #endregion

                    break;
            }

            pm.Save();
            Response.Redirect(string.Format("ActionPush2Spec.aspx?NodeID={0}&MyPK={1}&Event={2}&FK_Flow={3}&tk={4}", NodeID, MyPK, Event,
                                            FK_Flow, new Random().NextDouble()), true);
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("ActionPush2Spec.aspx?NodeID={0}&MyPK={1}&Event={2}&FK_Flow={3}&ThePushWay={4}&tk={5}", NodeID, MyPK, Event,
                                            FK_Flow, (sender as DDL).SelectedItemIntVal, new Random().NextDouble()), true);
        }
    }
}