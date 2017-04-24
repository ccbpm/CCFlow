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
using BP.Web;

public partial class WF_MapDef_WFRpt : BP.Web.WebPage
{
    public string FK_MapData
    {
        get
        {
            return this.Request.QueryString["FK_MapData"];
        }
    }
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    /// <summary>
    /// IsEditMapData
    /// </summary>
    public bool IsEditMapData
    {
        get
        {
            string s = this.Request.QueryString["IsEditMapData"];
            if (s == null || s == "1")
                return true;
            return false;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "流程报表定义";
        switch (this.DoType)
        {
            case "Reset":
                BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
                fl.CheckRptOfReset();
                this.Response.Redirect("WFRpt.aspx?FK_MapData=" + this.FK_MapData, true);
                return;
            default:
                break;
        }

        Cash.Map_Cash.Remove(this.FK_MapData);

        MapData md = new MapData(this.FK_MapData);
        MapAttrs mattrs = new MapAttrs(md.No);
        int count = mattrs.Count;
        if (mattrs.Count == 0)
        {
            BP.WF.Flow f = new BP.WF.Flow(this.FK_Flow);
            f.CheckRpt();
            this.Response.Redirect(this.Request.RawUrl, true);
            return;
        }

        if (gfs.Count == 1)
        {
            GroupField mygf = (GroupField)gfs[0];
            if (mygf.Lab != md.Name)
            {
                mygf.Lab = md.Name;
                mygf.Update();
            }
        }

        this.Pub1.AddB(this.Title + "&nbsp;&nbsp;<a href=\"javascript:GroupFieldNew('" + md.No + "')\">字段分组</a>");
        //  this.Pub1.AddB("-<a href=\"javascript:WinOpen('/WF/Comm/Search.htm?EnsName=" + this.MyPK + "')\">查询预览</a>");
        // this.Pub1.AddB("-<a href=\"javascript:WinOpen('/WF/Comm/Group.htm?EnsName=" + this.MyPK + "')\">分析预览</a>");

        if (this.FK_MapData.Contains("RptDtl") == false)
        {
            this.Pub1.AddB("-<a href=\"javascript:DoReset('" + this.FK_Flow + "','" + this.FK_MapData + "')\">重设字段</a>");

            /* 说明是主表：判断它是否有从表。*/
            string sql = "SELECT COUNT(No) FROM Sys_MapDtl WHERE No LIKE 'ND" + int.Parse(this.FK_Flow) + "%'";
            if (BP.DA.DBAccess.RunSQLReturnValInt(sql) >= 1)
            {
                // this.Pub1.AddB("-<a href=\"javascript:AddDtl('" + md.No + "')\">插入从表</a>");
                //sql = "SELECT No FROM Sys_MapData WHERE No LIKE '" + this.MyPK + "Dtl%'";
                //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                //switch (dt.Rows.Count)
                //{
                //    case 0:
                //        break;
                //    case 1:
                //        this.Pub1.AddB("-<a href='WFRpt.aspx?MyPK=" + dt.Rows[0][0].ToString() + "'>明细报表设计</a>");
                //        break;
                //    default:
                //        this.Pub1.AddB("-<a href='WFRpt.aspx?DoType=DeDtl&MyPK=" + this.MyPK + "'>明细报表设计</a>");
                //        break;
                //}
            }
        }
        else
        {
            this.Pub1.AddB("-<a href=\"WFRpt.aspx?MyPK=ND" + int.Parse(this.FK_Flow) + "Rpt\">" + "返回" + "</a>");
        }

        this.Pub1.AddHR();

        this.Pub1.AddTable("width='100%'");
        /*
         * 根据 GroupField 循环出现菜单。
         */
        foreach (GroupField gf in gfs)
        {
            string gfAttr = " onmouseover=GFOnMouseOver('" + gf.OID + "','" + rowIdx + "') onmouseout=GFOnMouseOut()";
            currGF = gf;
            this.Pub1.AddTR(gfAttr);
            if (gfs.Count == 1)
                this.Pub1.AddTD("colspan=4 class=GroupField valign='top' align:left style='height: 24px;align:left' ", "<div style='text-align:left; float:left'>&nbsp;<a href=\"javascript:GroupField('" + this.FK_MapData + "','" + gf.OID + "')\" >" + gf.Lab + "</a></div><div style='text-align:right; float:right'></div>");
            else
                this.Pub1.AddTD("colspan=4 class=GroupField valign='top' align:left style='height: 24px;align:left' onclick=\"GroupBarClick('" + gf.Idx + "')\" ", "<div style='text-align:left; float:left'><img src='../Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'   border=0 />&nbsp;<a href=\"javascript:GroupField('" + this.FK_MapData + "','" + gf.OID + "')\" >" + gf.Lab + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" ><img src='./WF/Img/Btn/Up.gif' class='Arrow' border=0/></a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" ><img src='./WF/Img/Btn/Down.gif' class='Arrow' border=0/></a></div>");

            this.Pub1.AddTREnd();
            int i = -1;
            int idx = -1;
            isLeftNext = true;
            rowIdx = 0;
            foreach (MapAttr attr in mattrs)
            {
                gfAttr = " onmouseover=GFOnMouseOver('" + gf.OID + "','" + rowIdx + "') onmouseout=GFOnMouseOut()";
                if (attr.GroupID == 0)
                {
                    attr.GroupID = gf.OID;
                    attr.Update();
                }

                if (attr.GroupID != gf.OID)
                {
                    if (gf.Idx == 0 && attr.GroupID == 0)
                    {
                    }
                    else
                        continue;
                }

                if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                    continue;

                if (isLeftNext)
                {
                    if (gfs.Count == 0)
                        this.InsertObjects(false);
                    else
                        this.InsertObjects(true);
                }

                // 显示的顺序号.
                idx++;
                if (attr.IsBigDoc && attr.UIIsLine)
                {
                    if (isLeftNext == false)
                    {
                        this.Pub1.AddTD();
                        this.Pub1.AddTD();
                        this.Pub1.AddTREnd();
                    }
                    rowIdx++;
                    this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "'  " + gfAttr);
                    this.Pub1.Add("<TD class=FDesc colspan=4 width='100%' >");
                    this.Pub1.Add(this.GenerLab(attr, idx, 0, count));
                    TextBox mytbLine = new TextBox();
                    mytbLine.ID = "TB_" + attr.KeyOfEn;
                    mytbLine.TextMode = TextBoxMode.MultiLine;
                    mytbLine.Rows = 8;
                    mytbLine.Attributes["style"] = "width:100%;padding: 0px;margin: 0px;";
                    mytbLine.Enabled = attr.UIIsEnable;
                    if (mytbLine.Enabled == false)
                        mytbLine.Attributes["class"] = "TBReadonly";
                    this.Pub1.Add(mytbLine);
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                    isLeftNext = true;
                    continue;
                }

                if (attr.IsBigDoc)
                {
                    if (isLeftNext)
                    {
                        rowIdx++;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' " + gfAttr);
                    }
                    this.Pub1.Add("<TD class=FDesc colspan=2 width='50%' >");
                    this.Pub1.Add(this.GenerLab(attr, idx, 0, count));
                    TextBox mytbLine = new TextBox();
                    mytbLine.TextMode = TextBoxMode.MultiLine;
                    mytbLine.Rows = 8;
                    mytbLine.Attributes["style"] = "width:100%;padding: 0px;margin: 0px;";
                    mytbLine.ID = "TB_" + attr.KeyOfEn;

                    mytbLine.Enabled = attr.UIIsEnable;
                    if (mytbLine.Enabled == false)
                        mytbLine.Attributes["class"] = "TBReadonly";


                    this.Pub1.Add(mytbLine);
                    this.Pub1.AddTDEnd();
                    if (isLeftNext == false)
                    {
                        this.Pub1.AddTREnd();
                    }

                    isLeftNext = !isLeftNext;
                    continue;
                }

                //计算 colspanOfCtl .
                int colspanOfCtl = 1;
                if (attr.UIIsLine)
                    colspanOfCtl = 3;

                if (attr.UIIsLine)
                {
                    if (isLeftNext == false)
                    {
                        this.Pub1.AddTD();
                        this.Pub1.AddTD();
                        this.Pub1.AddTREnd();
                    }
                    isLeftNext = true;
                }

                if (isLeftNext)
                {
                    rowIdx++;
                    this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' " + gfAttr);
                }

                TB tb = new TB();
                tb.Attributes["width"] = "100%";
                tb.Columns = 60;
                tb.ID = "TB_" + attr.KeyOfEn;

                #region add contrals.
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:

                        tb.Enabled = attr.UIIsEnable;
                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppString:
                                this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                                tb.ShowType = TBType.TB;
                                tb.Text = attr.DefVal;
                                if (colspanOfCtl == 3)
                                {
                                    this.Pub1.AddTD(" width=80% colspan=" + colspanOfCtl, tb);
                                }
                                else
                                {
                                    if (attr.IsSigan)
                                        this.Pub1.AddTD("colspan=" + colspanOfCtl, "<img src='/DataUser/Siganture/" + WebUser.No + ".jpg' border=0 onerror=\"this.src='/DataUser/Siganture/UnName.jpg'\"/>");
                                    else
                                        this.Pub1.AddTD("width='40%' colspan=" + colspanOfCtl, tb);
                                }
                                break;
                            case BP.DA.DataType.AppDate:
                                this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                                tb.ShowType = TBType.Date;

                                tb.Text = attr.DefVal;

                                if (attr.UIIsEnable)
                                    tb.Attributes["onfocus"] = "WdatePicker();";

                                this.Pub1.AddTD("width='40%' colspan=" + colspanOfCtl, tb);
                                break;
                            case BP.DA.DataType.AppDateTime:
                                this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                                tb.ShowType = TBType.DateTime;
                                tb.Text = attr.DefVal;
                                if (attr.UIIsEnable)
                                    tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";

                                this.Pub1.AddTD("width='40%' colspan=" + colspanOfCtl, tb);
                                break;
                            case BP.DA.DataType.AppBoolean:
                                if (attr.UIIsLine)
                                    this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                                else
                                    this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));

                                CheckBox cb = new CheckBox();
                                cb.Text = attr.Name;
                                cb.Checked = attr.DefValOfBool;
                                cb.Enabled = attr.UIIsEnable;
                                this.Pub1.AddTD("width='40%' colspan=" + colspanOfCtl, cb);
                                break;
                            case BP.DA.DataType.AppDouble:
                            case BP.DA.DataType.AppFloat:
                            case BP.DA.DataType.AppInt:
                                this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                                tb.ShowType = TBType.Num;
                                tb.Text = attr.DefVal;
                                this.Pub1.AddTD("width='40%' colspan=" + colspanOfCtl, tb);
                                break;
                            case BP.DA.DataType.AppMoney:
                                this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                                tb.ShowType = TBType.Moneny;
                                tb.Text = attr.DefVal;
                                this.Pub1.AddTD("width='40%' colspan=" + colspanOfCtl, tb);
                                break;
                            default:
                                break;
                        }

                        tb.Attributes["width"] = "100%";
                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppString:
                            case BP.DA.DataType.AppDateTime:
                            case BP.DA.DataType.AppDate:
                                if (tb.Enabled)
                                    tb.Attributes["class"] = "TB";
                                else
                                    tb.Attributes["class"] = "TBReadonly";
                                break;
                            default:
                                if (tb.Enabled)
                                    tb.Attributes["class"] = "TBNum";
                                else
                                    tb.Attributes["class"] = "TBNumReadonly";
                                break;
                        }
                        break;
                    case FieldTypeS.Enum:
                        this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                        DDL ddle = new DDL();
                        ddle.ID = "DDL_" + attr.KeyOfEn;
                        ddle.BindSysEnum(attr.KeyOfEn);
                        ddle.SetSelectItem(attr.DefVal);
                        ddle.Enabled = attr.UIIsEnable;
                        this.Pub1.AddTD("colspan=" + colspanOfCtl, ddle);
                        break;
                    case FieldTypeS.FK:
                        this.Pub1.AddTDDesc(this.GenerLab(attr, idx, i, count));
                        DDL ddl1 = new DDL();
                        ddl1.ID = "DDL_" + attr.KeyOfEn;
                        try
                        {
                            EntitiesNoName ens = attr.HisEntitiesNoName;
                            ens.RetrieveAll();
                            ddl1.BindEntities(ens);
                            ddl1.SetSelectItem(attr.DefVal);
                        }
                        catch
                        {
                        }
                        ddl1.Enabled = attr.UIIsEnable;
                        this.Pub1.AddTD("colspan=" + colspanOfCtl, ddl1);
                        break;
                    default:
                        break;
                }
                #endregion add contrals.

                if (colspanOfCtl == 3)
                {
                    isLeftNext = true;
                    this.Pub1.AddTREnd();
                    continue;
                }

                if (isLeftNext == false)
                {
                    isLeftNext = true;
                    this.Pub1.AddTREnd();
                    continue;
                }
                isLeftNext = false;
            }
            // 最后处理补充上它。
            if (isLeftNext == false)
            {
                this.Pub1.AddTD();
                this.Pub1.AddTD();
                this.Pub1.AddTREnd();
            }
            this.InsertObjects(false);
        }
        this.Pub1.AddTableEnd();
        


        #region 处理iFrom 的自适应的问题。
        string js = "\t\n<script type='text/javascript' >";
        foreach (MapDtl dtl in dtls)
        {
            js += "\t\n window.setInterval(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
        }

        js += "\t\n</script>";
        this.Pub1.Add(js);
        #endregion 处理iFrom 的自适应的问题。

        #region 处理隐藏字段。
        string msg = ""; // +++++++ 编辑隐藏字段 +++++++++ <br>";
        foreach (MapAttr attr in mattrs)
        {
            if (attr.UIVisible)
                continue;
            switch (attr.KeyOfEn)
            {
                case "OID":
                case "FID":
                case "FK_NY":
                case "Emps":
                case "FK_Dept":
                case "WFState":
                case "RDT":
                case "MyNum":
                case "Rec":
                case "CDT":
                    continue;
                default:
                    break;
            }
            msg += "<a href=\"javascript:Edit('" + this.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + attr.Name + "</a> ";
        }

        if (msg.Length > 10)
        {
            this.Pub1.AddFieldSet("编辑隐藏字段");
            this.Pub1.Add(msg);
            this.Pub1.Add("<br>说明：隐藏字段是不显示在表单里面，多用于属性的计算、方向条件的设置，报表的体现。");
            this.Pub1.AddFieldSetEnd();
        }
        #endregion 处理隐藏字段。

        #region 查询条件定义
        this.Pub1.AddFieldSet("查询条件定义" + " - <a href=\"javascript:WinOpen('../Rpt/Search.aspx?FK_Flow=" + this.FK_Flow + "')\">查询预览</a>-<a href=\"javascript:WinOpen('/WF/Comm/Group.htm?EnsName=" + this.MyPK + "')\">分析预览</a>");
        foreach (MapAttr mattr in mattrs)
        {
            if (mattr.UIContralType != UIContralType.DDL)
                continue;

            CheckBox cb = new CheckBox();
            cb.ID = "CB_F_" + mattr.KeyOfEn;
            if (md.RptSearchKeys.Contains("@" + mattr.KeyOfEn))
                cb.Checked = true;

            cb.Text = mattr.Name;
            this.Pub1.Add(cb);
        }

        this.Pub1.AddHR();
        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.Text = "保存";
        btn.ID = "Btn_Save";
        btn.Click += new EventHandler(btn_Click);
        this.Pub1.Add(btn);

        this.Pub1.AddFieldSetEnd();
        #endregion
    }

    void btn_Click(object sender, EventArgs e)
    {
        MapData md = new MapData(this.FK_MapData);
        MapAttrs mattrs = new MapAttrs(md.No);
        string keys = "";
        foreach (MapAttr mattr in mattrs)
        {
            if (mattr.UIContralType != UIContralType.DDL)
                continue;

            CheckBox cb = this.Pub1.GetCBByID("CB_F_" + mattr.KeyOfEn);
            if (cb.Checked)
                keys += "@" + mattr.KeyOfEn;
        }
        //    md.SearchKeys = keys;
        md.Update();
        Cash.Map_Cash.Remove(this.FK_MapData);
    }

    public void InsertObjects(bool isJudgeRowIdx)
    {
    }

    #region varable.
    public GroupField currGF = new GroupField();
    private MapDtls _dtls;
    public MapDtls dtls
    {
        get
        {
            if (_dtls == null)
                _dtls = new MapDtls(this.FK_MapData);

            return _dtls;
        }
    }
    private GroupFields _gfs;
    public GroupFields gfs
    {
        get
        {
            if (_gfs == null)
                _gfs = new GroupFields(this.FK_MapData);

            return _gfs;
        }
    }
    public int rowIdx = 0;
    public bool isLeftNext = true;
    #endregion varable.

    public string GenerLab(MapAttr attr, int idx, int i, int count)
    {
        string webPath = BP.WF.Glo.CCFlowAppPath;
        string divAttr = " onmouseover=FieldOnMouseOver('" + attr.MyPK + "') onmouseout=FieldOnMouseOut('" + attr.MyPK + "') ";
        string lab = attr.Name;
        if (attr.MyDataType == DataType.AppBoolean && attr.UIIsLine)
            lab = "编辑";

        if (attr.HisEditType == EditType.Edit || attr.HisEditType == EditType.UnDel)
        {
            switch (attr.LGType)
            {
                case FieldTypeS.Normal:
                    lab = "<a  href=\"javascript:Edit('" + this.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + lab + "</a>";
                    break;
                case FieldTypeS.FK:
                    lab = "<a  href=\"javascript:EditTable('" + this.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + lab + "</a>";
                    break;
                case FieldTypeS.Enum:
                    lab = "<a  href=\"javascript:EditEnum('" + this.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + lab + "</a>";
                    break;
                default:
                    break;
            }
        }
        else
        {
            lab = attr.Name;
        }

        if (idx == 0)
        {
            /*第一个。*/
            return "<div " + divAttr + " >" + lab + "<a href=\"javascript:Down('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" ><img src='" + webPath + "WF/Img/Btn/Right.gif' class='Arrow' alt='向右动顺序' border=0/></a></div>";
        }

        if (idx == count - 1)
        {
            /*到数第一个。*/
            return "<div " + divAttr + " ><a href=\"javascript:Up('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" ><img src='" + webPath + "WF/Img/Btn/Left.gif' alt='向左移动顺序' class='Arrow' border=0/></a>" + lab + "</div>";
        }
        return "<div " + divAttr + " ><a href=\"javascript:Up('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" ><img src='" + webPath + "WF/Img/Btn/Left.gif' alt='向下移动顺序' class='Arrow' border=0/></a>" + lab + "<a href=\"javascript:Down('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" ><img src='" + webPath + "WF/Img/Btn/Right.gif' alt='向右移动顺序' class='Arrow' border=0/></a></div>";
    }
}
