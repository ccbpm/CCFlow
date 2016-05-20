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
using BP.WF;
namespace CCFlow.WF.CCForm
{
    public partial class DtlFixRow : BP.Web.WebPage
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                if (string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
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
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                if (str == null)
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

        public string rootNo
        {
            get
            {
                string _rootNo = BP.Sys.SystemConfig.AppSettings["FixRoot"];
                if (string.IsNullOrEmpty(_rootNo))
                    _rootNo = "0";
                return _rootNo;
            }
        }

        #endregion 属性.
        protected void Page_Load(object sender, EventArgs e)
        {
            MapDtl mdtl = new MapDtl(this.EnsName);

          //  this.Page.RegisterClientScriptBlock("s",
          //"<link href='" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");


            if (this.IsReadonly == 1)
            {
                mdtl._IsReadonly = 1;
                this.Button1.Enabled = false;
            }
            Bind(mdtl);
        }

        private void Bind(MapDtl mdtl)
        {

            if (string.IsNullOrEmpty(mdtl.ImpFixTreeSql) || string.IsNullOrEmpty(mdtl.ImpFixDataSql))
                throw new Exception("未配置正确的数据源SQL");

            if (this.Request.QueryString["IsTest"] != null)
                BP.DA.Cash.SetMap(this.EnsName, null);

            GEDtls dtls = new GEDtls(this.EnsName);
            GEDtl dtlSingle = new GEDtl(this.EnsName);
            dtlSingle.CheckPhysicsTable();

            this.FK_MapData = mdtl.FK_MapData;


            #region 生成标题
            MapAttrs attrs = new MapAttrs(this.EnsName);
            MapAttrs attrs2 = new MapAttrs();
            int numOfCol = 0;
            this.Pub1.Add("<Table border=0>");
            this.Pub1.Add(mdtl.MTR);
            if (mdtl.IsShowTitle)
            {
                this.Pub1.AddTR();
                this.Pub1.Add("<TD class='TitleExt'><img src='../Img/Btn/Table.gif' onclick=\"return DtlOpt('" + this.RefPKVal + "','" + this.EnsName + "','" + this.FID + "');\" border=0/></TD>");
                numOfCol++;

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

            //获得父entity.
            GEEntity mainEn = mdtl.GenerGEMainEntity(this.RefPKVal);

            #region 加载明细表的数据
            //获取明细表的数据
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
                        qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                        break;
                }
            }
            catch
            {
                dtls.GetNewEntity.CheckPhysicsTable();
            }

            //获取配置的数据源数据
            string sql = mdtl.ImpFixDataSql.Clone() as string;
            sql = BP.WF.Glo.DealExp(sql, mainEn, null);
            DataTable dataTable = BP.DA.DBAccess.RunSQLReturnTable(sql);

            //对比entity与配置的数据源数据
            ContrastEntity(dtls, dataTable, attrs);



            #region　获取树形结构数据
            //树结构SQL
            sql = mdtl.ImpFixTreeSql.Clone() as string;
            sql = BP.WF.Glo.DealExp(sql, mainEn, null);
            DataTable treeTable = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (treeTable.Columns.Contains("No") == false)
                throw new Exception("@树结构实体没有约定的列,No." + sql);
            if (treeTable.Columns.Contains("Name") == false)
                throw new Exception("@没有约定的列,Name." + sql);
            if (treeTable.Columns.Contains("ParentNo") == false)
                throw new Exception("@没有约定的列,ParentNo." + sql);

            //把树转化为标准的行政机构格式.
            DataTable myTree = BP.DA.DataType.PraseParentTree2TreeNo(treeTable, rootNo);




            //求出，关联的外键集合.
            string refKeyValues = "@";
            //foreach (DataRow dr in dataTable.Rows)
            //{
            //    if (refKeyValues.Contains("@" + dr["RefKey"] + "@") == true)
            //        continue;
            //    refKeyValues += dr["RefKey"] + "@";
            //}
            #region 重新加载数据源
            //对比数据源后重新加载数据
            QueryObject reDoQo = new QueryObject(dtls);
            reDoQo.AddWhere("RefPK", this.RefPKVal);
            reDoQo.DoQuery();

            #endregion 重新加载数据源

            #endregion 加载明细表的数据
            foreach (GEDtl dtl in dtls)
            {
                if (refKeyValues.Contains("@" + dtl.GetValStringByKey("RefTreeNo") + "@") == true)
                    continue;
                refKeyValues += dtl.GetValStringByKey("RefTreeNo") + "@";
            }

            DataTable refTreeData = myTree.Copy();
            refTreeData.Clear();
            //获取关联的树形结构
            refTreeData = GetRefTreeTable(myTree, refKeyValues, refTreeData, "0");

            #endregion　获取树形结构数据

            //将数据输出
            RendToBody(mdtl, dtls, refTreeData, rootNo, mainEn, refKeyValues);

        }

