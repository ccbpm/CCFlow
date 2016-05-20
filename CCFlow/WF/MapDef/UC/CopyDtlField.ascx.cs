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


public partial class WF_MapDef_UC_CopyDtlField :BP.Web.UC.UCBase3
{
    public string Dtl
    {
        get
        {
            return this.Request.QueryString["Dtl"];
        }
    }
    public void BindAttrs()
    {
        MapDtl dtlFrom = new MapDtl(this.Dtl);
        MapDtl dtl = new MapDtl(this.MyPK);

        MapAttrs attrsFrom = new MapAttrs(this.Dtl);
        MapAttrs attrs = new MapAttrs(dtl.No);

        this.AddTable();
        this.AddCaptionLeft("<A href='CopyDtlField.aspx?MyPK=" + this.MyPK + "'>返回</A> - 选择要复制的字段");
        this.AddTR();
        this.AddTDTitle("IDX");
        this.AddTDTitle("名称");
        this.AddTDTitle("字段");
        this.AddTDTitle("类型");
        this.AddTDTitle("默认值");
        this.AddTREnd();

        bool isHave = false;

        int idx = 0;
        foreach (MapAttr attr in attrsFrom)
        {
            switch (attr.KeyOfEn)
            {
                case "OID":
                case "FID":
                case "WorkID":
                case "Rec":
                case "RDT":
                    continue;
                default:
                    break;
            }

            idx++;
            this.AddTR();
            this.AddTDIdx(idx);
            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + attr.MyPK;
            cb.Text = attr.Name;
            if (attrs.Contains(MapAttrAttr.KeyOfEn, attr.KeyOfEn))
                cb.Enabled = false;

            this.AddTD("nowarp=true", cb);
            this.AddTD(attr.KeyOfEn);
            this.AddTD(attr.MyDataTypeStr);
            this.AddTD(attr.DefValReal);
            isHave = true;
            this.AddTREnd();
        }
        idx++;
        this.AddTRSum();
        this.AddTDIdx(idx);
        this.AddTD();
        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.ID = "Btn_Copy";
        btn.Text =  "复制";
        btn.Click += new EventHandler(btn_Click);
        btn.Enabled = isHave;

        this.AddTD("colspan=3", btn);
        this.AddTREnd();
        this.AddTableEnd();
    }

    void btn_Click(object sender, EventArgs e)
    {
        MapAttrs attrsFrom = new MapAttrs(this.Dtl);
        MapAttrs attrs = new MapAttrs(this.MyPK);
        foreach (MapAttr attr in attrsFrom)
        {
            switch (attr.KeyOfEn)
            {
                case "OID":
                case "FID":
                case "WorkID":
                case "Rec":
                case "RDT":
                case "RefPK":
                    continue;
                default:
                    break;
            }

            if (attrs.Contains(MapAttrAttr.KeyOfEn, attr.KeyOfEn))
                continue;

            if (this.GetCBByID("CB_" + attr.MyPK).Checked == false)
                continue;

            MapAttr en = new MapAttr();
            en.Copy(attr);
            en.FK_MapData = this.MyPK;
            en.GroupID = 0;
            //en.Idx = 0;
            en.Insert();
        }
        this.WinCloseWithMsg("复制成功，您可以用调整从表的顺序。");
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Page.Title = "Copy Node Dtl Fields  Esc to exit.";
        if (this.Dtl != null)
        {
            this.BindAttrs();
            return;
        }

        MapDtl dtl = new MapDtl(this.MyPK);
        //string sql = "SELECT DISTINCT PTable, No, Name From Sys_MapDtl WHERE  No <> '"+this.MyPK+"'";
        string sql = "SELECT b.Name as NodeName, a.No AS DtlNo, a.Name as DtlName";
        sql += " FROM Sys_MapDtl a , Sys_MapData b  ";
        sql += " WHERE A.FK_MapData=b.No AND B.No LIKE '"+this.MyPK.Substring(0,4)+"%' AND B.No<>'"+this.MyPK+"'";


        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        if (dt.Rows.Count == 0)
        {
            this.WinCloseWithMsg("没有可复制的节点数据。");
            return;
        }

        if (dt.Rows.Count == 1)
        {
            this.Response.Redirect("CopyDtlField.aspx?MyPK=" + this.MyPK + "&Dtl=" + dt.Rows[0]["DtlNo"].ToString(), true);
            return;
        }

        this.AddFieldSet("本流程上的明细数据");
        this.AddUL();
        foreach (DataRow dr in dt.Rows)
        {
            
            this.AddLi("CopyDtlField.aspx?MyPK=" + this.MyPK + "&Dtl=" + dr["DtlNo"].ToString(), "节点名：" + dr["NodeName"].ToString() + " 表名称：" + dr["DtlName"].ToString());
        }
        this.AddULEnd();
        this.AddFieldSetEnd();


        //this.AddFieldSet("其他流程上的明细数据");
        //this.AddUL();
        //foreach (DataRow dr in dt.Rows)
        //{
        //    this.AddLi("CopyDtlField.aspx?MyPK=" + this.MyPK + "&Dtl=" + dr["No"].ToString(), dr["Name"].ToString());
        //}
        //this.AddULEnd();
        //this.AddFieldSetEnd();


    }
}
