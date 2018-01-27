using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.WF.XML;
using BP.Sys;
using BP.Web.Controls;
using BP.WF.Template;

namespace CCFlow.WF.CCForm
{
    public partial class Comm_Dtl : BP.Web.WebPage
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                if (string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return 0;

                string str = this.Request.QueryString["FK_Node"];
                if (str == "null")
                    return 0;


                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string FK_MapExt
        {
            get
            {
                return this.Request.QueryString["FK_MapExt"];
            }
        }
        public new string Key
        {
            get
            {
                return this.Request.QueryString["Key"];
            }
        }
        public new string EnsName
        {
            get
            {
                string str = this.Request.QueryString["EnsName"];
                if (str == null)
                    return "ND299Dtl";
                return str;
            }
        }
        /// <summary>
        /// 主表FK_MapData
        /// </summary>
        public string MainEnsName
        {
            get
            {
                string str = this.Request.QueryString["MainEnsName"];
                if (str == null)
                    return "ND299";
                return str;
            }
        }
        public int BlankNum
        {
            get
            {
                try
                {
                    return int.Parse(ViewState["BlankNum"].ToString());
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                ViewState["BlankNum"] = value;
            }
        }
        public new string RefPK
        {
            get
            {
                string str = this.Request.QueryString["RefPK"];
                return str;
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.Request.QueryString["RefPKVal"];
                if (str == null)
                    return "1";
                return str;
            }
        }
        /// <summary>
        /// 设置FID的值
        /// </summary>
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                if (str == null || str == "0")
                    return 0;
                return Int64.Parse(str);
            }
        }
        /// <summary>
        /// 明细表数量.
        /// </summary>
        public int DtlCount
        {
            get
            {
                return int.Parse(ViewState["DtlCount"].ToString());
            }
            set
            {
                ViewState["DtlCount"] = value;
            }
        }
        /// <summary>
        /// 只读
        /// </summary>
        public int IsReadonly
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["IsReadonly"]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 增加列的数量。
        /// </summary>
        public int addRowNum
        {
            get
            {
                try
                {
                    int i = int.Parse(this.Request.QueryString["addRowNum"]);
                    if (this.Request.QueryString["IsCut"] == null)
                        return i;
                    else
                        return i;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int _allRowCount = 0;
        public int allRowCount
        {
            get
            {
                int i = 0;
                try
                {
                    i = int.Parse(this.Request.QueryString["rowCount"]);
                }
                catch
                {
                    return 0;
                }
                return i;
            }
        }
        public int IsWap
        {
            get
            {
                if (this.Request.QueryString["IsWap"] == "1")
                    return 1;
                return 0;
            }
        }
        public bool IsEnable_del
        {
            get
            {
                string s = this.ViewState["R"] as string;
                if (s == null || s == "0")
                    return false;
                return true;
            }
            set
            {
                if (value)
                    this.ViewState["R"] = "1";
                else
                    this.ViewState["R"] = "0";
            }
        }
        public GEEntity _MainEn = null;
        public GEEntity MainEn
        {
            get
            {
                if (_MainEn == null)
                    _MainEn = new GEEntity(this.FK_MapData, this.RefPKVal);
                return _MainEn;
            }
        }
        public MapAttrs _MainMapAttrs = null;
        public MapAttrs MainMapAttrs
        {
            get
            {
                if (_MainMapAttrs == null)
                    _MainMapAttrs = new MapAttrs(this.FK_MapData);
                return _MainMapAttrs;
            }
        }
        public string FK_MapData = null;
        private Hashtable HTTemp = new Hashtable();
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //string url = this.Request.RawUrl;
            //url = url.Replace(".aspx", ".htm");
            //this.Response.Redirect(url, true);
            //return;


            MapDtl mdtl = new MapDtl(this.EnsName);
            BP.WF.Node nd = null;

            if (this.FK_Node != 0 && mdtl.FK_MapData!="Temp" && this.EnsName.Contains("ND" + this.FK_Node) ==false)
            {
                nd = new BP.WF.Node(this.FK_Node);
                /*如果
                 * 1,传来节点ID, 不等于0.
                 * 2,不是节点表单.  就要判断是否是独立表单，如果是就要处理权限方案。*/

                BP.WF.Template.FrmNode fn = new BP.WF.Template.FrmNode(nd.FK_Flow,nd.NodeID,mdtl.FK_MapData);
                int i=fn.Retrieve(FrmNodeAttr.FK_Frm, mdtl.FK_MapData, FrmNodeAttr.FK_Node, this.FK_Node);
                if (i != 0 && fn.FrmSln !=0 )
                {
                    /*使用了自定义的方案.
                     * 并且，一定为dtl设定了自定义方案，就用自定义方案.
                     */
                    MapDtl mymdtl = new MapDtl();
                    mymdtl.No = this.EnsName + "_" + this.FK_Node;
                    if (mymdtl.RetrieveFromDBSources() == 1)
                    {
                     //   /Dtl.aspx?EnsName=DtlDemo_FJDtl396340&RefPKVal=212&IsReadonly=0&FID=0&FK_Node=8701
                        this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "_" + this.FK_Node + "&RefPKVal=" + this.RefPKVal + "&IsReadonly=" + this.IsReadonly + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node, true);
                        return;
                    }
                }
            }

            if (mdtl.DtlModel == DtlModel.FixRow)
            {
                this.Response.Redirect("DtlFixRow.aspx?1=2" + this.RequestParas, true);
                return;
            }

            if (mdtl.HisEditModel == EditModel.FoolModel)
            {
                this.Response.Redirect("DtlCard.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node + "&MainEnsName=" + this.MainEnsName, true);
                return;
            }

            if (this.IsReadonly == 1)
            {
                mdtl._IsReadonly = 1;
                this.Button1.Enabled = false;
            }

            if (nd==null && this.FK_Node!=0)
                nd = new BP.WF.Node(this.FK_Node);

            this.Bind(mdtl, nd);
        }

        public void Bind(MapDtl mdtl, BP.WF.Node nd)
        {
            if (this.Request.QueryString["IsTest"] != null)
                BP.DA.Cash.SetMap(this.EnsName, null);

            GEDtls dtls = new GEDtls(this.EnsName);
            this.FK_MapData = mdtl.FK_MapData;
            GEEntity mainEn = null;

            #region 生成标题
            MapAttrs attrs = new MapAttrs(this.EnsName);
            MapAttrs attrs2 = new MapAttrs();
            int numOfCol = 0;

            float dtlWidth = mdtl.W - 20;
            this.Pub1.Add("<Table border=0  style='width:" + dtlWidth + "px'>");
            this.Pub1.Add(mdtl.MTR);
            if (mdtl.IsShowTitle)
            {
                this.Pub1.AddTR();

                if (this.IsWap == 1)
                {
                    string url = "../WAP/MyFlow.aspx?WorkID=" + this.RefPKVal + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + nd.FK_Flow;
                    this.Pub1.AddTD("<img onclick=\"javascript:SaveDtlDataTo('" + url + "');\" src='../Wap/Img/Back.png' style='width:50px;height:16px' border=0/>");
                }
                else
                {
                    this.Pub1.Add("<TD class='Idx' ><img src='../Img/Btn/Table.gif' onclick=\"return DtlOpt('" + this.RefPKVal + "','" + this.EnsName + "','" + this.FID + "', " + this.FK_Node + ");\" border=0/></TD>");
                    numOfCol++;
                }

                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.IsPK)
                        continue;

                    //如果启用了分组，并且是当前的分组字段。
                    if (mdtl.IsEnableGroupField && mdtl.GroupField == attr.KeyOfEn)
                        continue;

                    //for lijian 增加了 @符号是一个换行符. 
                    this.Pub1.AddTDTitleExt(attr.Name.Replace("@", "<br>"));// ("<TD class='FDesc' nowarp=true ><label>" + attr.Name + "</label></TD>");
                    numOfCol++;
                }

                if (mdtl.IsEnableAthM)
                {
                    this.Pub1.AddTDTitleExt("");
                    numOfCol++;
                }

                if (mdtl.IsEnableM2M)
                {
                    this.Pub1.AddTDTitleExt("");
                    numOfCol++;
                }

                if (mdtl.IsEnableM2MM)
                {
                    this.Pub1.AddTDTitleExt("");
                    numOfCol++;
                }

                if (mdtl.IsDelete && this.IsReadonly == 0)
                {
                    this.Pub1.Add("<TD class='TitleExt' nowarp=true ><img src='../Img/Btn/Save.gif' border=0 onclick='SaveDtlData();' ></TD>");
                    numOfCol++;
                }

                if (mdtl.IsEnableLink)
                {
                    this.Pub1.AddTDTitleExt("");
                    numOfCol++;
                }

                this.Pub1.AddTREnd();
            }
            #endregion 生成标题

            #region 生成数据.
            QueryObject qo = null;
            try
            {
                qo = new QueryObject(dtls);
                switch (mdtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        qo.addAnd();
                        qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        break;
                    case DtlOpenType.ForFID: // 按流程ID来控制.这里不允许修改，如需修改则加新case.
                        if (nd == null)
                            throw new Exception("@当前您是配置的权限是FID,但是当前没有节点ID.");

                        if (nd.HisNodeWorkType == BP.WF.NodeWorkType.SubThreadWork)
                            qo.AddWhere(GEDtlAttr.RefPK, this.FID); //edit by zhoupeng 2016.04.23
                        else
                            qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                        break;
                }

                if (mdtl.FilterSQLExp != "")
                {
                    string[] strs = mdtl.FilterSQLExp.Split('=');
                    qo.addAnd();
                    qo.AddWhere(strs[0], strs[1]);
                }
            }
            catch(Exception ex)
            {
                dtls.GetNewEntity.CheckPhysicsTable();
                throw ex;
            }

            //取出来附件集合，用户显示个数.
            FrmAttachmentDBs athDBs = null;
            if (mdtl.IsEnableAthM)
            {
                athDBs = new FrmAttachmentDBs();
                athDBs.Retrieve(FrmAttachmentDBAttr.FID, this.RefPKVal);
             }
            #endregion 生成数据.

            #region 生成翻页
            if (mdtl.IsEnableGroupField == true || mdtl.HisWhenOverSize == WhenOverSize.None 
                || mdtl.HisWhenOverSize == WhenOverSize.AddRow)
            {
                /*如果 是分组显示模式 .*/
                try
                {
                    int num = qo.DoQuery();
                    if (allRowCount == 0)
                    {
                        if (mdtl.RowsOfList >= num)
                        {
                            mdtl.RowsOfList = mdtl.RowsOfList;
                            _allRowCount = mdtl.RowsOfList;
                        }
                        else
                        {
                            mdtl.RowsOfList = num;
                            _allRowCount = num;
                        }
                    }
                    else
                    {
                        mdtl.RowsOfList = allRowCount;
                        _allRowCount = allRowCount;
                    }


                    if (this.IsReadonly == 0)
                    {
                        int dtlCount = dtls.Count;
                        for (int i = 0; i < mdtl.RowsOfList - dtlCount; i++)
                        {
                            BP.Sys.GEDtl dt = new GEDtl(this.EnsName);
                            dt.ResetDefaultVal();
                            dt.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                            dt.OID = i;
                            dtls.AddEntity(dt);
                        }

                        //if (num == mdtl.RowsOfList)
                        //{
                        //    BP.Sys.GEDtl dt1 = new GEDtl(this.EnsName);
                        //    dt1.ResetDefaultVal();
                        //    dt1.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                        //    dt1.OID = mdtl.RowsOfList + 1;
                        //    dtls.AddEntity(dt1);
                        //}
                    }
                }
                catch(Exception ex)
                {
                    dtls.GetNewEntity.CheckPhysicsTable();
                    throw ex;
                }
            }
            else
            {
                /*如果不是分组显示模式 .*/
                this.Pub2.Clear();
                try
                {
                    int pageSize = mdtl.RowsOfList;

                    int count = qo.GetCount();
                    if (allRowCount == 0)
                    {
                        if (mdtl.RowsOfList >= count)
                        {

                            _allRowCount = mdtl.RowsOfList;
                        }
                        else
                        {
                            _allRowCount = count;
                        }
                    }
                    else
                    {
                        _allRowCount = allRowCount;
                        count = allRowCount;
                    }

                    this.DtlCount = count;
                    this.Pub2.Clear();
                    this.Pub2.BindPageIdx(count, pageSize, this.PageIdx, "Dtl.aspx?EnsName=" + this.EnsName + "&FID=" + this.FID + "&RefPKVal=" + this.RefPKVal + "&IsWap=" + this.IsWap + "&IsReadonly=" + this.IsReadonly + "&MainEnsName=" + this.MainEnsName + "&rowCount=" + _allRowCount);
                    int num = qo.DoQuery("OID", pageSize, this.PageIdx, false);

                    if (mdtl.IsInsert && this.IsReadonly == 0)
                    {
                        int dtlCount = dtls.Count;
                        for (int i = 0; i < pageSize - dtlCount; i++)
                        {
                            BP.Sys.GEDtl dt = new GEDtl(this.EnsName);
                            dt.ResetDefaultVal();
                            dt.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                            dt.OID = i;
                            dtls.AddEntity(dt);
                        }

                        //if (num == mdtl.RowsOfList)
                        //{
                        //    BP.Sys.GEDtl dt1 = new GEDtl(this.EnsName);
                        //    dt1.ResetDefaultVal();
                        //    dt1.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                        //    dt1.OID = mdtl.RowsOfList + 1;
                        //    dtls.AddEntity(dt1);
                        //}
                    }
                }
                catch
                {
                    dtls.GetNewEntity.CheckPhysicsTable();
                }

            }
            #endregion 生成翻页

            DDL ddl = new DDL();
            CheckBox cb = new CheckBox();

            // 行锁定.
            bool isRowLock = mdtl.IsRowLock;

            #region 生成数据
            int idx = 1;
            string ids = ",";
            int dtlsNum = dtls.Count;
            MapExts mes = new MapExts(this.EnsName);

            // 需要自动填充的下拉框IDs. 这些下拉框不需要自动填充数据。
            string autoFullDataDDLIDs = ",";
            string LinkFields = ",";
            foreach (MapExt me in mes)
            {
                switch (me.ExtType)
                {
                    case MapExtXmlList.ActiveDDL:
                        autoFullDataDDLIDs += me.AttrsOfActive + ",";
                        break;
                    case MapExtXmlList.AutoFullDLL:
                        autoFullDataDDLIDs += me.AttrOfOper + ",";
                        break;
                    case MapExtXmlList.Link:
                        LinkFields += me.AttrOfOper + ",";
                        break;
                    default:
                        break;
                }
            }

            if (mdtl.IsEnableGroupField)
            {
                /*如果是分组显示模式, 就要特殊的处理显示.
                 1， 求出分组集合。
                 */

                string gField = mdtl.GroupField;
                MapAttr attrG = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, gField) as MapAttr;
                if (attrG == null)
                {
                    this.Pub1.Clear();
                    this.Pub1.AddFieldSetRed("err",
                        "明细表设计错误,分组字段已经不存在明细表中，请联系管理员解决此问题。");
                    return;
                }

                if (attrG.UIContralType == UIContralType.DDL)
                {
                    gField = gField + "Text";
                }

                //求出分组集合.
                string tmp = "";
                foreach (BP.Sys.GEDtl dtl in dtls)
                {
                    if (tmp.Contains("," + dtl.GetValStrByKey(gField) + ",") == false)
                        tmp += "," + dtl.GetValStrByKey(gField);
                }
                string[] strs = tmp.Split(',');

                string groupStr = "";
                // 遍历-分组集合.
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;

                    #region 增加分组行.

                    this.Pub1.AddTR();
                    if (attrG.UIContralType == UIContralType.CheckBok)
                    {
                        if (str == "0")
                            this.Pub1.AddTD("colspan=" + numOfCol, attrG.Name + ":是");
                        else
                            this.Pub1.AddTD("colspan=" + numOfCol, attrG.Name + ":否");
                    }
                    else
                    {
                        if (!groupStr.Contains(str + ","))
                        {
                            this.Pub1.AddTD("colspan=" + numOfCol, str);
                            groupStr += str + ",";
                        }
                    }
                    this.Pub1.AddTREnd();

                    #endregion 增加分组行.

                    #region 增加该分组的数据.

                    foreach (BP.Sys.GEDtl dtl in dtls)
                    {
                        if (dtl.GetValStrByKey(gField) != str)
                            continue;

                        #region 处理 IDX AddTR

                        if (ids.Contains("," + dtl.OID + ","))
                            continue;
                        ids += dtl.OID + ",";
                        this.Pub1.AddTR();
                        if (mdtl.IsShowIdx)
                        {
                            this.Pub1.AddTDIdx(idx++);
                        }
                        else
                            this.Pub1.AddTD();

                        #endregion 处理

                        #region 增加rows
                        foreach (MapAttr attr in attrs)
                        {
                            if (attr.UIVisible == false
                                || attr.KeyOfEn == "OID"
                                || attr.KeyOfEn == attrG.KeyOfEn)
                                continue;
                            //只读
                            if (this.IsReadonly == 1)
                                attr.UIIsEnable = false;
                            ////处理它的默认值.
                            //if (attr.DefValReal.Contains("@") == true && attr.UIIsEnable == false)
                            //    dtl.SetValByKey(attr.KeyOfEn, attr.DefVal);

                            string val = dtl.GetValByKey(attr.KeyOfEn).ToString();
                            if (attr.UIIsEnable == false && dtl.OID >= 100 &&
                                LinkFields.Contains("," + attr.KeyOfEn + ","))
                            {
                                if (string.IsNullOrEmpty(val))
                                    val = "...";
                                MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link,
                                    MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;

                                string url = meLink.Tag.Clone() as string;
                                if (url.Contains("?") == false)
                                    url = url + "?a3=2";

                                url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + mdtl.No +
                                      "&OID=" + dtl.OID;
                                if (url.Contains("@AppPath"))
                                    url = url.Replace("@AppPath",
                                        "http://" + this.Request.Url.Host + this.Request.ApplicationPath);

                                if (url.Contains("@"))
                                {
                                    if (attrs2.Count == 0)
                                        attrs2 = new MapAttrs(mdtl.No);
                                    foreach (MapAttr item in attrs2)
                                    {
                                        url = url.Replace("@" + item.KeyOfEn, dtl.GetValStrByKey(item.KeyOfEn));
                                        if (url.Contains("@") == false)
                                            break;
                                    }
                                    if (url.Contains("@"))
                                    {
                                        /*可能是主表也要有参数*/
                                        if (mainEn == null)
                                            mainEn = this.MainEn;
                                        foreach (Attr attrM in mainEn.EnMap.Attrs)
                                        {
                                            url = url.Replace("@" + attrM.Key, mainEn.GetValStrByKey(attrM.Key));
                                            if (url.Contains("@") == false)
                                                break;
                                        }
                                    }
                                }
                                this.Pub1.AddTD("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + val + "</a>");
                                continue;
                            }

                            switch (attr.UIContralType)
                            {
                                case UIContralType.TB:
                                    TextBox tb = new TextBox();
                                    tb.ID = "TB_" + attr.KeyOfEn + "_" + dtl.OID;
                                    //tb.Enabled = attr.UIIsEnable; //edited by liuxc,2015.7.3,如果Enabled=false，则服务器控件将不能获取此控件的真实值
                                    if (attr.UIIsEnable == false)
                                    {
                                        tb.Attributes.Add("readonly", "true");
                                        tb.CssClass = "TBReadonly";
                                    }
                                    else
                                    {
                                        tb.Attributes["onfocus"] = "SetChange(true);";
                                    }
                                    switch (attr.MyDataType)
                                    {
                                        case DataType.AppString:
                                            tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                            this.Pub1.AddTD("width='2px'", tb);
                                            tb.Text = val;
                                            if (attr.UIIsEnable == false)
                                            {
                                                tb.Attributes.Add("readonly", "true");
                                                tb.CssClass = "TBReadonly";
                                            }

                                            if (attr.UIHeight > 25)
                                            {
                                                tb.TextMode = TextBoxMode.MultiLine;
                                                tb.Attributes["Height"] = attr.UIHeight + "px";
                                                tb.Rows = attr.UIHeightInt / 25;
                                            }
                                            break;
                                        case DataType.AppDate:
                                            float dateWidth = attr.UIWidth == 100 ? 85 : attr.UIWidth;
                                            tb.Attributes["style"] = "width:" + dateWidth + "px;border-width:0px;";
                                            if (val != "0")
                                                tb.Text = val;

                                            if (attr.UIIsEnable)
                                            {
                                                tb.Attributes["onfocus"] = "WdatePicker();SetChange(false);";
                                                tb.Attributes["onChange"] = "SetChange(true);";
                                                tb.Attributes["class"] = "Wdate";
                                                //tb.CssClass = "easyui-datebox";
                                                //tb.Attributes["data-options"] = "editable:false";

                                            }
                                            else
                                                tb.ReadOnly = true;

                                            this.Pub1.AddTD("width='2px'", tb);
                                            break;
                                        case DataType.AppDateTime:

                                            float dateTimeWidth = attr.UIWidth == 100 ? 125 : attr.UIWidth;
                                            tb.Attributes["style"] = "width:" + dateTimeWidth + "px;border-width:0px;";
                                            if (val != "0")
                                                tb.Text = val;
                                            if (attr.UIIsEnable)
                                            {
                                                tb.Attributes["onfocus"] =
                                                    "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});SetChange(false);";
                                                //tb.CssClass = "easyui-datetimebox";
                                                //tb.Attributes["data-options"] = "editable:false";

                                                tb.Attributes["onChange"] = "SetChange(true);";
                                                tb.Attributes["class"] = "Wdate";

                                            }
                                            else
                                            {
                                                tb.ReadOnly = true;
                                            }
                                            this.Pub1.AddTD("width='2px'", tb);
                                            break;
                                        case DataType.AppInt:
                                            tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                            if (attr.UIIsEnable == false)
                                            {
                                                tb.Attributes["class"] = "TBNumReadonly";
                                                tb.ReadOnly = true;
                                            }
                                            try
                                            {
                                                tb.Text = val;
                                            }
                                            catch (Exception ex)
                                            {
                                                this.Alert(ex.Message + " val =" + val);
                                                tb.Text = "0";
                                            }
                                            this.Pub1.AddTD(tb);
                                            break;
                                        case DataType.AppMoney:
                                            tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                            if (attr.UIIsEnable == false)
                                            {
                                                tb.Attributes["class"] = "TBNumReadonly";
                                                tb.ReadOnly = true;
                                            }

                                            try
                                            {
                                                tb.Text = decimal.Parse(val).ToString("0.00");
                                            }
                                            catch (Exception ex)
                                            {
                                                this.Alert(ex.Message + " val =" + val);
                                                tb.Text = "0.00";
                                            }
                                            this.Pub1.AddTD(tb);
                                            break;
                                        default:
                                            tb.Attributes["style"] = "width:" + attr.UIWidth +
                                                                     "px;text-align:right;border-width:0px;";
                                            tb.Text = val;
                                            this.Pub1.AddTD(tb);
                                            break;
                                    }

                                    if (attr.IsNum && attr.LGType == FieldTypeS.Normal)
                                    {
                                        if (tb.Enabled)
                                        {
                                            // OnKeyPress="javascript:return VirtyNum(this);"
                                            if (attr.MyDataType == DataType.AppInt)
                                            {
                                                tb.Attributes["onkeyup"] += @"C" + dtl.OID + "();C" + attr.KeyOfEn +
                                                                            "(); ";
                                                tb.Attributes["OnKeyPress"] +=
                                                    @"javascript:return VirtyNum(this,'int');";

                                                tb.Attributes["onblur"] += @"value=value.replace(/[^-?\d]/g,'');C" +
                                                                           dtl.OID + "();C" + attr.KeyOfEn + "();";
                                            }
                                            else
                                            {
                                                tb.Attributes["onkeyup"] += @"C" + dtl.OID + "();C" + attr.KeyOfEn +
                                                                            "();";
                                                tb.Attributes["OnKeyPress"] +=
                                                    @"javascript:return VirtyNum(this,'float');";

                                                tb.Attributes["onblur"] +=
                                                    @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');C" + dtl.OID + "();C" +
                                                    attr.KeyOfEn + "();";
                                            }

                                            tb.Attributes["style"] = "width:" + attr.UIWidth +
                                                                     "px;text-align:right;border-width:0px;";
                                        }
                                        else
                                        {
                                            tb.Attributes["onpropertychange"] += "C" + attr.KeyOfEn + "();";
                                            tb.Attributes["style"] = "width:" + attr.UIWidth +
                                                                     "px;text-align:right;border-width:0px;";
                                        }
                                    }
                                    break;
                                case UIContralType.DDL:
                                    switch (attr.LGType)
                                    {
                                        case FieldTypeS.Enum:
                                            DDL myddl = new DDL();
                                            myddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                            myddl.Attributes["onchange"] = "SetChange (true);";
                                            if (attr.UIIsEnable)
                                            {
                                                try
                                                {
                                                    myddl.BindSysEnum(attr.KeyOfEn);
                                                    myddl.SetSelectItem(val);
                                                }
                                                catch (Exception ex)
                                                {
                                                    BP.Sys.PubClass.Alert(ex.Message);
                                                }
                                            }
                                            else
                                            {
                                                myddl.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.KeyOfEn),
                                                    dtl.GetValStrByKey(attr.KeyOfEn)));
                                            }
                                            myddl.Enabled = attr.UIIsEnable;
                                            this.Pub1.AddTDCenter(myddl);
                                            break;
                                        case FieldTypeS.FK:
                                            DDL ddl1 = new DDL();
                                            ddl1.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                            ddl1.Attributes["onchange"] = "SetChange (true);";
                                            ddl1.Attributes["onfocus"] = "SetChange(true);";
                                            if (attr.UIIsEnable)
                                            {
                                                //   ddl1.Attributes["onchange"] = "SetChange(true);";
                                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                                //ens.RetrieveAll();    //在attr.HisEntitiesNoName属性中，就已经获取了集合的数据，此处不需要再次获取，而且，如果是DDL中的动态SQL查询类型，此处使用RetrieveAll获取数据，会将上面已经获取的数据都清空掉，added by liuxc,2017-9-30
                                                ddl1.BindEntities(ens);

                                                //如果没有选择到数据，就让其出现请选择Item.
                                                if (ddl1.SetSelectItem(val) == false)
                                                {
                                                    if (ens.Count >= 1)
                                                    {
                                                        Entity en = ens[0] as Entity;
                                                        if (en!=null && en.GetValStrByKey("Name").Contains("请选择") == false)
                                                        {
                                                            ddl1.Items.Insert(0, new ListItem("请选择", val));
                                                        }
                                                        ddl1.SelectedIndex = 0;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                val = dtl.GetValStrByKey(attr.KeyOfEn);
                                                string text = dtl.GetValRefTextByKey(attr.KeyOfEn);

                                                if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(val) == false)
                                                {
                                                    EntitiesNoName ens = attr.HisEntitiesNoName;
                                                    EntityNoName enn = ens.GetEntityByKey(val) as EntityNoName;

                                                    if (enn != null)
                                                    {
                                                        text = enn.Name;
                                                    }
                                                    else
                                                    {
                                                        Entity myen = ens.GetNewEntity;
                                                        myen.PKVal = val;
                                                        if (myen.RetrieveFromDBSources() != 0)
                                                            text = myen.GetValStringByKey("Name");
                                                        else
                                                            text = val;
                                                    }
                                                }

                                                ddl1.Items.Add(new ListItem(text, val));
                                            }
                                            ddl1.Enabled = attr.UIIsEnable;
                                            this.Pub1.AddTDCenter(ddl1);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                case UIContralType.CheckBok:
                                    cb = new CheckBox();
                                    cb.ID = "CB_" + attr.KeyOfEn + "_" + dtl.OID;
                                    cb.Text = attr.Name;
                                    if (val == "1")
                                        cb.Checked = true;
                                    else
                                        cb.Checked = false;
                                    //  cb.Attributes["onchecked"] = "alert('ss'); isChange= true; ";
                                    cb.Attributes["onclick"] = "isChange= true;";
                                    cb.Enabled = attr.UIIsEnable;
                                    this.Pub1.AddTD(cb);
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (mdtl.IsEnableAthM)
                        {
                            if (dtl.OID >= 100)
                            {
                                int num = athDBs.GetCountByKey(FrmAttachmentDBAttr.RefPKVal, dtl.OID);
                                this.Pub1.AddTD(
                                    "<a href=\"javascript:window.showModalDialog('AttachmentUpload.aspx?IsBTitle=1&PKVal=" +
                                    dtl.OID + "&Ath=AthMDtl&FK_MapData=" + mdtl.No + "&FK_Node=" + this.FK_Node + "&FK_FrmAttachment=" + mdtl.No +
                                    "_AthMDtl','dialogHeight: 300px; dialogWidth: 800px;center: yes; help: no')\"><img src='../Img/AttachmentM.png' border=0 width='16px' />(" + num + ")</a>");
                            }
                            else
                                this.Pub1.AddTD("");
                        }

                        if (mdtl.IsEnableM2M)
                        {
                            if (dtl.OID >= 100)
                                this.Pub1.AddTD(
                                    "<a href=\"javascript:window.showModalDialog('M2M.aspx?IsOpen=1&NoOfObj=M2M&OID=" +
                                    dtl.OID + "&FK_MapData=" + mdtl.No +
                                    "','m2m','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='../Img/M2M.png' border=0 width='16px' /></a>");
                            else
                                this.Pub1.AddTD("");
                        }

                        if (mdtl.IsEnableM2MM)
                        {
                            if (dtl.OID >= 100)
                                this.Pub1.AddTD(
                                    "<a href=\"javascript:window.showModalDialog('M2MM.aspx?IsOpen=1&NoOfObj=M2MM&OID=" +
                                    dtl.OID + "&FK_MapData=" + mdtl.No +
                                    "','m2m','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='../Img/M2M.png' border=0 width='16px' /></a>");
                            else
                                this.Pub1.AddTD("");
                        }

                        if (mdtl.IsDelete && this.IsReadonly == 0 && dtl.OID >= 100)
                        {
                            if (isRowLock == true && dtl.IsRowLock == true)
                                this.Pub1.AddTD("<img src='../Img/Btn/Lock.png' class=ICON />"); //如果当前记录是锁定的，并且启动了锁定设置.
                            else
                                this.Pub1.Add(
                                    "<TD border=0><img src='../Img/Btn/Delete.gif' onclick=\"javascript:Del('" + dtl.OID +
                                    "','" + this.EnsName + "','" + this.RefPKVal + "','" + this.PageIdx + "')\" /></TD>");
                        }
                        else if (mdtl.IsDelete)
                        {
                            if (this.IsReadonly == 0)
                                this.Pub1.Add("<TD class=TD border=0>&nbsp;</TD>");

                        }
                        this.Pub1.AddTREnd();

                        #endregion 增加rows
                    }

                    #endregion 增加该分组的数据.
                }
            }
            else
            {
                foreach (BP.Sys.GEDtl dtl in dtls)
                {
                    #region 处理

                    if (ids.Contains("," + dtl.OID + ","))
                        continue;

                    ids += dtl.OID + ",";
                    this.Pub1.AddTR();

                    //if (dtlsNum == idx && mdtl.IsShowIdx && mdtl.IsInsert && this.IsReadonly == 0)
                    //{
                    //    DDL myAdd = new DDL();
                    //    myAdd.AutoPostBack = true;
                    //    myAdd.Items.Add(new ListItem("+", "+"));
                    //    for (int i = 1; i < 10; i++)
                    //    {
                    //        myAdd.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    //    }
                    //    myAdd.SelectedIndexChanged += new EventHandler(myAdd_SelectedIndexChanged);
                    //    this.Pub1.AddTD(myAdd);
                    //}
                    //else
                    //{
                    if (mdtl.IsShowIdx)
                    {
                        this.Pub1.AddTDIdx(idx++);
                    }
                    else
                        this.Pub1.AddTD();
                    //}

                    #endregion 处理

                    #region 增加rows
                    foreach (MapAttr attr in attrs)
                    {
                        if (attr.UIVisible == false || attr.KeyOfEn == "OID")
                            continue;

                        //只读
                        if (this.IsReadonly == 1)
                            attr.UIIsEnable = false;
                        ////处理它的默认值.
                        //if (attr.DefValReal.Contains("@") == true && attr.UIIsEnable == false)
                        //    dtl.SetValByKey(attr.KeyOfEn, attr.DefVal);

                        string val = dtl.GetValByKey(attr.KeyOfEn).ToString();
                        if (attr.UIIsEnable == false && dtl.OID >= 100 && LinkFields.Contains("," + attr.KeyOfEn + ","))
                        {
                            if (string.IsNullOrEmpty(val))
                                val = "...";
                            MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link,
                                MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;

                            string url = meLink.Tag.Clone() as string;
                            if (url.Contains("?") == false)
                                url = url + "?a3=2";

                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + mdtl.No +
                                  "&OID=" + dtl.OID;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath",
                                    "http://" + this.Request.Url.Host + this.Request.ApplicationPath);
                            if (url.Contains("@"))
                            {
                                if (attrs2.Count == 0)
                                    attrs2 = new MapAttrs(mdtl.No);
                                foreach (MapAttr item in attrs2)
                                {
                                    url = url.Replace("@" + item.KeyOfEn, dtl.GetValStrByKey(item.KeyOfEn));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                                if (url.Contains("@"))
                                {
                                    /*可能是主表也要有参数*/
                                    if (mainEn == null)
                                        mainEn = this.MainEn;
                                    foreach (Attr attrM in mainEn.EnMap.Attrs)
                                    {
                                        url = url.Replace("@" + attrM.Key, mainEn.GetValStrByKey(attrM.Key));
                                        if (url.Contains("@") == false)
                                            break;
                                    }
                                }
                            }
                            this.Pub1.AddTD("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + val + "</a>");
                            continue;
                        }

                        switch (attr.UIContralType)
                        {
                            case UIContralType.TB:
                                TextBox tb = new TextBox();
                                tb.ID = "TB_" + attr.KeyOfEn + "_" + dtl.OID;
                                //tb.Enabled = attr.UIIsEnable;
                                if (attr.UIIsEnable == false)
                                {
                                    tb.Attributes.Add("readonly", "true");
                                    tb.CssClass = "TBReadonly";
                                }
                                else
                                {
                                    tb.Attributes["onfocus"] = "SetChange(true);";
                                }
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppString:
                                        tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                        this.Pub1.AddTD("width='2px'", tb);
                                        tb.Text = val;
                                        if (attr.UIIsEnable == false)
                                        {
                                            tb.Attributes.Add("readonly", "true");
                                            tb.CssClass = "TBReadonly";
                                        }

                                        if (attr.UIHeight > 25)
                                        {
                                            tb.TextMode = TextBoxMode.MultiLine;
                                            tb.Attributes["Height"] = attr.UIHeight + "px";
                                            tb.Rows = attr.UIHeightInt / 25;
                                        }
                                        break;
                                    case DataType.AppDate:
                                        float dateWidth = attr.UIWidth == 100 ? 85 : attr.UIWidth;

                                        tb.Attributes["style"] = "width:" + dateWidth + "px;border-width:0px;";
                                        if (val != "0")
                                            tb.Text = val;
                                        tb.Attributes["readonly"] = "readonly";
                                        if (attr.UIIsEnable)
                                        {
                                            tb.Attributes["onfocus"] = "WdatePicker();SetChange(false);";
                                            tb.Attributes["onChange"] = "SetChange(true);";
                                            tb.Attributes["class"] = "Wdate";

                                            //tb.CssClass = "easyui-datebox";
                                            //tb.Attributes["data-options"] = "editable:false";
                                        }
                                        else
                                        {
                                            tb.ReadOnly = true;
                                        }
                                        this.Pub1.AddTD("width='2px'", tb);
                                        break;
                                    case DataType.AppDateTime:
                                        float dateTimeWidth = attr.UIWidth == 100 ? 125 : attr.UIWidth;

                                        tb.Attributes["style"] = "width:" + dateTimeWidth + "px;border-width:0px;";
                                        if (val != "0")
                                            tb.Text = val;
                                        if (attr.UIIsEnable)
                                        {
                                            tb.Attributes["onfocus"] =
                                                "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});SetChange(false);";
                                            //tb.CssClass = "easyui-datetimebox";
                                            //tb.Attributes["data-options"] = "editable:false";
                                            tb.Attributes["onChange"] = "SetChange(true);";
                                            tb.Attributes["class"] = "Wdate";
                                        }
                                        else
                                        {
                                            tb.ReadOnly = true;
                                        }
                                        this.Pub1.AddTD("width='2px'", tb);
                                        break;
                                    case DataType.AppInt:
                                        tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                        if (attr.UIIsEnable == false)
                                        {
                                            tb.Attributes["class"] = "TBNumReadonly";
                                            tb.ReadOnly = true;
                                        }
                                        else
                                        {
                                            tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                        }
                                        try
                                        {
                                            tb.Text = val;
                                        }
                                        catch (Exception ex)
                                        {
                                            this.Alert(ex.Message + " val =" + val);
                                            tb.Text = "0";
                                        }
                                        this.Pub1.AddTD(tb);
                                        break;
                                    case DataType.AppMoney:
                                        tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                        if (attr.UIIsEnable == false)
                                        {
                                            tb.Attributes["class"] = "TBNumReadonly";
                                            tb.ReadOnly = true;
                                        }
                                        else
                                        {
                                            tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                        }
                                        try
                                        {
                                            tb.Text = decimal.Parse(val).ToString("0.00");
                                        }
                                        catch (Exception ex)
                                        {
                                            this.Alert(ex.Message + " val =" + val);
                                            tb.Text = "0.00";
                                        }
                                        this.Pub1.AddTD(tb);
                                        break;
                                    default:
                                        tb.Attributes["style"] = "width:" + attr.UIWidth +
                                                                 "px;text-align:right;border-width:0px;";
                                        tb.Text = val;
                                        this.Pub1.AddTD(tb);
                                        break;
                                }

                                if (attr.IsNum && attr.LGType == FieldTypeS.Normal)
                                {
                                    if (tb.Enabled)
                                    {
                                        tb.Attributes["onchange"] += @"C" + attr.KeyOfEn + "()";
                                        // OnKeyPress="javascript:return VirtyNum(this);"
                                        if (attr.MyDataType == DataType.AppInt)
                                        {
                                            tb.Attributes["onkeyup"] += @"C" + dtl.OID + "();C" + attr.KeyOfEn + "(); ";
                                            tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'int');";
                                            tb.Attributes["onblur"] += @"value=value.replace(/[^-?\d]/g,'');C" + dtl.OID +
                                                                       "();C" + attr.KeyOfEn + "();";
                                        }
                                        else
                                        {
                                            tb.Attributes["onkeyup"] += @"C" + dtl.OID + "();C" + attr.KeyOfEn + "();  ";
                                            tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'float');";

                                            tb.Attributes["onblur"] += @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');C" +
                                                                       dtl.OID + "();C" + attr.KeyOfEn + "();";
                                        }
                                        tb.Attributes["style"] = "width:" + attr.UIWidth +
                                                                 "px;text-align:right;border-width:0px;";
                                    }
                                    else
                                    {
                                        tb.Attributes["onpropertychange"] += "C" + attr.KeyOfEn + "();";
                                        tb.Attributes["style"] = "width:" + attr.UIWidth +
                                                                 "px;text-align:right;border-width:0px;";
                                    }
                                }
                                break;
                            case UIContralType.DDL:
                                switch (attr.LGType)
                                {
                                    case FieldTypeS.Enum:
                                        DDL myddl = new DDL();
                                        myddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                        myddl.Attributes["onchange"] = "SetChange (true);";
                                        if (attr.UIIsEnable)
                                        {
                                            try
                                            {
                                                myddl.BindSysEnum(attr.KeyOfEn);
                                                myddl.SetSelectItem(val);
                                            }
                                            catch (Exception ex)
                                            {
                                                BP.Sys.PubClass.Alert(ex.Message);
                                            }
                                        }
                                        else
                                        {
                                            myddl.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.KeyOfEn),
                                                dtl.GetValStrByKey(attr.KeyOfEn)));
                                        }
                                        myddl.Enabled = attr.UIIsEnable;
                                        this.Pub1.AddTDCenter(myddl);
                                        break;
                                    case FieldTypeS.FK:
                                        DDL ddl1 = new DDL();
                                        ddl1.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                        ddl1.Attributes["onchange"] = "SetChange (true);";
                                        ddl1.Attributes["onfocus"] = "SetChange(true);";
                                        if (attr.UIIsEnable)
                                        {
                                            DataTable mydt = this.HTTemp[attr.KeyOfEn] as DataTable;
                                            if (mydt == null)
                                            {
                                                #region 检查是否有 sql 过滤填充?
                                                foreach (MapExt me in mes)
                                                {
                                                    if (me.AttrOfOper != attr.KeyOfEn || me.ExtType != MapExtXmlList.AutoFullDLL)
                                                        continue;

                                                    //如果没有数据了，就删除他.
                                                    if (string.IsNullOrEmpty(me.Doc) == true)
                                                    {
                                                        me.Delete();
                                                        continue;
                                                    }

                                                    Hashtable htdtl = dtl.Row;
                                                    foreach (string key in this.Request.QueryString.Keys)
                                                    {
                                                        if (string.IsNullOrEmpty(key) == true)
                                                            continue;
                                                        if (htdtl.ContainsKey(key) == true)
                                                            continue;
                                                        htdtl.Add(key, this.Request.QueryString[key]);
                                                    }

                                                    string fullSQL = me.AutoFullDLL_SQL_ForDtl(htdtl, this.MainEn.Row);
                                                    // dtl.GetValStrByKey(me.AttrOfOper);

                                                    mydt = DBAccess.RunSQLReturnTable(fullSQL);
                                                    this.HTTemp[attr.KeyOfEn] = mydt;
                                                }
                                                #endregion

                                                if (mydt == null)
                                                {
                                                    EntitiesNoName ens = attr.HisEntitiesNoName;
                                                    //ens.RetrieveAll();
                                                    mydt = ens.ToDataTableField();
                                                    HTTemp[attr.KeyOfEn] = mydt;
                                                }
                                            }

                                            if (ddl1.Bind(mydt, "No","Name", val) == false)
                                            {
                                                if (mydt.Rows.Count >= 1)
                                                {
                                                    DataRow dr = mydt.Rows[0]; // en = ens[0] as Entity;
                                                    if (  dr[1].ToString().Contains("请选择") == false)
                                                    {
                                                        ddl1.Items.Insert(0, new ListItem("请选择", val));
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            val = dtl.GetValStrByKey(attr.KeyOfEn);
                                            string text = dtl.GetValRefTextByKey(attr.KeyOfEn);

                                            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(val) == false)
                                            {
                                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                                EntityNoName enn = ens.GetEntityByKey(val) as EntityNoName;

                                                if (enn != null)
                                                {
                                                    text = enn.Name;
                                                }
                                                else
                                                {
                                                    Entity myen = ens.GetNewEntity;
                                                    myen.PKVal = val;
                                                    if (myen.RetrieveFromDBSources() != 0)
                                                        text = myen.GetValStringByKey("Name");
                                                    else
                                                        text = val;
                                                }
                                            }

                                            ddl1.Items.Add(new ListItem(text, val));
                                            ddl1.Enabled = false;
                                        }
                                        this.Pub1.AddTDCenter(ddl1);
                                        break;
                                    case FieldTypeS.Normal:
                                         DDL ddlNormal = new DDL();
                                         ddlNormal.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                        if (attr.UIIsEnable)
                                        {
                                            ddlNormal.Attributes["onchange"] = "SetChange (true);";
                                            ddlNormal.Attributes["onfocus"] = "SetChange(true);";

                                            //ddlNormal.BindEntities(ens);
                                            if (ddlNormal.Bind(attr.HisDT, "No", "Name", val) == false)
                                            {

                                                if (attr.HisDT.Rows.Count >= 1)
                                                {
                                                    DataRow dr = attr.HisDT.Rows[0];
                                                    if (dr["Name"].ToString().Contains("请选择") == false)
                                                    {
                                                        ddlNormal.Items.Insert(0, new ListItem("请选择", val));
                                                    }
                                                }
                                                ddlNormal.SelectedIndex = 0;
                                                //ddlNormal.Items.Insert(0, new ListItem("请选择", val));
                                            }
                                        }
                                        else
                                        {
                                            ddlNormal.Enabled = false;
                                            string t = dtl.GetValStrByKey(attr.KeyOfEn + "T");
                                            if (t == "")
                                                t = "无";
                                            ddlNormal.Items.Add(new ListItem(t,dtl.GetValStrByKey(attr.KeyOfEn)));
                                        }
                                        this.Pub1.AddTDCenter(ddlNormal);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case UIContralType.CheckBok:
                                cb = new CheckBox();
                                cb.ID = "CB_" + attr.KeyOfEn + "_" + dtl.OID;
                                cb.Text = attr.Name;
                                if (val == "1")
                                    cb.Checked = true;
                                else
                                    cb.Checked = false;

                                if (attr.UIIsEnable)
                                    cb.Attributes["onclick"] = " SetChange(true);";

                                cb.Enabled = attr.UIIsEnable;
                                this.Pub1.AddTD(cb);
                                break;
                            default:
                                break;
                        }
                    }

                    if (mdtl.IsEnableAthM)
                    {
                        if (dtl.OID >= 100)
                        {
                            int num = athDBs.GetCountByKey(FrmAttachmentDBAttr.RefPKVal, dtl.OID);
                            this.Pub1.AddTD(
                                "<a href=\"javascript:window.showModalDialog('AttachmentUpload.aspx?IsBTitle=1&PKVal=" +
                                dtl.OID + "&Ath=AthMDtl&FK_MapData=" + mdtl.No + "&FK_Node=" + this.FK_Node + "&FK_FrmAttachment=" + mdtl.No +
                                "_AthMDtl','dialogHeight: 300px; dialogWidth: 800px;center: yes; help: no')\"><img src='../Img/AttachmentM.png' border=0 width='16px' />(" + num + ")</a>");
                        }
                        else
                            this.Pub1.AddTD("");
                    }

                    if (mdtl.IsEnableM2M)
                    {
                        if (dtl.OID >= 100)
                            this.Pub1.AddTD(
                                "<a href=\"javascript:window.showModalDialog('M2M.aspx?IsOpen=1&NoOfObj=M2M&OID=" +
                                dtl.OID + "&FK_MapData=" + mdtl.No +
                                "','m2m','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='../Img/M2M.png' border=0 width='16px' /></a>");
                        else
                            this.Pub1.AddTD("");
                    }

                    if (mdtl.IsEnableM2MM)
                    {
                        if (dtl.OID >= 100)
                            this.Pub1.AddTD(
                                "<a href=\"javascript:window.showModalDialog('M2MM.aspx?IsOpen=1&NoOfObj=M2MM&OID=" +
                                dtl.OID + "&FK_MapData=" + mdtl.No +
                                "','m2m','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='../Img/M2M.png' border=0 width='16px' /></a>");
                        else
                            this.Pub1.AddTD("");
                    }

                    if (mdtl.IsEnableLink)
                    {
                        string url = mdtl.LinkUrl.Clone() as string;
                        if (url.Contains("?") == false)
                            url = url + "?a3=2";
                        url = url.Replace("*", "@");

                        if (url.Contains("OID="))
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + mdtl.No;
                        else
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + mdtl.No +
                                  "&OID=" + dtl.OID;

                        if (url.Contains("@AppPath"))
                            url = url.Replace("@AppPath",
                                "http://" + this.Request.Url.Host + this.Request.ApplicationPath);

                        url = BP.WF.Glo.DealExp(url, dtl, null);
                        url = url.Replace("@OID", dtl.OID.ToString());
                        url = url.Replace("@FK_Node", this.FK_Node.ToString());
                        url = url.Replace("'", "");

                        if (dtl.OID >= 100)
                            this.Pub1.AddTD("<a href=\"" + url + "\" target='" + mdtl.LinkTarget + "' >" +
                                            mdtl.LinkLabel + "</a>");
                        else
                            this.Pub1.AddTD("");
                    }

                    if (mdtl.IsDelete && this.IsReadonly == 0 && dtl.OID >= 100)
                    {
                        if (isRowLock == true && dtl.IsRowLock == true)
                            this.Pub1.AddTD("<img src='../Img/Btn/Lock.png' class=ICON />"); //如果当前记录是锁定的，并且启动了锁定设置.
                        else
                            this.Pub1.Add("<TD border=0><img src='../Img/Btn/Delete.gif' onclick=\"javascript:Del('" +
                                          dtl.OID + "','" + this.EnsName + "','" + this.RefPKVal + "','" + this.PageIdx +
                                          "')\" /></TD>");
                    }
                    else if (mdtl.IsDelete)
                    {
                        if (this.IsReadonly == 0)
                            this.Pub1.Add("<TD class=TD border=0>&nbsp;</TD>");
                    }
                    this.Pub1.AddTREnd();

                    #endregion 增加rows
                }
                if (mdtl.IsInsert && this.IsReadonly == 0)
                {
                    this.Pub1.AddTR();
                    DDL myAdd = new DDL();
                    myAdd.AutoPostBack = true;
                    myAdd.Items.Add(new ListItem("+", "+"));
                    for (int i = 1; i < 10; i++)
                    {
                        myAdd.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    myAdd.SelectedIndexChanged += new EventHandler(myAdd_SelectedIndexChanged);
                    this.Pub1.AddTD(myAdd);
                    foreach (MapAttr attr in attrs)
                    {
                        if (attr.UIVisible == false || attr.KeyOfEn == "OID")
                            continue;

                        ////处理它的默认值.
                        //if (attr.DefValReal.Contains("@") == true && attr.UIIsEnable == false)
                        //    dtl.SetValByKey(attr.KeyOfEn, attr.DefVal);
                        this.Pub1.AddTD("");
                    }

                    if (mdtl.IsDelete)
                    {
                        if (this.IsReadonly == 0)
                            this.Pub1.Add("<TD class=TD border=0>&nbsp;</TD>");
                    }
                    this.Pub1.AddTREnd();

                }
            }