        /// <summary>
        /// 输出表格
        /// </summary>
        private void RendToBody(MapDtl mdtl, GEDtls dtls, DataTable refTreeData, string rootNo, GEEntity mainEn,
            string refKeyValues)
        {


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



            MapAttrs attrs = new MapAttrs(this.EnsName);
            MapAttrs attrs2 = new MapAttrs();

            int colspan = attrs.Count;
            string appPath = BP.WF.Glo.CCFlowAppPath; //this.Request.ApplicationPath;
            try
            {
                #region 获取数据。



                #endregion 获取数据。


                #region 循环树,生成表格.

                //获取根节点的数据
                DataTable newRefTreeData = refTreeData.Copy();
                newRefTreeData.DefaultView.RowFilter = " RefParentNo=" + rootNo;
                newRefTreeData = newRefTreeData.DefaultView.ToTable();

                int myidx = 0;
                foreach (DataRow dr in newRefTreeData.Rows)
                {
                    string no = dr["No"].ToString();
                    string name = dr["Name"].ToString();
                    string refNo = dr["RefNo"].ToString();
                    string refParentNo = dr["RefParentNo"].ToString();
                    string isDtl = dr["IsDtl"].ToString();

                    myidx++;


                    int attrCount = 0;
                    this.Pub1.AddTR();

                    foreach (MapAttr attr in attrs)
                    {
                        if (attr.UIVisible == false)
                            continue;

                        if (attr.IsPK)
                            continue;

                        //如果启用了分组，并且是当前的分组字段。
                        if (mdtl.IsEnableGroupField && mdtl.GroupField == attr.KeyOfEn)
                            continue;

                        if (attrCount == 0)
                        {
                            this.Pub1.AddTDIdx(myidx);
                            this.Pub1.AddTD(name);
                        }
                        else
                        {
                            if (attr.UIContralType == UIContralType.TB)
                            {

                                switch (attr.MyDataType)
                                {

                                    case DataType.AppDouble:
                                    case DataType.AppFloat:
                                    case DataType.AppInt:
                                    case DataType.AppMoney:
                                        string count = "0";
                                        GetChildCout(attr.KeyOfEn, no, ref count, refTreeData, attr.MyDataType);
                                        this.Pub1.AddTD("<input class='TBReadonly' id='" + no + "_" + refParentNo + "_" +
                                                        attr.KeyOfEn + "' value='" + count + "' name='" + no + "_" +
                                                        refParentNo + "'  readonly='readonly' />");
                                        break;
                                    default:
                                        this.Pub1.AddTD("");
                                        break;
                                }
                            }
                            else
                                this.Pub1.AddTD("");

                        }
                        attrCount++;
                    }
                    this.Pub1.AddTREnd();
                    //查找当前当前节点的所有子节点数据
                    FindNextNode(refTreeData, no, ref myidx, attrs, mdtl, mainEn, LinkFields, attrs2, 2);
                }

                #endregion

                #region 拓展属性

                if (this.IsReadonly == 0 && mes.Count != 0)
                {
                    this.Page.RegisterClientScriptBlock("s81",
                        "<script language='JavaScript' src='../Scripts/jquery-1.4.1.min.js' ></script>");

                    this.Page.RegisterClientScriptBlock("b81",
                        "<script language='JavaScript' src='MapExt.js' defer='defer' type='text/javascript' ></script>");

                    this.Pub1.Add(
                        "<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");

                    this.Page.RegisterClientScriptBlock("dCd",
                        "<script language='JavaScript' src='/DataUser/JSLibData/" + mdtl.No + ".js' ></script>");

                    foreach (BP.Sys.GEDtl mydtl in dtls)
                    {
                        //ddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                        foreach (MapExt me in mes)
                        {
                            switch (me.ExtType)
                            {
                                case MapExtXmlList.DDLFullCtrl: // 自动填充.
                                    DDL ddlOper = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                    if (ddlOper == null)
                                        continue;
                                    ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID +
                                                                     "\', \'" + me.MyPK + "\')";
                                    break;
                                case MapExtXmlList.ActiveDDL:
                                    DDL ddlPerant = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                    string val, valC;
                                    DataTable dt;
                                    if (ddlPerant == null)
                                        continue;
#warning 此处需要优化
                                    string ddlC = "Pub1_DDL_" + me.AttrsOfActive + "_" + mydtl.OID;
                                    ddlPerant.Attributes["onchange"] = " isChange=true; DDLAnsc(this.value, \'" + ddlC +
                                                                       "\', \'" + me.MyPK + "\')";
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
                                        mysql = mysql.Replace("@WebUser.No", WebUser.No);
                                        mysql = mysql.Replace("@WebUser.Name", WebUser.Name);
                                        mysql = mysql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                    }

                                    dt = DBAccess.RunSQLReturnTable(mysql);

                                    ddlChild.Bind(dt, "No", "Name");
                                    if (ddlChild.SetSelectItem(valC) == false)
                                    {
                                        ddlChild.Items.Insert(0, new ListItem("请选择" + valC, valC));
                                        ddlChild.SelectedIndex = 0;
                                    }
                                    ddlChild.Attributes["onchange"] = " isChange=true;";
                                    break;
                                case MapExtXmlList.AutoFullDLL: //自动填充下拉框的范围.
                                    DDL ddlFull = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                    if (ddlFull == null)
                                        continue;

                                    string valOld = mydtl.GetValStrByKey(me.AttrOfOper);
                                    //string valOld =ddlFull.SelectedItemStringVal;

                                    string fullSQL = me.Doc.Replace("@WebUser.No", WebUser.No);
                                    fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                                    fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                    fullSQL = fullSQL.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                                    fullSQL = fullSQL.Replace("@Key", this.Request.QueryString["Key"]);

                                    if (fullSQL.Contains("@"))
                                    {
                                        Attrs attrsFull = mydtl.EnMap.Attrs;
                                        foreach (Attr attr in attrsFull)
                                        {
                                            if (fullSQL.Contains("@") == false)
                                                break;
                                            fullSQL = fullSQL.Replace("@" + attr.Key, mydtl.GetValStrByKey(attr.Key));
                                        }
                                    }

                                    if (fullSQL.Contains("@"))
                                    {
                                        /*从主表中取数据*/
                                        Attrs attrsFull = this.MainEn.EnMap.Attrs;
                                        foreach (Attr attr in attrsFull)
                                        {
                                            if (fullSQL.Contains("@") == false)
                                                break;

                                            if (fullSQL.Contains("@" + attr.Key) == false)
                                                continue;

                                            fullSQL = fullSQL.Replace("@" + attr.Key,
                                                this.MainEn.GetValStrByKey(attr.Key));
                                        }
                                    }

                                    ddlFull.Items.Clear();
                                    ddlFull.Bind(DBAccess.RunSQLReturnTable(fullSQL), "No", "Name");
                                    if (ddlFull.SetSelectItem(valOld) == false)
                                    {
                                        ddlFull.Items.Insert(0, new ListItem("请选择" + valOld, valOld));
                                        ddlFull.SelectedIndex = 0;
                                    }
                                    ddlFull.Attributes["onchange"] = " isChange=true;";
                                    break;
                                case MapExtXmlList.TBFullCtrl: // 自动填充.
                                    TextBox tbAuto = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                    if (tbAuto == null)
                                        continue;
                                    tbAuto.Attributes["onkeyup"] =
                                        " isChange=true; DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID +
                                        "\', \'" + me.MyPK + "\');";
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
                                        tbCheck.Attributes[me.Tag2] += " rowPK=" + mydtl.OID + "; " + me.Tag1 +
                                                                       "(this);";
                                    break;
                                case MapExtXmlList.PopVal: //弹出窗.
                                    TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                    tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc +
                                                                  "','sd');";
                                    break;
                                case MapExtXmlList.Link: // 超链接.

                                    //TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                    //tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                    break;
                                case MapExtXmlList.RegularExpression: //正则表达式,对数据控件处理
                                    TextBox tbExp = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                    if (tbExp == null || me.Tag == "onsubmit")
                                        continue;
                                    //验证输入的正则格式
                                    string regFilter = me.Doc;
                                    if (regFilter.LastIndexOf("/g") < 0 && regFilter.LastIndexOf('/') < 0)
                                        regFilter = "'" + regFilter + "'";
                                    //处理事件
                                    tbExp.Attributes.Add("" + me.Tag + "",
                                        "return txtTest_Onkeyup(this," + regFilter + ",'" + me.Tag1 + "')");
                                        //[me.Tag] += "this.value=this.value.replace(" + regFilter + ",'')";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                #endregion 拓展属性

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

                                if (ext.Tag == "1" && ext.Doc != "")
                                {

                                    script += this.GenerAutoFull(dtl.OID.ToString(), attrs, ext);
                                    script += "SumTreeData('" + dtl.GetValStrByKey("RefTreeNo") + "','"+attr.KeyOfEn+"')";
                                }
                            }

                          
                            this.Response.Write(top + script + end);
                        }


                        //                     foreach (GEDtl dtl in dtls)
                        //                     {
                        //                         string top = "\n function C" + dtl.OID + "() { \n ";
                        //                         string script = "";
                        //                         ///MapExts exts = new MapExts(dtl.FK_MapData);
                        //                         foreach (MapAttr attr in attrs)
                        //                         {
                        //                             if (attr.UIVisible == false)
                        //                                 continue;
                        //                             if (attr.IsNum == false)
                        //                                 continue;
                        //                             if (attr.LGType != FieldTypeS.Normal)
                        //                                 continue;
                        //#warning 没有看明白这里怎么计算的.
                        //                             //if (attr.HisAutoFull == AutoFullWay.Way1_JS)
                        //                             //{
                        //                             //    script += this.GenerAutoFull(dtl.OID.ToString(), attrs, attr);
                        //                             //}
                        //                         }
                        //                         string end = " \n  } ";
                        //                         this.Response.Write(top + script + end);
                        //                     }
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

                       
                        string end = "\n  isChange =true ; } </script>";
                        this.Response.Write(top + this.GenerSum(attr, dtls) + " ; \t\n" + end);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GenerAutoFull(string pk, MapAttrs attrs, MapExt ext)
        {
            string left = "\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value = ";
            string right = ext.Doc;
            foreach (MapAttr mattr in attrs)
            {
                //if (mattr.IsNum == false)
                //    continue;

                //if (mattr.LGType != FieldTypeS.Normal)
                //    continue;

                string tbID = "TB_" + mattr.KeyOfEn + "_" + pk;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                //right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                //right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) ) ");

                right = right.Replace("@" + mattr.Name, " parseFloat( replaceAll(document.forms[0]." + tb.ClientID + ".value, ',' ,  '' ) ) ");
                right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( replaceAll(document.forms[0]." + tb.ClientID + ".value, ',' ,  '' ) ) ");
            }
            string s = left + right;
            s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + ext.AttrOfOper + "_" + pk).ClientID + ".value ) ;";
            return s += " C" + ext.AttrOfOper + "();";
        }

       /// <summary>
       /// 获取父节点的编号
       /// </summary>
       /// <param name="refTree"></param>
       /// <param name="no"></param>
       /// <returns></returns>
        private string GetPrarentTreeNo(DataTable refTree,string no)
       {
           string parentNo = "";
            foreach (DataRow row in refTree.Rows)
            {
                if (row["No"].ToString().Equals(no))
                {
                    parentNo =  row["RefParentNo"].ToString();
                    break;
                }
            }
           return parentNo;
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
            string right = "";
            int i = 0;
            foreach (GEDtl dtl in dtls)
            {
                string tbID = "TB_" + mattr.KeyOfEn + "_" + dtl.OID;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                if (i == 0)
                    right += " parseVal2Float('" + tb.ClientID + "')";
                else
                    right += " +parseVal2Float('" + tb.ClientID + "')";
                i++;
            }
            string s = left + right + " ;";
            switch (mattr.MyDataType)
            {
                case BP.DA.DataType.AppMoney:
                case BP.DA.DataType.AppRate:
                    return s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value ) ;";
                default:
                    return s;
            }
        }
        /// <summary>
        /// 获取关联的树节点
        /// </summary>
        /// <param name="dtTree"></param>
        /// <param name="refKeys"></param>
        /// <param name="key"></param>
        /// <param name="newDtTree"></param>
        /// <param name="rootNo"></param>
        /// <returns></returns>
        private DataTable GetRefTreeTable(DataTable dtTree, string refKeys, DataTable newDtTree, string rootNo)
        {

            foreach (DataRow row in dtTree.Rows)
            {
                if (refKeys.Contains("@" + row["RefNo"] + "@"))
                {
                    //查看新的数据源中是否含有了当前数据
                    DataTable copyNewDtTree = newDtTree.Copy();
                    copyNewDtTree.DefaultView.RowFilter = " RefNo=" + row["RefNo"];
                    copyNewDtTree = copyNewDtTree.DefaultView.ToTable();

                    if (copyNewDtTree.Rows.Count > 0)
                        continue;

                    DataRow newTreeRow = newDtTree.NewRow();

                    foreach (DataColumn column in dtTree.Columns)

                        if (column.ColumnName.Equals("IsDtl"))
                            newTreeRow["IsDtl"] = "1";
                        else
                            newTreeRow[column.ColumnName] = row[column.ColumnName];

                    newDtTree.Rows.Add(newTreeRow);
                    //查找当前节点的所有父节点
                    newDtTree = CopyParent2New(dtTree, newDtTree, row["RefParentNo"].ToString(), rootNo);
                }

            }
            return newDtTree;
        }

        /// <summary>
        /// 查找当前节点的所有父节点
        /// </summary>
        /// <param name="dtTree"></param>
        /// <param name="newDtTree"></param>
        /// <param name="No"></param>
        /// <returns></returns>
        private DataTable CopyParent2New(DataTable dtTree, DataTable newDtTree, string parentNo, string rootNo)
        {
            //查看数据源中是否含有此节点数据
            DataTable filterNewTree = newDtTree.Copy();
            filterNewTree.DefaultView.RowFilter = "No=" + parentNo;
            filterNewTree = filterNewTree.DefaultView.ToTable();

            //如果父节点与根节点不相同
            if (!parentNo.Equals(rootNo) && filterNewTree.Rows.Count == 0)
            {


                //查找当前节点是否有父节点
                DataTable filterTree = dtTree.Copy();
                filterTree.DefaultView.RowFilter = " No=" + parentNo;
                filterTree = filterTree.DefaultView.ToTable();

                if (filterTree.Rows.Count > 0)
                {

                    foreach (DataRow row in filterTree.Rows)
                    {
                        //将数据添加到新的数据源中
                        DataRow newTreeRow = newDtTree.NewRow();

                        foreach (DataColumn column in filterTree.Columns)
                            newTreeRow[column.ColumnName] = row[column.ColumnName];
                        newDtTree.Rows.Add(newTreeRow);
                        //查找当前借点是否有父节点
                        CopyParent2New(dtTree, newDtTree, row["RefParentNo"].ToString(), rootNo);
                    }
                }
            }


            return newDtTree;
        }

        /// <summary>
        /// 查找当前节点的下一节点 如果下一节点是dtl节点 那么就输出dtl
        /// </summary>
        /// <param name="dtTree"></param>
        /// <param name="parentNo"></param>
        private void FindNextNode(DataTable dtTree, string parentNo, ref int myidx, MapAttrs attrs, MapDtl mdtl, GEEntity mainEn, string LinkFields, MapAttrs attrs2,int idxRoot)
        {
            //判断当前节点 是否还有子节点
            DataTable newDtTree = dtTree.Copy();
            newDtTree.DefaultView.RowFilter = " RefParentNo=" + parentNo;
            newDtTree = newDtTree.DefaultView.ToTable();

            if (newDtTree.Rows.Count != 0)
            {
                foreach (DataRow row in newDtTree.Rows)
                {
                    myidx++;
                    string no = row["No"].ToString();
                    string name = row["Name"].ToString();
                    string isDtl = row["IsDtl"].ToString();
                    string refParentNo = row["RefParentNo"].ToString();
                    int attrCount = 0;
                    this.Pub1.AddTR();
                    foreach (MapAttr attr in attrs)
                    {
                        if (attr.UIVisible == false)
                            continue;

                        if (attr.IsPK)
                            continue;

                        //如果启用了分组，并且是当前的分组字段。
                        if (mdtl.IsEnableGroupField && mdtl.GroupField == attr.KeyOfEn)
                            continue;

                        if (attrCount == 0)
                        {
                            this.Pub1.AddTDIdx(myidx);
                            this.Pub1.AddTD(BP.DA.DataType.GenerSpace(idxRoot) + "|-" + name);
                        }
                        else
                        {

                            if (attr.UIContralType == UIContralType.TB)
                            {

                                switch (attr.MyDataType)
                                {

                                    case DataType.AppDouble:
                                    case DataType.AppFloat:
                                    case DataType.AppInt:
                                    case DataType.AppMoney:
                                        string count = "0";
                                        GetChildCout(attr.KeyOfEn, no,ref count, dtTree, attr.MyDataType);
                                        this.Pub1.AddTD("<input class='TBReadonly' id='" + no + "_" + refParentNo + "_"+attr.KeyOfEn+"' value='" + count + "' name='" + no +
                                                      "_" + refParentNo + "'  readonly='readonly' />");
                                        break;
                                    default:
                                        this.Pub1.AddTD("");
                                        break;
                                }
                            }
                            else
                                this.Pub1.AddTD("");
                        }
                        attrCount++;
                    }
                    this.Pub1.AddTREnd();
                    //如果是明细表借点开始增加明细表数据
                    if (isDtl == "1")
                    {

                        DDL ddl = new DDL();
                        CheckBox cb = new CheckBox();
                        GEDtls copyDtls = new GEDtls(this.EnsName);

                        QueryObject doQo = new QueryObject(copyDtls);
                        doQo.AddWhere("RefPk", this.RefPKVal);
                        doQo.addAnd();
                        doQo.AddWhere("RefTreeNo", no);
                        doQo.DoQuery();
                        foreach (GEDtl dtl in copyDtls)
                        {
                            myidx++;
                            this.Pub1.AddTR();

                            int dtlAttrCount = 0;
                            foreach (MapAttr attr in attrs)
                            {
                                if (attr.UIVisible == false)
                                    continue;

                                if (attr.IsPK)
                                    continue;

                                //如果启用了分组，并且是当前的分组字段。
                                if (mdtl.IsEnableGroupField && mdtl.GroupField == attr.KeyOfEn)
                                    continue;

                                if (dtlAttrCount == 0)
                                {
                                    this.Pub1.AddTDIdx(myidx);
                                    this.Pub1.AddTD( BP.DA.DataType.GenerSpace(idxRoot+2) +"|-"+
                                                    dtl.GetValStringByKey(attr.KeyOfEn));
                                }
                                else
                                {
                                    #region  增加行
                                    string val = dtl.GetValByKey(attr.KeyOfEn).ToString();
                                    if (attr.UIIsEnable == false && dtl.OID >= 100 && LinkFields.Contains("," + attr.KeyOfEn + ","))
                                    {
                                        if (string.IsNullOrEmpty(val))
                                            val = "...";

                                        MapExts mes = new MapExts(this.EnsName);

                                        MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link,
                                            MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;

                                        string url = meLink.Tag.Clone() as string;
                                        if (url.Contains("?") == false)
                                            url = url + "?a3=2";

                                        url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + mdtl.No + "&OID=" + dtl.OID;
                                        if (url.Contains("@AppPath"))
                                            url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + this.Request.ApplicationPath);
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
                                            tb.Enabled = attr.UIIsEnable;
                                            if (attr.UIIsEnable == false)
                                            {
                                                tb.Attributes.Add("readonly", "true");
                                                tb.CssClass = "TBReadonly";
                                            }
                                            else
                                            {
                                                tb.Attributes["onfocus"] = "isChange=true;";
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
                                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                                    tb.Text = val;
                                                    if (attr.UIIsEnable)
                                                    {
                                                        tb.Attributes["onfocus"] = "WdatePicker();isChange=true;";
                                                    }
                                                    else
                                                    {
                                                        tb.ReadOnly = true;
                                                    }
                                                    this.Pub1.AddTD("width='2px'", tb);
                                                    break;
                                                case DataType.AppDateTime:
                                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                                    tb.Text = val;
                                                    if (attr.UIIsEnable)
                                                    {
                                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});isChange=true;";
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
                                                        tb.Text = "0";
                                                    }
                                                    tb.Attributes["onchange"] = "SumTreeData('" + no + "','" + attr.KeyOfEn + "');";
                                                    hiddenRefData.Text += no + "@"+attr.KeyOfEn+"@"+ tb.ClientID + ";";
                                                    this.Pub1.AddTD(tb);
                                                    break;
                                                case DataType.AppDouble:
                                                case DataType.AppFloat:
                                                case DataType.AppMoney:
                                                case DataType.AppRate:
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
                                                        tb.Text = "0.00";
                                                    }
                                                    tb.Attributes["onchange"] = "SumTreeData('" + no + "','" + attr.KeyOfEn + "');";
                                                     hiddenRefData.Text += no + "@" + attr.KeyOfEn + "@" + tb.ClientID + ";";
                                                    this.Pub1.AddTD(tb);
                                                    break;
                                                default:
                                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border-width:0px;";
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
                                                        tb.Attributes["onkeyup"] += @"C" + dtl.OID + "();C" + attr.KeyOfEn + "(); ";
                                                        tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'int');";
                                                        tb.Attributes["onblur"] += @"value=value.replace(/[^-?\d]/g,'');C" + dtl.OID + "();C" + attr.KeyOfEn + "();";
                                                    }
                                                    else
                                                    {
                                                        tb.Attributes["onkeyup"] += @"C" + dtl.OID + "();C" + attr.KeyOfEn + "();  ";
                                                        tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'float');";

                                                        tb.Attributes["onblur"] += @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');C" + dtl.OID + "();C" + attr.KeyOfEn + "();";
                                                    }
                                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border-width:0px;";
                                                }
                                                else
                                                {
                                                    tb.Attributes["onpropertychange"] += "C" + attr.KeyOfEn + "();";
                                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border-width:0px;";
                                                }
                                            }
                                            break;
                                        case UIContralType.DDL:
                                            switch (attr.LGType)
                                            {
                                                case FieldTypeS.Enum:
                                                    DDL myddl = new DDL();
                                                    myddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                                    myddl.Attributes["onchange"] = "isChange= true;";
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
                                                        myddl.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.KeyOfEn), dtl.GetValStrByKey(attr.KeyOfEn)));
                                                    }
                                                    myddl.Enabled = attr.UIIsEnable;
                                                    this.Pub1.AddTDCenter(myddl);
                                                    break;
                                                case FieldTypeS.FK:
                                                    DDL ddl1 = new DDL();
                                                    ddl1.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                                    ddl1.Attributes["onchange"] = "isChange=true;";
                                                    ddl1.Attributes["onfocus"] = "isChange=true;";
                                                    if (attr.UIIsEnable)
                                                    {
                                                        //   ddl1.Attributes["onchange"] = "isChange=true;";
                                                        EntitiesNoName ens = attr.HisEntitiesNoName;
                                                        ens.RetrieveAll();
                                                        ddl1.BindEntities(ens);
                                                        if (ddl1.SetSelectItem(val) == false)
                                                            ddl1.Items.Insert(0, new ListItem("请选择", val));
                                                    }
                                                    else
                                                    {
                                                        ddl1.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.KeyOfEn), dtl.GetValStrByKey(attr.KeyOfEn)));
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
                                            this.Pub1.AddTD(cb);
                                            break;
                                        default:
                                            break;
                                    }

                                    if (mdtl.IsDelete && this.IsReadonly == 0 && dtl.OID >= 100)
                                    {
                                        if (dtl.IsRowLock == true)
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
                                    #endregion  增加行
                                }
                                dtlAttrCount++;
                            }
                            this.Pub1.AddTREnd();
                        }
                    }
                    else
                        FindNextNode(dtTree, no, ref myidx, attrs, mdtl, mainEn, LinkFields, attrs2,idxRoot+2);

                }

            }


        }
        /// <summary>
        /// 对比entity与配置的数据源数据
        /// </summary>
        /// <param name="dtls"></param>
        /// <param name="dataTable"></param>
        private void ContrastEntity(GEDtls dtls, DataTable dataTable, MapAttrs attrs)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string mainPk = row["RefMainPk"].ToString();

                QueryObject doQo = new QueryObject(dtls);
                doQo.AddWhere("RefPK", this.RefPKVal);
                doQo.addAnd();
                doQo.AddWhere("RefMainPk", mainPk);
                int count = doQo.DoQuery();
                if (count > 0)
                    continue;

                GEDtl newDtl = new GEDtl(this.EnsName);
                foreach (MapAttr attr in attrs)
                {

                    if (attr.KeyOfEn.Equals("RefPk"))
                        newDtl.SetValByKey(attr.KeyOfEn, this.RefPKVal);
                    else if (attr.KeyOfEn.Equals("FID"))
                        newDtl.SetValByKey(attr.KeyOfEn, this.FID);
                    else if (attr.KeyOfEn.Equals("RefPK"))
                        newDtl.SetValByKey(attr.KeyOfEn, this.RefPKVal);
                    else if (dataTable.Columns.Contains(attr.KeyOfEn))
                        newDtl.SetValByKey(attr.KeyOfEn, row[attr.KeyOfEn]);
                }
                newDtl.Insert();
            }


        }

        private void GetChildCout(string keyOfEn, string parentNo,ref string count, DataTable refTreeData, int dataType)
        {
            //查出当前节点的子集
            DataTable newRefTreeData = refTreeData.Copy();
            newRefTreeData.DefaultView.RowFilter = " RefParentNo=" + parentNo;
            newRefTreeData = newRefTreeData.DefaultView.ToTable();

            if (newRefTreeData.Rows.Count == 0)
            {
                GEDtls dtls = new GEDtls(this.EnsName);

                QueryObject doQo = new QueryObject(dtls);
                doQo.AddWhere("RefPk", this.RefPKVal);
                doQo.addAnd();
                doQo.AddWhere("RefTreeNo", parentNo);
                doQo.DoQuery();

                foreach (GEDtl dtl in dtls)
                {
                    switch (dataType)
                    {
                        case DataType.AppInt:
                            int appIntCount = int.Parse(count);
                            appIntCount = appIntCount + dtl.GetValIntByKey(keyOfEn);
                            count = appIntCount.ToString();
                            break;

                        case DataType.AppFloat:
                            float appFloatCount = float.Parse(count);
                            appFloatCount = appFloatCount + dtl.GetValFloatByKey(keyOfEn);
                            count = appFloatCount.ToString();
                            break;

                        case DataType.AppDouble:
                            double appDoubleCount = float.Parse(count);
                            appDoubleCount = appDoubleCount + dtl.GetValDoubleByKey(keyOfEn);
                            count = appDoubleCount.ToString();
                            break;

                        case DataType.AppMoney:
                            decimal appDecimalCount = decimal.Parse(count);
                            appDecimalCount = appDecimalCount + dtl.GetValMoneyByKey(keyOfEn);
                            count = appDecimalCount.ToString();
                            break;
                    }
                }

            }

            else
            {
                foreach (DataRow row in newRefTreeData.Rows)
                {
                    GetChildCout(keyOfEn, row["No"].ToString(), ref count, refTreeData, dataType);
                }

            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            MapDtl mdtl = new MapDtl(this.EnsName);
            GEDtls dtls = new GEDtls(this.EnsName);
            FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.
            GEEntity mainEn = mdtl.GenerGEMainEntity(this.RefPKVal);


            QueryObject qo = new QueryObject(dtls);
            switch (mdtl.DtlOpenType)
            {
                case DtlOpenType.ForEmp:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    qo.addAnd();
                    qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                    break;
                case DtlOpenType.ForWorkID:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    break;
                case DtlOpenType.ForFID:
                    qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                    break;
            }

            qo.DoQuery();


            #region 从表保存前处理事件.
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
            #endregion 从表保存前处理事件.

            // 判断是否有事件.
            bool isHaveBefore = false;
            bool isHaveEnd = false;
            FrmEvent fe_Before = fes.GetEntityByKey(FrmEventAttr.FK_Event, EventListDtlList.DtlItemSaveBefore) as FrmEvent;
            if (fe_Before == null)
                isHaveBefore = false;
            else
                isHaveBefore = true;

            FrmEvent fe_End = fes.GetEntityByKey(FrmEventAttr.FK_Event, EventListDtlList.DtlItemSaveAfter) as FrmEvent;
            if (fe_End == null)
                isHaveEnd = false;
            else
                isHaveEnd = true;
            Map map = dtls.GetNewEntity.EnMap;
            bool isTurnPage = false;
            string err = "";
            int idx = 0;
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


            this.Response.Redirect("DtlFixRow.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&Key=" + this.Key, true);



        }
    }
}