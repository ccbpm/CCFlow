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
using BP.WF;
using BP.En;
using BP.Sys;

namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_CopyFieldFromNode : BP.Web.WebPage
    {
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string GroupField
        {
            get
            {
                return this.Request.QueryString["GroupField"];
            }
        }
        public string NodeOfSelect
        {
            get
            {
                string s = this.Request.QueryString["NodeOfSelect"];
                if (s == null)
                {
                    Node nd = new Node(this.FK_Node);
                    int fid = int.Parse(nd.FK_Flow);
                    return "ND" + fid + "01";
                    //return this.FK_Node.Substring(0, this.FK_Node.Length - 1) + "01";
                }
                return s;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title =  "字段复制";

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);

            Node sNd = new Node(this.NodeOfSelect);

            MapAttrs attrs = new MapAttrs(this.NodeOfSelect);
            MapAttrs attrsCopy = new MapAttrs(this.FK_Node);


            this.Pub1.AddFieldSet("选择节点");
            this.Pub1.AddUL("Main");
            foreach (BP.WF.Node en in nds)
            {
                if (this.NodeOfSelect == en.NodeID.ToString())
                    this.Pub1.AddLiB("CopyFieldFromNode.aspx?FK_Node=" + this.FK_Node + "&NodeOfSelect=ND" + en.NodeID, "步骤:" + en.Step + ",名称:" + en.Name);
                else
                    this.Pub1.AddLi("CopyFieldFromNode.aspx?FK_Node=" + this.FK_Node + "&NodeOfSelect=ND" + en.NodeID, "步骤:" + en.Step + ",名称:" + en.Name);
            }
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();

            this.Pub2.AddTable("width='500px'");

            //this.Pub2.AddTR();
            //this.Pub2.AddTDTitle(this.ToE("Field", "字段"));
            ////this.Pub2.AddTDTitle("描述");
            ////this.Pub2.AddTDTitle("类型");
            ////this.Pub2.AddTDTitle(this.ToE("Show", "显示"));
            //this.Pub2.AddTREnd();

            GroupFields gfs = new GroupFields(this.NodeOfSelect);
            MapDtls dtls = new MapDtls(this.NodeOfSelect);
            MapM2Ms m2ms = new MapM2Ms(this.NodeOfSelect);
            MapFrames frms = new MapFrames(this.NodeOfSelect);

            bool isHave = false;
            foreach (GroupField gf in gfs)
            {
                this.Pub2.AddTRSum();
                CheckBox cb = new CheckBox();
                cb.ID = "CB" + gf.OID;
                cb.Text = "<b>" + gf.Lab + "</b>";
                this.Pub2.AddTD(cb);
                this.Pub2.AddTREnd();


                foreach (MapM2M m2m in m2ms)
                {
                    if (m2m.GroupID != gf.OID)
                        continue;
                    this.Pub2.AddTR();
                    cb = new CheckBox();
                    cb.ID = "CB" + m2m.MyPK + "_" + m2m.GroupID;
                    cb.Text = "多选" + ":" + m2m.Name;
                    this.Pub2.AddTD(cb);
                    this.Pub2.AddTREnd();
                }

                foreach (MapFrame frm in frms)
                {
                    if (frm.GroupID != gf.OID)
                        continue;
                    this.Pub2.AddTR();
                    cb = new CheckBox();
                    cb.ID = "CB" + frm.MyPK + "_" + frm.GroupID;
                    cb.Text = "框架" + ":" + frm.Name;
                    this.Pub2.AddTD(cb);
                    this.Pub2.AddTREnd();
                }


                this.Pub2.AddTR();
                this.Pub2.AddTDBigDocBegain();
                foreach (MapAttr attr in attrs)
                {
                    if (gf.OID != attr.GroupID)
                        continue;

                    switch (attr.KeyOfEn)
                    {
                        case BP.WF.GEStartWorkAttr.CDT:
                        case BP.WF.GEStartWorkAttr.Emps:
                        case BP.WF.GEStartWorkAttr.FID:
                        case BP.WF.GEStartWorkAttr.OID:
                        case BP.WF.GEStartWorkAttr.RDT:
                        case BP.WF.GEStartWorkAttr.Rec:
                        case BP.WF.GEStartWorkAttr.FK_NY:
                        case BP.WF.GEStartWorkAttr.WFState:
                        case BP.WF.GEStartWorkAttr.MyNum:
                        case BP.WF.GEStartWorkAttr.FK_Dept:
                        case BP.WF.GEStartWorkAttr.FK_DeptText:
                            continue;
                        default:
                            break;
                    }

                    cb = new CheckBox();
                    cb.ID = attr.KeyOfEn;
                    cb.Text = attr.KeyOfEn + " " + attr.Name;

                    if (attrsCopy.Contains(MapAttrAttr.KeyOfEn, attr.KeyOfEn))
                        cb.Enabled = false;
                    else
                        cb.Enabled = true;

                    isHave = true;
                    this.Pub2.Add(cb);
                }
                this.Pub2.AddTDEnd();
                this.Pub2.AddTREnd();
            }

            this.Pub2.AddTableEndWithBR();
            Button btn = new Button();
            btn.CssClass = "Btn";
            if (isHave == false)
            {
                return;
            }
            this.Pub2.Add("到分组:");
            gfs = new GroupFields(this.FK_Node);
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_GroupField";
            ddl.Bind(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab);
            ddl.SetSelectItem(this.GroupField);
            this.Pub2.Add(ddl);
            btn.ID = "Btn_OK";
            btn.Text =  " 复制 ";

            btn.UseSubmitBehavior = false;
            btn.OnClientClick = "this.disabled=true;";
            //this.disabled='disabled'; return true;";
            // btn.Attributes["onclick"] = " return confirm('您确定要复制选择的字段到 [" + nd.Name + "]表单中吗？');";
            // btn.Attributes["onclick"] = " return confirm('" + this.ToE("AYS", "您确定要复制选择的字段到 [" + nd.Name + "]表单中吗？") + "');";

            btn.Click += new EventHandler(btn_Copy_Click);
            this.Pub2.Add(btn);
        }
        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Response.Redirect("CopyFieldFromNode.aspx?FK_Node=" + this.FK_Node + "&NodeOfSelect=" + this.Pub2.GetDDLByID("DDL1").SelectedItemStringVal);
        }

        void btn_Copy_Click(object sender, EventArgs e)
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Node sNd = new Node(this.NodeOfSelect);
            BP.En.Attrs attrs = sNd.HisWork.EnMap.Attrs;
            BP.En.Attrs attrsCopy = nd.HisWork.EnMap.Attrs;

            // 开始copy 分组的节点。
            GroupFields gfs = new GroupFields(this.NodeOfSelect);
            MapDtls dtls = new MapDtls(this.NodeOfSelect);
            MapM2Ms m2ms = new MapM2Ms(this.NodeOfSelect);
            MapFrames frms = new MapFrames(this.NodeOfSelect);
            foreach (GroupField gf in gfs)
            {
                CheckBox cb = this.Pub2.GetCBByID("CB" + gf.OID);
                if (cb.Checked == false)
                    continue;

                // 生成一个分组实体.
                GroupField mygf = new GroupField();
                mygf.Lab = gf.Lab;
                mygf.EnName = this.FK_Node;
                mygf.Idx = gf.Idx;
                mygf.Insert();

                foreach (MapM2M m2m in m2ms)
                {
                    if (m2m.GroupID != gf.OID)
                        continue;

                    MapM2M mym2m = new MapM2M();
                    mym2m.MyPK = m2m.MyPK.Replace(this.NodeOfSelect, this.FK_Node);
                    //  mym2m.FK_MapData =this
                    if (mym2m.IsExits)
                        continue;

                    mym2m.Copy(m2m);
                    mym2m.FK_MapData = this.FK_Node;
                    mym2m.GroupID = mygf.OID;
                    mym2m.MyPK = m2m.MyPK.Replace(this.NodeOfSelect, this.FK_Node);
                    mym2m.Insert();
                }

                foreach (MapFrame frm in frms)
                {
                    if (frm.GroupID != gf.OID)
                        continue;

                    MapFrame myen = new MapFrame();
                    myen.MyPK = frm.MyPK.Replace(this.NodeOfSelect, this.FK_Node);
                    if (myen.IsExits)
                        continue;

                    myen.Copy(frm);
                    myen.FK_MapData = this.FK_Node;
                    myen.GroupID = mygf.OID;
                    myen.MyPK = frm.MyPK.Replace(this.NodeOfSelect, this.FK_Node);
                    myen.Insert();
                }

                // 复制从表.
                foreach (MapDtl dtl in dtls)
                {
                    cb = this.Pub2.GetCBByID("CB_" + dtl.No + gf.OID);
                    MapDtl dtlNew = new MapDtl();
                    dtlNew.No = dtl.No.Replace(this.NodeOfSelect, this.FK_Node);
                    if (dtlNew.IsExits)
                        continue;

                    dtlNew.Copy(dtl);
                    dtlNew.FK_MapData = this.FK_Node;
                    dtlNew.No = dtl.No.Replace(this.NodeOfSelect, this.FK_Node);

                    //  dtlNew.No = this.FK_Node + "Dtl";
                    // dtlNew.No = dtl.No.Replace(this.FK_Node, this.NodeOfSelect);

                    dtlNew.IsInsert = false;
                    dtlNew.IsUpdate = false;
                    dtlNew.IsDelete = false;
                    dtlNew.PTable = dtlNew.No;
                    dtlNew.Insert();

                    // 复制从表里面的明细。
                    int idx = 0;
                    MapAttrs mattrs = new MapAttrs(dtl.No);
                    foreach (MapAttr attr in mattrs)
                    {
                        MapAttr attrNew = new MapAttr();
                        attrNew.Copy(attr);
                        attrNew.FK_MapData = dtlNew.No;
                        attrNew.UIIsEnable = false;
                        if (attrNew.DefVal.Contains("@"))
                            attrNew.DefVal = "";

                        attrNew.HisEditType = EditType.Edit;
                        attrNew.Insert();
                    }
                }

                // copy his fields. 
                MapAttrs willCopyAttrs = new MapAttrs();
                willCopyAttrs.Retrieve(MapAttrAttr.GroupID, gf.OID, MapAttrAttr.Idx);
                int idx1 = 0;
                foreach (MapAttr attr in willCopyAttrs)
                {
                    MapAttr attrNew = new MapAttr();
                    if (attrNew.IsExit(MapAttrAttr.FK_MapData, this.FK_Node,
                        MapAttrAttr.KeyOfEn, attr.KeyOfEn) == true)
                        continue;

                    if (attr.UIVisible == false)
                        continue;

                    idx1++;
                    attrNew.Copy(attr);
                    attrNew.GroupID = mygf.OID;
                    attrNew.FK_MapData = this.FK_Node;
                    attrNew.UIIsEnable = false;
                    attrNew.Idx = idx1;
                    if (attrNew.KeyOfEn == "Title")
                        attrNew.HisEditType = EditType.Edit;

                    attrNew.HisEditType = EditType.Edit;
                    attrNew.DefVal = "";
                    attrNew.Insert();
                }
            }


            int GroupField = this.Pub2.GetDDLByID("DDL_GroupField").SelectedItemIntVal;
            foreach (Attr attr in attrs)
            {
                if (this.Pub2.IsExit(attr.Key) == false)
                    continue;
                CheckBox cb = this.Pub2.GetCBByID(attr.Key);
                if (cb.Checked == false)
                    continue;

                BP.Sys.MapAttr ma = new BP.Sys.MapAttr();
                int i = ma.Retrieve(BP.Sys.MapAttrAttr.KeyOfEn, attr.Key,
                     BP.Sys.MapAttrAttr.FK_MapData, this.NodeOfSelect);

                BP.Sys.MapAttr ma1 = new BP.Sys.MapAttr();
                bool ishavle = ma1.IsExit(BP.Sys.MapAttrAttr.KeyOfEn, attr.Key,
                     BP.Sys.MapAttrAttr.FK_MapData, this.FK_Node);

                if (ishavle)
                    continue;


                ma1.Copy(ma);

                ma1.FK_MapData = this.FK_Node;
                ma1.KeyOfEn = ma.KeyOfEn;
                ma1.Name = ma.Name;
                ma1.GroupID = GroupField;
                ma1.UIIsEnable = false;

                ma1.HisEditType = EditType.Edit;

                if (ma1.DefVal != null && ma1.DefVal.Contains("@"))
                    ma1.DefVal = "";
                ma1.Insert();
            }

            if (this.Pub2.IsExit("CB_Table"))
            {
                if (this.Pub2.GetCBByID("CB_Table").Checked)
                {
                    MapData md1 = new MapData(this.NodeOfSelect);
                    MapData md2 = new MapData(this.FK_Node);
                    //md2.CellsX = md1.CellsX;
                    //md2.CellsY = md1.CellsY;
                    md2.Update();

                    //MapAttrs ma1 = md1.GenerHisTableCells;
                    // 删除历史数据。

                    //ma1.Delete(MapAttrAttr.FK_MapData, this.FK_Node + "T");
                    //foreach (MapAttr attr in ma1)
                    //{
                    //    MapAttr attr2 = new MapAttr();
                    //    attr2.Copy(attr);
                    //    // attr2.OID = 0;
                    //    attr2.GroupID = 0;
                    //    attr2.Idx = 0;
                    //    attr2.FK_MapData = this.FK_Node + "T";
                    //    attr2.UIIsEnable = false;
                    //    attr2.Insert();
                    //}
                }
            }

            this.WinClose();
            //this.WinCloseWithMsg("复制成功");
            //this.Response.Redirect("Designer.aspx?PK=" + this.FK_Node + "&NodeOfSelect=" + this.NodeOfSelect);
        }
    }


}