            #region 拓展属性
            if (this.IsReadonly == 0 && mes.Count != 0)
            {
                this.Page.RegisterClientScriptBlock("s81",
              "<script language='JavaScript' src='../Scripts/jquery-1.4.1.min.js' ></script>");

                this.Page.RegisterClientScriptBlock("b81",
             "<script language='JavaScript' src='MapExt.js' defer='defer' type='text/javascript' ></script>");

                this.Pub1.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");

                this.Page.RegisterClientScriptBlock("dCd",
    "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "DataUser/JSLibData/" + mdtl.No + ".js' ></script>");

                foreach (BP.Sys.GEDtl mydtl in dtls)
                {
                    //ddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                    foreach (MapExt me in mes)
                    {
#warning 此处需要优化
                        string ddlC = "DDL_" + me.AttrOfOper.Replace(me.FK_MapData,"") + "_" + mydtl.OID;
                        switch (me.ExtType)
                        {
                            case MapExtXmlList.DDLFullCtrl: // 自动填充.
                                DDL ddlOper = this.Pub1.GetDDLByID(ddlC);
                                if (ddlOper == null)
                                    continue;
                                ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                                break;
                            case MapExtXmlList.ActiveDDL:
                                DDL ddlPerant = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                string val, valC;
                                DataTable dt;
                                if (ddlPerant == null)
                                    continue;
#warning 此处需要优化
                                ddlC = "Pub1_DDL_" + me.AttrsOfActive + "_" + mydtl.OID;
                                ddlPerant.Attributes["onchange"] = " SetChange (true); DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\','" + mydtl.OID+ "')";
                                DDL ddlChild = this.Pub1.GetDDLByID("DDL_" + me.AttrsOfActive + "_" + mydtl.OID);
                                val = ddlPerant.SelectedItemStringVal;
                                if (ddlChild.Items.Count == 0)
                                    valC = mydtl.GetValStrByKey(me.AttrsOfActive);
                                else
                                    valC = ddlChild.SelectedItemStringVal;

                                string mysql = me.Doc.Replace("@Key", val);
                                if (mysql.Contains("@") && mydtl.OID >= 100)
                                {
                                    mysql = BP.WF.Glo.DealExp(mysql, mydtl, null);
                                }
                                else
                                {
                                    continue;
                                }

                                dt = DBAccess.RunSQLReturnTable(mysql);

                                ddlChild.Bind(dt, "No", "Name");
                                if (ddlChild.SetSelectItem(valC) == false)
                                {
                                    if (dt.Rows.Count >= 1)
                                    {
                                        DataRow dr=dt.Rows[0];
                                        if (dr["Name"].ToString().Contains("请选择") == false)
                                        {
                                            ddlChild.Items.Insert(0, new ListItem("请选择", val));
                                        }
                                    }
                                    ddlChild.SelectedIndex = 0;
                                    //ddlChild.Items.Insert(0, new ListItem("请选择" + valC, valC));
                                }
                                ddlChild.Attributes["onchange"] = " SetChange (true);";
                                break;
                            case MapExtXmlList.AutoFullDLL: //自动填充下拉框的范围.
                                //DDL ddlFull = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                //if (ddlFull == null)
                                //    continue;

                                //string fullSQL = mydtl.GetValStrByKey(me.AttrOfOper);
                                //string valOld =ddlFull.SelectedItemStringVal;
                                //Hashtable htdtl = mydtl.Row;
                                //foreach (string key in this.Request.QueryString.Keys)
                                //{
                                //    if (htdtl.ContainsKey(key) == true)
                                //        continue;
                                //    htdtl.Add(key, this.Request.QueryString[key]);
                                //}
                                  
                                //// 宋洪刚 解决如果是退回状态，并且设置自动填充没有值情况下则不进行自动填充！
                                //DataTable autoFullTable = DBAccess.RunSQLReturnTable(fullSQL);
                                //bool isHaveValue = string.IsNullOrEmpty(valOld) ? true : false;
                                //if (autoFullTable.Rows.Count > 0 || isHaveValue)
                                //{

                                //    ddlFull.Items.Clear();
                                //    ddlFull.Bind(autoFullTable, "No", "Name");
                                //    if (ddlFull.SetSelectItem(valOld) == false)
                                //    {
                                //        if (autoFullTable.Rows.Count >= 1)
                                //        {
                                //            DataRow dr = autoFullTable.Rows[0];
                                //            if (dr["Name"].ToString().Contains("请选择") == false)
                                //            {
                                //                ddlFull.Items.Insert(0, new ListItem("请选择", valOld));
                                //            }
                                //        }
                                //        ddlFull.SelectedIndex = 0;

                                //        //ddlFull.Items.Insert(0, new ListItem("请选择" + valOld, valOld));
                                //        //ddlFull.SelectedIndex = 0;
                                //    }
                                //}
                                //ddlFull.Attributes["onchange"] = " SetChange (true);";
                                break;
                            case MapExtXmlList.TBFullCtrl: // 自动填充.
                                TextBox tbAuto = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (tbAuto == null)
                                    continue;
                                tbAuto.Attributes["onkeyup"] = "SetChange (true); DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                                tbAuto.Attributes["AUTOCOMPLETE"] = "OFF";
                                if (me.Tag != "")
                                {
                                    /* 处理下拉框的选择范围的问题 */
                                    string[] strs = me.Tag.Split('$');
                                    foreach (string str in strs)
                                    {
                                        string[] myCtl = str.Split(':');
                                        string ctlID = myCtl[0];
                                        DDL ddlC1 = this.Pub1.GetDDLByID("DDL_" + ctlID + "_" + mydtl.OID);
                                        if (ddlC1 == null)
                                        {
                                            //me.Tag = "";
                                            // me.Update();
                                            continue;
                                        }

                                        string sql = myCtl[1].Replace("~", "'");
                                        sql = sql.Replace("@WebUser.No", WebUser.No);
                                        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                                        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                        sql = sql.Replace("@Key", tbAuto.Text.Trim());
                                        dt = DBAccess.RunSQLReturnTable(sql);
                                        string valC1 = ddlC1.SelectedItemStringVal;
                                        ddlC1.Items.Clear();
                                        foreach (DataRow dr in dt.Rows)
                                            ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                        ddlC1.SetSelectItem(valC1);
                                    }
                                }
                                break;
                            case MapExtXmlList.InputCheck:
                                TextBox tbCheck = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (tbCheck != null)
                                    tbCheck.Attributes[me.Tag2] += " rowPK=" + mydtl.OID + "; " + me.Tag1 + "(this);";
                                break;
                            case MapExtXmlList.PopVal: //弹出窗.
                                try
                                {
                                    TextBox tb = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);

                                    // tb.Attributes["ondblclick"] = " SetChange(true); ReturnVal(this,'" + me.Doc + "','sd');";
                                    // edit by stone 2015-03-08 解决，明细表不能弹出pop值的问题.
                                    tb.Attributes["onfocus"] = "SetChange(false);";
                                    tb.Attributes["ondblclick"] = " SetChange(false);ReturnValCCFormPopVal(this,'" + me.MyPK + "','" + mydtl.OID + "');SetChange (true);";
                                }
                                catch
                                {
                                }


                                break;
                            case MapExtXmlList.Link: // 超链接.

                                //TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                //tb.Attributes["ondblclick"] = " SetChange(true); ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.RegularExpression://正则表达式,对数据控件处理
                                TextBox tbExp = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (tbExp == null || me.Tag == "onsubmit")
                                    continue;
                                //验证输入的正则格式
                                string regFilter = me.Doc;
                                if (regFilter.LastIndexOf("/g") < 0 && regFilter.LastIndexOf('/') < 0)
                                    regFilter = "'" + regFilter + "'";
                                //处理事件
                                tbExp.Attributes.Add("" + me.Tag + "", "return txtTest_Onkeyup(this," + regFilter + ",'" + me.Tag1 + "')");//[me.Tag] += "this.value=this.value.replace(" + regFilter + ",'')";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion 拓展属性

            #region 生成合计
            if (mdtl.IsShowSum && dtls.Count > 1)
            {
                this.Pub1.AddTRSum();
                this.Pub1.AddTD();
                foreach (MapAttr attr in attrs)
                {
                    // 分组字段或隐藏字段不显示 [liold 140602]
                    if (attr.UIVisible == false)
                        continue;
                    if (attr.Field == mdtl.GroupField && mdtl.IsEnableGroupField)
                        continue;

                    if (attr.IsNum && attr.LGType == FieldTypeS.Normal && attr.IsSum)
                    {
                        TextBox tb = new TextBox();
                        tb.ID = "TB_" + attr.KeyOfEn;
                        tb.Text = attr.DefVal;
                        // tb.ShowType = attr.HisTBType;
                        tb.ReadOnly = true;
                        tb.Font.Bold = true;
                        tb.BackColor = System.Drawing.Color.FromName("infobackground");
                        switch (attr.MyDataType)
                        {
                            case DataType.AppMoney:
                                tb.Text = dtls.GetSumDecimalByKey(attr.KeyOfEn).ToString("0.00");
                                tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border:none";
                                break;
                            case DataType.AppInt:
                                tb.Text = dtls.GetSumIntByKey(attr.KeyOfEn).ToString();
                                tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border:none";
                                break;
                            case DataType.AppFloat:
                                tb.Text = dtls.GetSumFloatByKey(attr.KeyOfEn).ToString();
                                tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border:none";
                                break;
                            default:
                                break;
                        }
                        this.Pub1.AddTD("align=right", tb);
                    }
                    else
                    {
                        this.Pub1.AddTD();
                    }
                }
                if (mdtl.IsEnableAthM)
                    this.Pub1.AddTD();

                if (mdtl.IsEnableM2M)
                    this.Pub1.AddTD();

                if (mdtl.IsEnableM2MM)
                    this.Pub1.AddTD();

                if (mdtl.IsReadonly == false)
                    this.Pub1.AddTD();

                this.Pub1.AddTREnd();
            }
            #endregion 生成合计

            #endregion 生成数据

            this.Pub1.AddTableEnd();

            #region 生成 自动计算行
            if (this.IsReadonly == 0)
            {
                // 输出自动计算公式
                this.Response.Write("\n<script language='JavaScript'>");
                MapExts exts = new MapExts(mdtl.No);
                foreach (GEDtl dtl in dtls)
                {
                    string top = "\n function C" + dtl.OID + "() { \n ";
                    string script = "";
                    string end = " \n  } ";
                    //添加函数
                    if (exts == null || exts.Count == 0)
                    {
                        this.Response.Write(top + script + end);
                        continue;
                    }
                    foreach (MapExt ext in exts)
                    {
                        if (ext.ExtType != MapExtXmlList.AutoFull)
                        {
                            this.Response.Write(top + script + end);
                            continue;
                        }

                        foreach (MapAttr attr in attrs)
                        {
                            if (attr.UIVisible == false)
                                continue;
                            if (attr.IsNum == false)
                                continue;
                            if (attr.LGType != FieldTypeS.Normal)
                                continue;

                            if (ext.ExtType == MapExtXmlList.AutoFull && ext.Doc != "")
                            {
                                script += this.GenerAutoFull(dtl.OID.ToString(), attrs, ext);
                            }
                        }
                        this.Response.Write(top + script + end);
                    }
                }
                this.Response.Write("\n</script>");

                // 输出合计算计公式
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.LGType != FieldTypeS.Normal)
                        continue;

                    if (attr.IsNum == false)
                        continue;

                    string top = "\n<script language='JavaScript'> function C" + attr.KeyOfEn + "() { \n ";
                    string end = "\n  isChange =true ;  } </script>";
                    this.Response.Write(top + this.GenerSum(attr, dtls) + " ; \t\n" + end);
                }
            }
            #endregion

        }
        bool isAddDDLSelectIdxChange = false;
        void myAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            string val = ddl.SelectedItemStringVal;
            string url = "";
            isAddDDLSelectIdxChange = true;
            this.Save();
            try
            {
                int addRow = int.Parse(ddl.SelectedItemStringVal.Replace("+", "").Replace("-", ""));
                _allRowCount += addRow;
            }
            catch
            {

            }

            if (val.Contains("+"))
                url = "Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx + "&rowCount=" + _allRowCount + "&AddRowNum=" + ddl.SelectedItemStringVal.Replace("+", "").Replace("-", "") + "&IsCut=0&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node + "&Key=" + this.Request.QueryString["Key"]+"&FID="+this.FID+"&PWorkID="+this.Request.QueryString["PWorkID"];
            else
                url = "Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx + "&rowCount=" + _allRowCount + "&AddRowNum=" + ddl.SelectedItemStringVal.Replace("+", "").Replace("-", "") + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node + "&Key=" + this.Request.QueryString["Key"] + "&FID=" + this.FID + "&PWorkID=" + this.Request.QueryString["PWorkID"];

            this.Response.Redirect(url, true);
        }
        public void Save()
        {
            MapDtl mdtl = new MapDtl(this.EnsName);
            GEDtls dtls = new GEDtls(this.EnsName);
            FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.
            GEEntity mainEn = null;

            #region 从表保存前处理事件.
            if (fes.Count > 0)
            {
                try
                {
                    mainEn = mdtl.GenerGEMainEntity(this.RefPKVal);
                    string msg = fes.DoEventNode(EventListDtlList.DtlSaveBefore, mainEn);
                    if (msg != null)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 从表保存前处理事件.

            QueryObject qo = null;
            try
            {
                qo = new QueryObject(dtls);
                switch (mdtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        qo.addAnd();
                        qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        break;
                    case DtlOpenType.ForFID: // 按流程ID来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, this.FID);
                        break;
                }
            }
            catch(Exception ex)
            {
                dtls.GetNewEntity.CheckPhysicsTable();
                throw ex;
            }

            int num = 0;
            if (mdtl.HisWhenOverSize == WhenOverSize.TurnPage)
                num = qo.DoQuery("OID", mdtl.RowsOfList, this.PageIdx, false);
            else
                num = qo.DoQuery();
            int dtlCount = dtls.Count;
            if (_allRowCount == 0)
            {
                if (mdtl.HisWhenOverSize != WhenOverSize.TurnPage)
                    mdtl.RowsOfList = mdtl.RowsOfList + this.addRowNum;
            }
            else
            {
                if (mdtl.HisWhenOverSize != WhenOverSize.TurnPage)
                    mdtl.RowsOfList = _allRowCount;
            }

            for (int i = 0; i < mdtl.RowsOfList - dtlCount; i++)
            {
                BP.Sys.GEDtl dt = new GEDtl(this.EnsName);
                dt.ResetDefaultVal();
                dt.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                dt.OID = i;
                dtls.AddEntity(dt);
            }

            Map map = dtls.GetNewEntity.EnMap;
            bool isTurnPage = false;
            string err = "";
            int idx = 0;

            // 判断是否有事件.
            bool isHaveBefore = true;
            bool isHaveEnd = true;
            FrmEvent fe_Before = fes.GetEntityByKey(FrmEventAttr.FK_Event, EventListDtlList.DtlItemSaveBefore) as FrmEvent;
            if (fe_Before == null)
                isHaveBefore = false;

            FrmEvent fe_End = fes.GetEntityByKey(FrmEventAttr.FK_Event, EventListDtlList.DtlItemSaveAfter) as FrmEvent;
            if (fe_End == null)
                isHaveEnd = false;

            //...................................
            bool isRowLock = mdtl.IsRowLock;
            foreach (GEDtl dtl in dtls)
            {
                idx++;
                try
                {
                    this.Pub1.Copy(dtl, dtl.OID.ToString(), map);

                    //如果是行锁定,就不执行.
                    if (isRowLock == true && dtl.IsRowLock)
                        continue;

                    #region 给系统变量赋值, 经常遇到是 null 的情况.
                    if (mdtl.DtlOpenType == DtlOpenType.ForFID)
                        dtl.RefPK = this.FID.ToString();
                    else
                        dtl.RefPK = this.RefPKVal;

                    //给FID 赋值.
                    if (this.FID == 0)
                        dtl.FID = Int64.Parse(this.RefPKVal);
                    else
                        dtl.FID = this.FID;
                    #endregion


                    if (dtl.OID < mdtl.RowsOfList + 2)
                    {
                        int myOID = dtl.OID;
                        dtl.OID = 0;
                        if (dtl.IsBlank)
                            continue;

                        dtl.OID = myOID;
                        if (dtl.OID == mdtl.RowsOfList + 1)
                            isTurnPage = true;

                        if (isHaveBefore)
                        {
                            try
                            {
                                string r = fes.DoEventNode(EventListDtlList.DtlItemSaveBefore, dtl);
                                if (r == "false" || r == "0")
                                    continue;
                                err += r;
                            }
                            catch (Exception ex)
                            {
                                err += ex.Message;
                                continue;
                            }
                        }
                        dtl.InsertAsOID(DBAccess.GenerOID("Dtl"));
                    }
                    else
                    {
                        if (this.FID != 0)
                            dtl.FID = this.FID;

                        if (isHaveBefore)
                        {
                            try
                            {
                                err += fes.DoEventNode(EventListDtlList.DtlItemSaveBefore, dtl);
                            }
                            catch (Exception ex)
                            {
                                err += ex.Message;
                                continue;
                            }
                        }
                        dtl.Update();
                    }

                    #region 执行保存后的事件.
                    if (isHaveEnd)
                    {
                        /* 如果有保存后的事件。*/
                        try
                        {
                            fes.DoEventNode(EventListDtlList.DtlItemSaveAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            err += ex.Message;
                        }
                    }
                    #endregion 执行保存后的事件.

                }
                catch (Exception ex)
                {
                    dtl.CheckPhysicsTable();
                    err += "Row: " + idx + " Error \r\n" + ex.Message;
                }
            }

            if (err != "")
            {
                BP.DA.Log.DefaultLogWriteLineInfo(err);
                this.Alert(err);
                return;
            }

            if (isAddDDLSelectIdxChange == true)
                return;

            #region 从表保存后处理事件。
            if (fes.Count > 0)
            {
                try
                {
                    string msg = fes.DoEventNode(EventListDtlList.DtlSaveEnd, mainEn);
                    if (msg != null)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            string url = "";
            string[] paras = this.RequestParas.Split('&');
            foreach (string str in paras)
            {
                if (str.Contains("EnsName=") || str.Contains("RefPKVal=") || str.Contains("PageIdx")==true )
                    continue;
                url += "&"+str;
            }

            if (isTurnPage)
            {
                int pageNum = 0;
                int count = this.DtlCount + 1;
                decimal pageCountD = decimal.Parse(count.ToString()) / decimal.Parse(mdtl.RowsOfList.ToString()); // 页面个数。
                string[] strs = pageCountD.ToString("0.0000").Split('.');
                if (int.Parse(strs[1]) > 0)
                    pageNum = int.Parse(strs[0]) + 1;
                else
                    pageNum = int.Parse(strs[0]);
                this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + pageNum + url, true);
            }
            else
            {
                this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx  +url, true);
            }
        }
        public void ExpExcel()
        {
            BP.Sys.MapDtl mdtl = new MapDtl(this.EnsName);
            this.Title = mdtl.Name;
            GEDtls dtls = new GEDtls(this.EnsName);
            QueryObject qo = new QueryObject(dtls);
            switch (mdtl.DtlOpenType)
            {
                case DtlOpenType.ForEmp:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    //qo.addAnd();
                    //qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                    break;
                case DtlOpenType.ForWorkID:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    break;
                case DtlOpenType.ForFID:
                    qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                    break;
            }
            qo.DoQuery();

            // this.ExportDGToExcelV2(dtls, this.Title + ".xls");
            //DataTable dt = dtls.ToDataTableDesc();
            // this.GenerExcel(dtls.ToDataTableDesc(), mdtl.Name + ".xls");

            this.GenerExcel_pri_Text(dtls.ToDataTableDesc(), mdtl.Name + "@" + WebUser.No + "@" + DataType.CurrentData + ".xls");

            //this.ExportDGToExcelV2(dtls, this.Title + ".xls");
            //dtls.GetNewEntity.CheckPhysicsTable();
            //this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
        }
        /// <summary>
        /// 生成列的计算
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string GenerAutoFull(string pk, MapAttrs attrs, MapExt ext)
        {
            try
            {
                string left = "\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value = ";
                string right = ext.Doc;
                //首先替换参数带；号
                foreach (MapAttr mattr in attrs)
                {
                    string tbID = "TB_" + mattr.KeyOfEn + "_" + pk;
                    TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                    if (tb == null)
                        continue;

                    //添加分号，解决因字段匹配问题：@GWDZ 被字段GW替换后DZ
                    right = right.Replace("@" + mattr.Name, " parseVal2Float('" + tb.ClientID + "', '"+mattr.DefVal+"') ");
                    right = right.Replace("@" + mattr.KeyOfEn + ";", " parseVal2Float('" + tb.ClientID + "', '"+mattr.DefVal+"') ");
                }
                //适应老规则，替换不带；号的参数
                foreach (MapAttr mattr in attrs)
                {
                    string tbID = "TB_" + mattr.KeyOfEn + "_" + pk;
                    TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                    if (tb == null)
                        continue;
                    
                    //添加分号，解决因字段匹配问题：@GWDZ 被字段GW替换后DZ
                    right = right.Replace("@" + mattr.Name, " parseVal2Float('" + tb.ClientID + "', '" + mattr.DefVal + "') ");
                    right = right.Replace("@" + mattr.KeyOfEn, " parseVal2Float('" + tb.ClientID + "', '"+mattr.DefVal+"') ");
                }
                string s = left + right;
                s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value ) ;";
                return s += " C" + ext.AttrOfOper + "();";
            }
            catch
            {
                return null;
            }
        }
        public string GenerSum(MapAttr mattr, GEDtls dtls)
        {
            if (dtls.Count <= 1)
                return "";

            string ClientID = "";
            try
            {
                ClientID = this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID;
            }
            catch
            {
                return "";
            }

            string left = "\n  document.forms[0]." + ClientID + ".value = ";
            string right = "(";
            string multi = "1000000000000";
            int i = 0;
            foreach (GEDtl dtl in dtls)
            {
                string tbID = "TB_" + mattr.KeyOfEn + "_" + dtl.OID;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                if (i == 0)
                    right += " parseVal2Float('" + tb.ClientID + "', '" + mattr.DefVal + "')*" + multi;
                else
                    right += " +parseVal2Float('" + tb.ClientID + "', '" + mattr.DefVal + "')*" + multi;
                i++;
            }
            string s = left + right + ")/" + multi + " ;";
            switch (mattr.MyDataType)
            {
                case BP.DA.DataType.AppMoney:
                    return s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value ) ;";
                default:
                    return s;
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            this.Save();
        }
    }

}