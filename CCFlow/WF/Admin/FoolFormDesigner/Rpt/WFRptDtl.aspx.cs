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
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.WF;
using BP.Web;

public partial class WF_MapDef_WFRptDtl : BP.Web.WebPage
{
    public string FK_Flow
    {
        get
        {
            string flowNo = this.MyPK.Replace("ND", "");

            flowNo = flowNo.Replace("Rpt", "");

            flowNo = flowNo.Replace("Dtl1", "");
            flowNo = flowNo.Replace("Dtl2", "");
            flowNo = flowNo.Replace("Dtl3", "");
            flowNo = flowNo.Replace("Dtl4", "");
            flowNo = flowNo.Replace("Dtl", "");

            flowNo = flowNo.PadLeft(3, '0');
            return flowNo;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        switch (this.DoType)
        {
            case "DeDtl":
                BindDtlList();
                break;
            default:
                InsertDtl();
                break;
        }
    }
    public void BindDtlList()
    {
       string sql = "SELECT No,Name FROM Sys_MapData WHERE No LIKE '" + this.MyPK + "Dtl%'";
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

        this.Pub1.AddUL();
        foreach (DataRow dr in dt.Rows)
        {
            this.Pub1.AddLi("WFRpt.aspx?MyPK=" + dr[0].ToString(), dr[1].ToString());
        }
        this.Pub1.AddULEnd();
    }
    public void InsertDtl()
    {
        string sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData LIKE '" + this.MyPK.Replace("Rpt", "") + "%'";
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        if (dt.Rows.Count == 0)
        {
            this.WinCloseWithMsg("此流程中没有从表所以您不能插入。");
            return;
        }

        Nodes nds = new Nodes(BP.WF.Glo.GenerFlowNo(this.MyPK));
        this.Pub1.AddTable();
        this.Pub1.AddCaptionLeft("请选择要插入的从表");
        foreach (BP.WF.Node nd in nds)
        {
            if (nd.IsEndNode == false)
                continue;

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("节点步骤：" + nd.Step + " 节点名称：" + nd.Name);
            this.Pub1.AddTREnd();

            MapDtls dtls = new MapDtls("ND" + nd.NodeID);

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin();

            foreach (MapDtl dtl in dtls)
            {
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + dtl.No;
                cb.Text = dtl.Name;
                this.Pub1.Add(cb);

                #region 字段分组
                DDL ddlGroup = new DDL();
                ddlGroup.ID = "DDL_" + dtl.No;
                GroupFields gfs = new GroupFields(dtl.FK_MapData);
                ddlGroup.Bind(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab);
                this.Pub1.Add("插入位置");
                this.Pub1.Add(ddlGroup);

                //this.Pub1.Add("查询条件:");
                //MapAttrs attrs = new MapAttrs(dtl.No);
                //foreach (MapAttr attr in attrs)
                //{
                //    if (attr.UIContralType != UIContralType.DDL)
                //        continue;
                //    cb = new CheckBox();
                //    cb.ID = "CB_" + dtl.No + "_" + attr.KeyOfEn;
                //    cb.Text = attr.Name;
                //    this.Pub1.Add(cb);
                //}
                #endregion 字段分组
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
        }

        this.Pub1.AddTRSum();
        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.Text = "插入到报表";
        btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_Click);
        this.Pub1.AddTD(btn);
        this.Pub1.AddTREnd();
        this.Pub1.AddTableEnd();
    }

    void btn_Click(object sender, EventArgs e)
    {
        MapDtls dtl2s = new MapDtls();
        dtl2s.Delete(MapDtlAttr.FK_MapData, this.MyPK);
        Nodes nds = new Nodes(BP.WF.Glo.GenerFlowNo(this.MyPK));
        foreach (BP.WF.Node nd in nds)
        {
            if (nd.IsEndNode == false)
                continue;

            MapDtls dtls = new MapDtls("ND" + nd.NodeID);
            int i = 0;

            foreach (MapDtl dtl in dtls)
            {
                if (this.Pub1.GetCBByID("CB_" + dtl.No).Checked == false)
                    continue;

                i++;
                // 生成从表让其可以在单个数据里显示他们。
                MapDtl dtlNew = new MapDtl();
                dtlNew.Copy(dtl);
                dtlNew.No = this.MyPK + i;
                dtlNew.FK_MapData = this.MyPK;
                dtlNew.GroupID = this.Pub1.GetDDLByID("DDL_" + dtl.No).SelectedItemIntVal;
                dtlNew.Insert();

                // 删除原来的数据。
                MapAttrs attrsDtl = new MapAttrs();
                attrsDtl.Delete(MapAttrAttr.FK_MapData, dtlNew.No);

                // 复制到新的数据表里。
                MapAttrs attrs = new MapAttrs(dtl.No);
                foreach (MapAttr attr in attrs)
                {
                    MapAttr attrN = new MapAttr();
                    attrN.Copy(attr);
                    attrN.FK_MapData = dtlNew.No;
                    attrN.Insert();
                }
                Cash.Map_Cash.Remove(dtlNew.No);


                #region  复制成 主表.让其可以查询。
                // 处理主表。
                MapData md = new MapData();
                md.Copy(dtlNew);
                md.No = "ND" + int.Parse(this.FK_Flow) + "RptDtl" + i.ToString();
                md.Save();

                // 删除原来的属性。
                attrs.Delete(MapAttrAttr.FK_MapData, md.No);

                // 删除分组。
                GroupField gfBase = new GroupField();
                gfBase.Delete(GroupFieldAttr.EnName, md.No);

                // 增加基本信息分组。
                gfBase.EnName = md.No;
                gfBase.Lab = md.Name;
                gfBase.Idx = 99;
                gfBase.Insert();


                //生成基本信息属性。
                foreach (MapAttr attr in attrs)
                {
                    MapAttr attrN = new MapAttr();
                    attrN.Copy(attr);
                    attrN.FK_MapData = md.No;
                    attrN.GroupID = gfBase.OID;
                    attrN.Insert();
                }

                MapAttrs attrNs = new MapAttrs(md.No);

                // 对个别字段进行处理。
                foreach (MapAttr attr in attrNs)
                {
                    switch (attr.KeyOfEn)
                    {
                        case StartWorkAttr.FK_Dept:
                            continue;
                            //if (attr.UIContralType != UIContralType.DDL)
                            //{
                            //attr.UIBindKey = "BP.Port.Depts";
                            //attr.UIContralType = UIContralType.DDL;
                            //attr.LGType = FieldTypeS.FK;
                            //attr.UIVisible = true;
                            //// if (gfs.Contains(attr.GroupID) == false)
                            //attr.GroupID = gfBase.OID;// gfs[0].GetValIntByKey("OID");
                            //attr.Update();
                            //// }
                            //break;
                        case "FK_NY":
                            //attr.Delete();
                            ////if (attr.UIContralType != UIContralType.DDL)
                            ////{
                            //attr.UIBindKey = "BP.Pub.NYs";
                            //attr.UIContralType = UIContralType.DDL;
                            //attr.LGType = FieldTypeS.FK;
                            //attr.UIVisible = true;
                            ////   if (gfs.Contains(attr.GroupID) == false)
                            //attr.GroupID = gfBase.OID; // gfs[0].GetValIntByKey("OID");
                            //attr.Update();
                            break;
                        case "Rec":
                            attr.UIBindKey = "BP.Port.Emps";
                            attr.UIContralType = UIContralType.DDL;
                            attr.LGType = FieldTypeS.FK;
                            attr.UIVisible = true;
                            attr.Name = "最后处理人";
                            attr.GroupID = gfBase.OID;
                            attr.Update();
                            break;
                        default:
                            break;
                    }
                }

                // 生成流程基本信息属性。
                GroupField gfFlow = new GroupField();
                gfFlow.EnName = md.No;
                gfFlow.Idx = 0;
                gfFlow.Lab = "流程信息";
                gfFlow.Insert();


                MapAttr attrFlow = new BP.Sys.MapAttr();

                attrFlow = new BP.Sys.MapAttr();
                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "Title";
                attrFlow.Name = "标题"; 
                attrFlow.MyDataType = BP.DA.DataType.AppString;
                attrFlow.UIContralType = UIContralType.TB;
                attrFlow.LGType = FieldTypeS.Normal;
                attrFlow.UIVisible = true;
                attrFlow.UIIsEnable = true;
                attrFlow.UIIsLine = true;
                attrFlow.MinLen = 0;
                attrFlow.MaxLen = 1000;
                attrFlow.Idx = -100;
                attrFlow.GroupID = gfFlow.OID;
                attrFlow.Insert();


                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "FlowStarter";
                attrFlow.Name = "发起人"; //"发起人";
                attrFlow.MyDataType = BP.DA.DataType.AppString;
                attrFlow.UIContralType = UIContralType.DDL;
                attrFlow.UIBindKey = "BP.Port.Emps";
                attrFlow.LGType = FieldTypeS.FK;
                attrFlow.UIVisible = true;
                attrFlow.UIIsEnable = false;
                attrFlow.UIIsLine = false;
                attrFlow.MaxLen = 20;
                attrFlow.MinLen = 0;
                attrFlow.Insert();

                attrFlow = new BP.Sys.MapAttr();
                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "FlowStarterDept";
                attrFlow.Name = "发起人部门";
                attrFlow.MyDataType = BP.DA.DataType.AppString;
                attrFlow.UIContralType = UIContralType.DDL;
                attrFlow.UIBindKey = "BP.Port.Depts";
                attrFlow.LGType = FieldTypeS.FK;
                attrFlow.UIVisible = true;
                attrFlow.UIIsEnable = false;
                attrFlow.MaxLen = 32;
                attrFlow.MinLen = 0;
                attrFlow.Insert();


                attrFlow = new BP.Sys.MapAttr();
                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "FlowEmps";
                attrFlow.Name = "参与人"; //  
                attrFlow.MyDataType = BP.DA.DataType.AppString;
                attrFlow.UIContralType = UIContralType.TB;
                attrFlow.LGType = FieldTypeS.Normal;
                attrFlow.UIVisible = true;
                attrFlow.UIIsEnable = true;
                attrFlow.UIIsLine = false;
                attrFlow.MinLen = 0;
                attrFlow.MaxLen = 1000;
                attrFlow.Idx = -100;
                attrFlow.GroupID = gfFlow.OID;
                attrFlow.Insert();


                attrFlow = new BP.Sys.MapAttr();
                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "FlowStartRDT";
                attrFlow.Name = "发起时间"; //  
                attrFlow.MyDataType = BP.DA.DataType.AppDateTime;
                attrFlow.UIContralType = UIContralType.TB;
                attrFlow.LGType = FieldTypeS.Normal;
                attrFlow.UIVisible = true;
                attrFlow.UIIsEnable = true;
                attrFlow.UIIsLine = false;
                attrFlow.MinLen = 0;
                attrFlow.MaxLen = 1000;
                attrFlow.Idx = -100;
                attrFlow.GroupID = gfFlow.OID;
                attrFlow.Insert();

                attrFlow = new BP.Sys.MapAttr();
                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "FlowNY";
                attrFlow.Name = "隶属年月"; 
                attrFlow.MyDataType = BP.DA.DataType.AppString;
                attrFlow.UIContralType = UIContralType.DDL;
                attrFlow.UIBindKey = "BP.Pub.NYs";
                attrFlow.LGType = FieldTypeS.FK;
                attrFlow.UIVisible = true;
                attrFlow.UIIsEnable = false;
                attrFlow.MaxLen = 20;
                attrFlow.MinLen = 0;
                attrFlow.Insert();


                attrFlow = new BP.Sys.MapAttr();
                attrFlow.FK_MapData = md.No;
                attrFlow.HisEditType = EditType.UnDel;
                attrFlow.KeyOfEn = "MyNum";
                attrFlow.Name = "条"; //  
                attrFlow.MyDataType = BP.DA.DataType.AppInt;
                attrFlow.DefVal = "1";
                attrFlow.UIContralType = UIContralType.TB;
                attrFlow.LGType = FieldTypeS.Normal;
                attrFlow.UIVisible = false;
                attrFlow.UIIsEnable = false;
                attrFlow.UIIsLine = false;
                attrFlow.Idx = -101;
                attrFlow.GroupID = gfFlow.OID;
                if (attrFlow.IsExits == false)
                    attrFlow.Insert();

                // 清除缓存的map.
                Cash.Map_Cash.Remove(md.No);
                //检查主表的正确性。
                GEEntity ge = new GEEntity(md.No);
                ge.CheckPhysicsTable();
                #endregion  复制成 主表.让其可以查询。
            }
        }
        this.WinClose();
    }
}
