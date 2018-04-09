using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Web.Controls;
using BP.En;
using BP.WF;
using BP.Sys;
using BP.WF.Rpt; 
using BP;
using BP.WF.Template;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CCFlow.WF.Rpt
{
    public partial class Search : BP.Web.UC.UCBase3
    {
        #region 属性.
        /// <summary>
        /// 编号名称
        /// </summary>
        public string RptNo
        {
            get
            {
                string s = this.Request.QueryString["RptNo"];
                if (DataType.IsNullOrEmpty(s))
                    return this.EnsName;
                return s;
            }
        }
        public new string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (DataType.IsNullOrEmpty(s))
                    throw new Exception("没有得到FK_Flow参数");
                return s;
            }
        }
        public Entities _HisEns = null;
        public Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    if (this.RptNo != null)
                    {
                        if (this._HisEns == null)
                            _HisEns = BP.En.ClassFactory.GetEns(this.RptNo);
                    }
                }
                return _HisEns;
            }
        }
        public MapRpt currMapRpt = null;
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 处理查询权限， 此处不要修改，以Search.ascx为准.
            // this.Page.RegisterClientScriptBlock("sss",
            //"<link href='" + BP.WF.Glo.CCFlowAppPath + "WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");
            currMapRpt = new MapRpt(this.RptNo);
            Entity en = this.HisEns.GetNewEntity;

            string flowNo = this.currMapRpt.FK_Flow;
            if (DataType.IsNullOrEmpty(flowNo))
            {
                this.currMapRpt.FK_Flow = this.FK_Flow;
                this.currMapRpt.Update();
            }

            Flow fl = new Flow(this.currMapRpt.FK_Flow);
            
            //this.Page.Title = fl.Name;
            //杨玉慧  改成 流程名字+ 报表的名字  因为一个流程有多个报表
            this.Page.Title = fl.Name + "(" + this.currMapRpt.Name + ")";

            //初始化查询工具栏.
            this.ToolBar1.InitToolbarOfMapRpt(fl, currMapRpt, this.RptNo, en, 1);

            //增加发起.
            if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, BP.Web.WebUser.No) == true)
            {
                string str = "<div style='float:right'><a href=\"javascript:WinOpen('../MyFlow.htm?FK_Flow=" + this.FK_Flow + "','df');\" ><img src='/WF/Img/Start.png' width='12px' border=0/>&nbsp;发起</a></div>";
                this.ToolBar1.Add(str);
            }

            if (BP.Web.WebUser.No == "admin")
            {
                string url = "/WF/RptDfine/Default.htm?FK_MapData=ND" + int.Parse(this.FK_Flow) + "Rpt&FK_Flow=" + this.FK_Flow + "&RptNo=" + this.currMapRpt.No;

                //  string str = "<div style='float:right'><a href=\"javascript:Setting('"+this.RptNo+"','"+this.FK_Flow+"');\" >设置</a></div>";
                string str = "<div style='float:right'><a href='" + url + "' ><img src='/WF/Img/Setting.png' width='12px' border=0/>&nbsp;设置</a></div>";
                this.ToolBar1.Add(str);
            }


            this.ToolBar1.AddLinkBtn(BP.Web.Controls.NamesOfBtn.Export); //导出.
            this.ToolBar1.AddLinkBtn(BP.Web.Controls.NamesOfBtn.ExportByTemplate);  //导出数据到模板文件

            //增加转到.
            this.ToolBar1.Add("&nbsp;");
            DDL ddl = new DDL();
            ddl.ID = "GoTo";
            ddl.Items.Add(new ListItem("查询", "Search"));
            // ddl.Items.Add(new ListItem("高级查询", "SearchAdv"));
            ddl.Items.Add(new ListItem("分组分析", "Group"));
            ddl.Items.Add(new ListItem("交叉报表", "D3"));
            ddl.Items.Add(new ListItem("对比分析", "Contrast"));
            ddl.SetSelectItem(this.PageID);
            this.ToolBar1.AddDDL(ddl);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged_GoTo);

            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Search).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Export).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.ExportByTemplate).Click += new System.EventHandler(this.ToolBar1_ButtonClick);

            #endregion 处理查询权限

            //处理按钮.
            this.SetDGData();
        }

        void ddl_SelectedIndexChanged_GoTo(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            string item = ddl.SelectedItemStringVal;

            string tKey = DateTime.Now.ToString("MMddhhmmss");
            this.Response.Redirect(item + ".aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow + "&T=" + tKey, true);
        }

        public Entities SetDGData()
        {
            try
            {
                return this.SetDGData(this.PageIdx);
            }
            catch
            {
                Flow fl = new Flow(this.FK_Flow);
                fl.DoCheck();

                MapRpt myRpt = new MapRpt("ND" + int.Parse(this.FK_Flow) + "MyRpt");
                myRpt.ResetIt();
                //清缓存
                BP.DA.Cash.Map_Cash.Clear();

                return this.SetDGData(this.PageIdx);
            }
        }

        public Entities SetDGData(int pageIdx)
        {
            #region 执行数据分页查询，并绑定分页控件.

            BP.DA.Cash.Map_Cash.Clear();    //added by liuxc,2016-12-21,必须增加清除缓存，否则会因为缓存与数据库中数据对应不上而报错
            Entities ens = this.HisEns;
            Entity en = ens.GetNewEntity;
            QueryObject qo = new QueryObject(ens);
            qo = this.ToolBar1.GetnQueryObject(ens, en);

            //if (qo.MyParas.COntinckey("WFSta") == false)
            //{
            //    qo.addAnd();
            //    try
            //    {
            //        qo.AddWhere("WFSta", "!=", "0");
            //    }
            //    catch
            //    {
            //        BP.WF.Flow fl = new Flow(this.FK_Flow);
            //        fl.CheckRpt();
            //        qo.AddWhere("WFSta", "!=", "0");
            //    }
            //}

            this.Pub2.Clear();
            this.Pub2.BindPageIdxEasyUi(qo.GetCount(),
                                        this.PageID + ".aspx?RptNo=" + this.RptNo + "&EnsName=" + this.RptNo +
                                        "&FK_Flow=" + this.FK_Flow,
                                        pageIdx,
                                        SystemConfig.PageSize);


            qo.DoQuery(en.PK, SystemConfig.PageSize, pageIdx);
            #endregion 执行数据分页查询，并绑定分页控件.

            #region 检查是否显示按关键字查询，如果是就把关键标注为红色.

            if (en.EnMap.IsShowSearchKey)
            {
                string keyVal = this.ToolBar1.GetTBByID("TB_Key").Text.Trim();

                if (keyVal.Length >= 1)
                {
                    Attrs attrs = en.EnMap.Attrs;

                    foreach (Entity myen in ens)
                    {
                        foreach (Attr attr in attrs)
                        {
                            if (attr.IsFKorEnum)
                                continue;

                            if (attr.IsPK)
                                continue;

                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                case DataType.AppInt:
                                case DataType.AppFloat:
                                case DataType.AppDouble:
                                case DataType.AppBoolean:
                                    continue;
                                default:
                                    break;
                            }

                            myen.SetValByKey(attr.Key, myen.GetValStrByKey(attr.Key).Replace(keyVal, "<font color=red>" + keyVal + "</font>"));
                        }
                    }
                }
            }
            #endregion 检查是否显示按关键字查询，如果是就把关键标注为红色.

            // 处理entity的GuestNo 列的问题。
            //  if (en.EnMap.Attrs.Contains(NDXRptBaseAttr.ex
            //foreach (Entity en in ens)
            //{
            //}
            //绑定数据.
            this.BindEns(ens, null);

            #region 生成翻页的js，暂不用
            //int ToPageIdx = this.PageIdx + 1;
            //int PPageIdx = this.PageIdx - 1;
            //this.UCSys1.Add("<SCRIPT language=javascript>");
            //this.UCSys1.Add("\t\n document.onkeydown = chang_page;");
            //this.UCSys1.Add("\t\n function chang_page() { ");
            //if (this.PageIdx == 1)
            //{
            //    this.UCSys1.Add("\t\n if (event.keyCode == 37 || event.keyCode == 33) alert('已经是第一页');");
            //}
            //else
            //{
            //    this.UCSys1.Add("\t\n if (event.keyCode == 37  || event.keyCode == 38 || event.keyCode == 33) ");
            //    this.UCSys1.Add("\t\n     location='" + this.PageID + ".aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.currMapRpt.FK_Flow + "&PageIdx=" + PPageIdx + "';");
            //}

            //if (this.PageIdx == maxPageNum)
            //{
            //    this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) alert('已经是最后一页');");
            //}
            //else
            //{
            //    this.UCSys1.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) ");
            //    this.UCSys1.Add("\t\n     location='" + this.PageID + ".aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.currMapRpt.FK_Flow + "&PageIdx=" + ToPageIdx + "';");
            //}

            //this.UCSys1.Add("\t\n } ");
            //this.UCSys1.Add("</SCRIPT>");
            #endregion 生成翻页的js

            return ens;
        }
        private string GenerEnUrl(Entity en, Attrs attrs)
        {
            string url = "";
            foreach (Attr attr in attrs)
            {
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        if (attr.IsPK)
                            url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                        break;
                    case UIContralType.DDL:
                        url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                        break;
                }
            }
            return url;
        }

        /// <summary>
        /// 排序Attrs集合，按照MapAttr.Idx排序
        /// added by liuxc,2016-12-19
        /// </summary>
        /// <param name="attrs">Attrs集合</param>
        /// <returns></returns>
        private Attrs SortAttrs(Attrs attrs)
        {
            MapAttrs mattrs = new MapAttrs();
            mattrs.Retrieve(MapAttrAttr.FK_MapData, this.RptNo, MapAttrAttr.Idx);
            Attrs nattrs = new Attrs();
            Attr attr = null;

            foreach(MapAttr mattr in mattrs)
            {
                attr = attrs.GetAttrByKey(mattr.KeyOfEn);
                nattrs.Add(attr);
            }

            return nattrs;
        }

        /// <summary>
        /// 绑定实体集合.
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="ctrlId"></param>
        public void BindEns(Entities ens, string ctrlId)
        {
            #region 定义变量.
            MapData md = new MapData(this.RptNo);
            if (this.Page.Title == "")
                this.Page.Title = md.Name;

            this.UCSys1.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            Attrs attrs = SortAttrs(myen.EnMap.Attrs);  //edited by liuxc,2016-12-19,修改字段排序
            #endregion 定义变量.

            this.UCSys1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%;line-height:22px'");

            #region  生成表格标题
            this.UCSys1.AddTR();
            this.UCSys1.AddTDGroupTitle("style='text-align:center;width:40px;'", "序");
            //this.UCSys1.AddTDGroupTitle("标题");
            
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr
                    //|| attr.Key == "Title"
                    || attr.Key == "MyNum")
                    continue;

                this.UCSys1.AddTDGroupTitle(attr.Desc);
            }

            this.UCSys1.AddTREnd();
            #endregion  生成表格标题

            #region 用户界面属性设置

            int pageidx = this.PageIdx - 1;
            int idx = SystemConfig.PageSize * pageidx;
            #endregion 用户界面属性设置

            #region 数据输出.

            foreach (Entity en in ens)
            {
                #region 输出字段。

                idx++;
                this.UCSys1.AddTR();
                this.UCSys1.AddTDIdx(idx);
                //this.UCSys1.AddTD("<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.aspx?FK_Flow=" + this.currMapRpt.FK_Flow + "&WorkID=" + en.GetValStrByKey("OID") + "','tdr');\" >" + en.GetValByKey("Title") + "</a>");

                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr || attr.Key == "MyNum")  // || attr.Key == "Title"
                        continue;

                    if(attr.Key == "Title")
                    {
                        this.UCSys1.AddTD("<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.htm?FK_Flow=" + this.currMapRpt.FK_Flow + "&WorkID=" + en.GetValStrByKey("OID") + "','tdr');\" >" + en.GetValByKey("Title") + "</a>");
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.DDL || attr.UIContralType == UIContralType.RadioBtn)
                    {
                        string s = en.GetValRefTextByKey(attr.Key);
                        if (DataType.IsNullOrEmpty(s))
                        {
                            switch (attr.Key)
                            {
                                case "FK_NY": // 2012-01
                                    s = en.GetValStringByKey(attr.Key);
                                    break;
                                default: //其他的情况，把编码输出出来.
                                    s = en.GetValStringByKey(attr.Key);
                                    break;
                            }
                        }
                        this.UCSys1.AddTD(s);
                        continue;
                    }

                    string str = en.GetValStrByKey(attr.Key);

                    switch (attr.MyDataType)
                    {
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            if (str == "" || str == null)
                                str = "&nbsp;";
                            this.UCSys1.AddTD(str);
                            break;
                        case DataType.AppString:
                            if (str == "" || str == null)
                                str = "&nbsp;";

                            if (attr.UIHeight != 0)
                                this.UCSys1.AddTDDoc(str, str);
                            else
                                this.UCSys1.AddTD(str);
                            break;
                        case DataType.AppBoolean:
                            if (str == "1")
                                this.UCSys1.AddTD("是");
                            else
                                this.UCSys1.AddTD("否");
                            break;
                        case DataType.AppFloat:
                        case DataType.AppInt:
                        case DataType.AppDouble:
                            this.UCSys1.AddTDNum(str);
                            break;
                        case DataType.AppMoney:
                            this.UCSys1.AddTDNum(decimal.Parse(str).ToString("0.00"));
                            break;
                        default:
                            throw new Exception("no this case ...");
                    }
                }

                this.UCSys1.AddTREnd();
                #endregion 输出字段。
            }
            #endregion 数据输出.

            #region 计算一下是否可以求出合计,主要是判断是否有数据类型在这个Entities中。

            bool IsHJ = false;
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr
                    || attr.Key == "Title"
                    || attr.Key == "MyNum")
                    continue;

                if (attr.UIVisible == false)
                    continue;

                if (attr.UIContralType == UIContralType.DDL)
                    continue;

                if (attr.Key == "OID" ||
                    attr.Key == "MID"
                    || attr.Key == "FID"
                    || attr.Key == "PWorkID"
                    || attr.Key.ToUpper() == "WORKID")
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppDouble:
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppMoney:
                        IsHJ = true;
                        break;
                    default:
                        break;
                }

                if (IsHJ)
                    break;
            }
            #endregion 计算一下是否可以求出合计,主要是判断是否有数据类型在这个Entities中。

            #region  输出合计。
            //edited by liuxc,2015.5.14,解决合计行错列问题
            if (IsHJ)
            {
                this.UCSys1.Add("<TR class='Sum' >");
                //this.UCSys1.AddTD();
                this.UCSys1.AddTD("合计");
                foreach (Attr attr in attrs)
                {
                    //if (attr.Key == "MyNum")
                    //    continue;
                    if (attr.MyFieldType == FieldType.RefText
                    //|| attr.Key == "Title"
                    || attr.Key == "MyNum")
                        continue;

                    if (attr.UIContralType == UIContralType.DDL || attr.UIContralType == UIContralType.RadioBtn)
                    {
                        this.UCSys1.AddTD();
                        continue;
                    }

                    //if (attr.UIVisible == false)
                    //    continue;

                    if (attr.Key == "OID" || attr.Key == "MID"
                        || attr.Key.ToUpper() == "WORKID"
                        || attr.Key == "FID")
                    {
                        this.UCSys1.AddTD();
                        continue;
                    }

                    switch (attr.MyDataType)
                    {
                        case DataType.AppDouble:
                            this.UCSys1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                            this.UCSys1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            this.UCSys1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.UCSys1.AddTDJE(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        default:
                            this.UCSys1.AddTD();
                            break;
                    }
                }
                /*结束循环*/
                //this.UCSys1.AddTD();
                this.UCSys1.AddTREnd();
            }

            #endregion

            this.UCSys1.AddTableEnd();
        }

        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            var btn = (LinkBtn)sender;

            try
            {

                switch (btn.ID)
                {
                    case NamesOfBtn.Export: //数据导出.
                    case NamesOfBtn.Excel: //数据导出
                        #region 数据导出
                        MapData md = new MapData(this.RptNo);
                        Entities ens = md.HisEns;
                        Entity en = ens.GetNewEntity;
                        QueryObject qo = new QueryObject(ens);
                        qo = this.ToolBar1.GetnQueryObject(ens, en);

                        DataTable dt = qo.DoQueryToTable();
                        DataTable myDT = new DataTable();
                        MapAttrs attrs = new MapAttrs(this.RptNo);

                        foreach (MapAttr attr in attrs)
                        {
                            if (attr.KeyOfEn == "MyNum")
                                continue;

                            Type t = null;

                            switch(attr.LGType)
                            {
                                case FieldTypeS.Normal:
                                    switch (attr.MyDataType)
                                    {
                                        case BP.DA.DataType.AppInt:
                                            t = typeof (int);
                                            break;
                                        case BP.DA.DataType.AppFloat:
                                        case BP.DA.DataType.AppDouble:
                                        case BP.DA.DataType.AppMoney:
                                            t = typeof (double);
                                            break;
                                        default:
                                            t = typeof (string);
                                            break;
                                    }
                                    break;
                                default:
                                    t = typeof(string);
                                    break;
                            }

                            myDT.Columns.Add(new DataColumn(attr.Name, t));
                            myDT.Columns[attr.Name].ExtendedProperties.Add("width", attr.UIWidthInt);

                            if (attr.IsNum && attr.LGType == FieldTypeS.Normal && "OID,FID,PWorkID,FlowEndNode,PNodeID".IndexOf(attr.KeyOfEn) == -1)
                                myDT.Columns[attr.Name].ExtendedProperties.Add("sum", attr.IsSum);
                        }

                        foreach (DataRow dr in dt.Rows)
                        {
                            DataRow myDR = myDT.NewRow();

                            foreach (MapAttr attr in attrs)
                            {
                                if (attr.KeyOfEn == "MyNum")
                                    continue;

                                switch (attr.LGType)
                                {
                                    case FieldTypeS.Normal:
                                        switch (attr.MyDataType)
                                        {
                                            case BP.DA.DataType.AppString:
                                            case BP.DA.DataType.AppDate:
                                            case BP.DA.DataType.AppDateTime:
                                            case BP.DA.DataType.AppInt:
                                            case BP.DA.DataType.AppFloat:
                                            case BP.DA.DataType.AppDouble:
                                            case BP.DA.DataType.AppMoney:
                                                myDR[attr.Name] = dr[attr.Field];
                                                break;
                                            case BP.DA.DataType.AppBoolean:
                                                if (dr[attr.Field].ToString() == "0")
                                                    myDR[attr.Name] = "否";
                                                else
                                                    myDR[attr.Name] = "是";
                                                break;
                                        }
                                        break;
                                    case FieldTypeS.Enum:
                                        SysEnum sem = new SysEnum();
                                        sem.Retrieve(SysEnumAttr.EnumKey, attr.KeyOfEn, SysEnumAttr.IntKey, dr[attr.Field]);
                                        myDR[attr.Name] = sem.Lab;
                                        break;
                                    case FieldTypeS.FK:
                                        string tabName = attr.UIBindKey;
                                        if (attr.KeyOfEn == "FK_NY") {
                                            tabName = "Pub_NY";
                                        }
                                        else if (attr.KeyOfEn == "FK_Dept")
                                        {
                                            tabName = "Port_Dept";
                                        }
                                        DataTable drDt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM " + tabName + " WHERE NO='" + dr[attr.Field] + "'");
                                        if (drDt.Rows.Count > 0)
                                            myDR[attr.Name] = drDt.Rows[0]["NAME"].ToString();
                                        break;
                                    case FieldTypeS.WinOpen:
                                        break;
                                }
                            }

                            myDT.Rows.Add(myDR);
                        }

                        try
                        {
                            string filename = this.Request.PhysicalApplicationPath + @"\Temp\" + en.EnDesc + "_" +
                                              DateTime.Today.ToString("yyyy年MM月dd日") + ".xls";
                            CCFlow.WF.Comm.Utilities.NpoiFuncs.DataTableToExcel(myDT, filename, en.EnDesc,
                                                                                BP.Web.WebUser.Name, true, true, true);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("数据没有正确导出可能的原因之一是:系统管理员没正确的安装Excel组件，请通知他，参考安装说明书解决。@系统异常信息：" + ex.Message);
                        }

                        this.SetDGData();
                        #endregion
                        return;
                    case NamesOfBtn.ExportByTemplate:   //导出数据到模板
                        #region 数据导出至Excel模板
                        MapData mdMyRpt = new MapData(this.RptNo);
                        string fk_mapdata = "ND" + int.Parse(FK_Flow) + "Rpt";
                        string tmpFile = null;
                        string tmpDir = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + this.RptNo + @"\";
                        string tmpXml = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + this.RptNo + @"\" + mdMyRpt.Name + ".xml";
                        DirectoryInfo infoTmpDir = new DirectoryInfo(tmpDir);
                        FileInfo[] tmpFiles = null;
                        RptExportTemplate tmp = null;
                        MapData mdRpt = new MapData(fk_mapdata);

                        if (!infoTmpDir.Exists)
                            infoTmpDir.Create();

                        tmpFiles = infoTmpDir.GetFiles(mdMyRpt.Name + ".xls*");

                        if (tmpFiles.Length > 0)
                            tmpFile = tmpFiles[0].FullName;

                        if (!string.IsNullOrWhiteSpace(tmpFile))
                        {
                            tmp = RptExportTemplate.FromXml(tmpXml);
                        }
                        else
                        {
                            ToolBar1_ButtonClick(ToolBar1.GetLinkBtnByID(NamesOfBtn.Export), new EventArgs());
                            return;
                        }

                        ens = mdMyRpt.HisEns;
                        en = ens.GetNewEntity;
                        ens = mdRpt.HisEns; //added by liuxc,2016-12-19,变更为Rpt集合类，这样查询的时候，就可以用MyRpt的查询条件，查询出Rpt实体集合
                        qo = this.ToolBar1.GetnQueryObject(ens, en);
                        qo.DoQuery();

                        //获取流程绑定的表单库中的表单信息
                        List<string> listFrms = new List<string>(); //存储绑定表单mapdata编号
                        FrmNodes frms = new FrmNodes();
                        frms.Retrieve(FrmNodeAttr.FK_Flow, FK_Flow );

                        foreach (FrmNode fn in frms)
                        {
                            if (listFrms.Contains(fn.FK_Frm))
                                continue;

                            listFrms.Add(fn.FK_Frm);
                        }

                        GEEntitys ges = null;
                        GEEntitys dtlGes = null;
                        QueryObject qo2 = null;
                        string dtlNo = tmp.GetDtl();
                        Dictionary<string, Entities> frmDatas = new Dictionary<string, Entities>(); //存储fk_mapdata,Entities
                        Dictionary<string, MapAttrs> frmAttrs = new Dictionary<string, MapAttrs>(); //存储fk_mapdata,MapAttrs
                        string oids = GetOidsJoin(ens, "OID", false);

                        //获取各绑定表单的记录集合
                        frmDatas.Add(fk_mapdata, ens);
                        frmAttrs.Add(fk_mapdata, new MapAttrs(fk_mapdata));

                        //增加明细表的字段定义
                        if (!string.IsNullOrWhiteSpace(dtlNo))
                        {
                            frmAttrs.Add(dtlNo, new MapAttrs(dtlNo));
                        }

                        foreach (string frm in listFrms)
                        {
                            //如果模板中没有涉及该表单的字段绑定信息，则不加载此表单的数据
                            if (!tmp.HaveCellInMapData(frm))
                                continue;

                            ges = new GEEntitys(frm);
                            qo2 = new QueryObject(ges);

                            if (ens.Count > 0)
                                qo2.AddWhereIn("OID", oids);

                            qo2.DoQuery();
                            frmDatas.Add(frm, ges);
                            frmAttrs.Add(frm, new MapAttrs(frm));
                        }

                        oids = GetOidsJoin(ens, "OID", true);

                        //获取定义明细表的记录集合
                        if (!string.IsNullOrWhiteSpace(dtlNo))
                        {
                            dtlGes = new GEEntitys(dtlNo);
                            qo2 = new QueryObject(dtlGes);

                            if (ens.Count > 0)
                                qo2.AddWhereIn("RefPK", oids);

                            qo2.DoQuery();
                        }

                        IWorkbook wb = null;
                        ISheet sheet = null;
                        IRow row = null;
                        ICell cell = null;
                        int r = 0;
                        int c = 0;
                        int lastRowIdx = 0;
                        MapAttr mattr = null;
                        MapAttr dmattr = null;
                        IDataFormat fmt = null;
                        int dtlRecordCount = dtlGes != null ? dtlGes.Count : 0;
                        string workid = string.Empty;
                        Entity newEn = null;
                        Entities tens = null;
                        DataTable dtData = new DataTable();
                        DataRow dr1 = null;

                        try
                        {
                            using (FileStream fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read))
                            {
                                if (Path.GetExtension(tmpFile).ToLower() == ".xls")
                                    wb = new HSSFWorkbook(fs);
                                else
                                    wb = new XSSFWorkbook(fs);

                                sheet = wb.GetSheetAt(0);
                                fmt = wb.CreateDataFormat();
                                lastRowIdx = sheet.LastRowNum;

                                //垂直方向填充数据时，先将缺少的行数增加上
                                for (int i = sheet.LastRowNum; i < tmp.BeginIdx + ens.Count + dtlRecordCount - 1; i++)
                                {
                                    sheet.GetRow(lastRowIdx).CopyRowTo(i + 1);
                                }

                                //生成列
                                foreach (RptExportTemplateCell tcell in tmp.Cells)
                                {
                                    if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                        mattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                                    else
                                        mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;

                                    switch (mattr.MyDataType)
                                    {
                                        case DataType.AppString:
                                            dtData.Columns.Add(mattr.MyPK, typeof(string));
                                            break;
                                        case DataType.AppInt:
                                            if (mattr.LGType == FieldTypeS.Normal)
                                                dtData.Columns.Add(mattr.MyPK, typeof(int));
                                            else
                                                dtData.Columns.Add(mattr.MyPK, typeof(string));
                                            break;
                                        case DataType.AppFloat:
                                        case DataType.AppMoney:
                                            if (mattr.LGType == FieldTypeS.Normal)
                                                dtData.Columns.Add(mattr.MyPK, typeof(double));
                                            else
                                                dtData.Columns.Add(mattr.MyPK, typeof(string));
                                            break;
                                        case DataType.AppDate:
                                        case DataType.AppDateTime:
                                            dtData.Columns.Add(mattr.MyPK, typeof(string));
                                            break;
                                        case DataType.AppBoolean:
                                            dtData.Columns.Add(mattr.MyPK, typeof(bool));
                                            break;
                                        default:
                                            throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                                    }
                                }

                                for (int i = 0; i < ens.Count; i++)
                                {
                                    //添加主表数据
                                    dr1 = dtData.NewRow();

                                    foreach (RptExportTemplateCell tcell in tmp.Cells)
                                    {
                                        if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                            continue;

                                        mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;
                                        tens = frmDatas[tcell.FK_MapData];

                                        if (tcell.FK_MapData != fk_mapdata)
                                            newEn = tens.GetEntityByKey(ens[i].PKVal) ?? tens.GetNewEntity;
                                        else
                                            newEn = ens[i];

                                        switch (mattr.MyDataType)
                                        {
                                            case DataType.AppString:
                                                if (mattr.LGType == FieldTypeS.Normal)
                                                    dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.KeyOfEn);
                                                else
                                                    dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.KeyOfEn);
                                                break;
                                            case DataType.AppInt:
                                                if (mattr.LGType == FieldTypeS.Normal)
                                                    dr1[mattr.MyPK] = newEn.GetValIntByKey(tcell.KeyOfEn);
                                                else
                                                    dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.KeyOfEn);
                                                break;
                                            case DataType.AppFloat:
                                            case DataType.AppMoney:
                                                if (mattr.LGType == FieldTypeS.Normal)
                                                    dr1[mattr.MyPK] = newEn.GetValDoubleByKey(tcell.KeyOfEn);
                                                else
                                                    dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.KeyOfEn);
                                                break;
                                            case DataType.AppDate:
                                            case DataType.AppDateTime:
                                                dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.KeyOfEn, "");
                                                break;
                                            case DataType.AppBoolean:
                                                dr1[mattr.MyPK] = newEn.GetValBooleanByKey(tcell.KeyOfEn);
                                                break;
                                            default:
                                                throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                                        }
                                    }

                                    dtData.Rows.Add(dr1);

                                    //添加明细表数据
                                    if (dtlGes == null)
                                        continue;

                                    workid = ens[i].GetValIntByKey("OID").ToString();

                                    foreach (GEEntity gen in dtlGes)
                                    {
                                        if (gen.GetValStringByKey("RefPK") != workid) continue;

                                        dr1 = dtData.NewRow();

                                        foreach (RptExportTemplateCell tcell in tmp.Cells)
                                        {
                                            if (string.IsNullOrWhiteSpace(tcell.DtlKeyOfEn))
                                                continue;

                                            if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                                mattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                                            else
                                                mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;

                                            newEn = gen;

                                            switch (mattr.MyDataType)
                                            {
                                                case DataType.AppString:
                                                    if (mattr.LGType == FieldTypeS.Normal)
                                                        dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.DtlKeyOfEn);
                                                    else
                                                        dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.DtlKeyOfEn);
                                                    break;
                                                case DataType.AppInt:
                                                    if (mattr.LGType == FieldTypeS.Normal)
                                                        dr1[mattr.MyPK] = newEn.GetValIntByKey(tcell.DtlKeyOfEn);
                                                    else
                                                    {
                                                        //此处需要区别明细表的该字段数据类型是否与主表一致
                                                        dmattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                                                        if (dmattr.MyDataType == mattr.MyDataType)
                                                            dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.DtlKeyOfEn);
                                                        else
                                                            dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.DtlKeyOfEn);
                                                    }
                                                    break;
                                                case DataType.AppFloat:
                                                case DataType.AppMoney:
                                                    if (mattr.LGType == FieldTypeS.Normal)
                                                        dr1[mattr.MyPK] = newEn.GetValDoubleByKey(tcell.DtlKeyOfEn);
                                                    else
                                                        dr1[mattr.MyPK] = newEn.GetValRefTextByKey(tcell.DtlKeyOfEn);
                                                    break;
                                                case DataType.AppDate:
                                                case DataType.AppDateTime:
                                                    dr1[mattr.MyPK] = newEn.GetValStringByKey(tcell.DtlKeyOfEn);
                                                    break;
                                                case DataType.AppBoolean:
                                                    dr1[mattr.MyPK] = newEn.GetValBooleanByKey(tcell.DtlKeyOfEn);
                                                    break;
                                                default:
                                                    throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                                            }
                                        }

                                        dtData.Rows.Add(dr1);
                                    }
                                }

                                //写入excel单元格值
                                for (int i = 0; i < dtData.Rows.Count; i++)
                                {
                                    dr1 = dtData.Rows[i];

                                    foreach (RptExportTemplateCell tcell in tmp.Cells)
                                    {
                                        r = tmp.Direction == FillDirection.Vertical
                                                ? (i + tmp.BeginIdx)
                                                : tcell.RowIdx;
                                        c = tmp.Direction == FillDirection.Vertical
                                                ? tcell.ColumnIdx
                                                : (i + tmp.BeginIdx);
                                        row = sheet.GetRow(r);
                                        cell = row.GetCell(c);

                                        if (cell == null)
                                        {
                                            cell = row.CreateCell(c);
                                        }

                                        if (string.IsNullOrWhiteSpace(tcell.KeyOfEn))
                                            mattr = frmAttrs[tcell.FK_DtlMapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_DtlMapData + "_" + tcell.DtlKeyOfEn) as MapAttr;
                                        else
                                            mattr = frmAttrs[tcell.FK_MapData].GetEntityByKey(MapAttrAttr.MyPK, tcell.FK_MapData + "_" + tcell.KeyOfEn) as MapAttr;

                                        switch (mattr.MyDataType)
                                        {
                                            case DataType.AppString:
                                                cell.SetCellValue(dr1[mattr.MyPK] as string);
                                                break;
                                            case DataType.AppInt:
                                                if (mattr.LGType == FieldTypeS.Normal)
                                                    cell.SetCellValue((int)dr1[mattr.MyPK]);
                                                else
                                                    cell.SetCellValue(dr1[mattr.MyPK] as string);
                                                break;
                                            case DataType.AppFloat:
                                            case DataType.AppMoney:
                                                if (mattr.LGType == FieldTypeS.Normal)
                                                    cell.SetCellValue((double)dr1[mattr.MyPK]);
                                                else
                                                    cell.SetCellValue(dr1[mattr.MyPK] as string);
                                                break;
                                            case DataType.AppDate:
                                                cell.SetCellValue(dr1[mattr.MyPK] as string);
                                                cell.CellStyle.DataFormat = fmt.GetFormat("yyyy-m-d;@");
                                                break;
                                            case DataType.AppDateTime:
                                                cell.SetCellValue(dr1[mattr.MyPK] as string);
                                                cell.CellStyle.DataFormat = fmt.GetFormat("yyyy-m-d h:mm;@");
                                                break;
                                            case DataType.AppBoolean:
                                                cell.SetCellValue((bool)dr1[mattr.MyPK]);
                                                break;
                                            default:
                                                throw new Exception("未涉及到的数据类型，请检查数据是否正确。");
                                        }
                                    }
                                }

                                //弹出下载
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    wb.Write(ms);
                                    byte[] bs = ms.GetBuffer(); //2016-12-17，直接使用ms会报错，所以先将流内容存储出来再使用

                                    Response.AddHeader("Content-Length", bs.Length.ToString());
                                    Response.ContentType = "application/octet-stream";
                                    Response.AddHeader("Content-Disposition",
                                                       "attachment; filename=" +
                                                       HttpUtility.UrlEncode(
                                                           mdMyRpt.Name + "_" +
                                                           DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                                                           Path.GetExtension(tmpFile), Encoding.UTF8));
                                    Response.BinaryWrite(bs);
                                    wb = null;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("数据导出时出现错误，@错误信息：" + ex.Message);
                        }

                        this.SetDGData();
                        #endregion
                        return;
                    default:
                        this.PageIdx = 1;
                        this.SetDGData(1);
                        this.ToolBar1.SaveSearchState(this.RptNo, null);
                        return;
                }
            }
            catch (Exception ex)
            {
                if (!(ex is System.Threading.ThreadAbortException))
                {
                    this.ResponseWriteRedMsg(ex);
                    //在这里显示错误
                }
            }

            if (btn.ID == NamesOfBtn.ExportByTemplate)
                Response.End();
        }

        /// <summary>
        /// 获取指定字段的拼接字符串形式，用英文逗号相连
        /// </summary>
        /// <param name="ens">实体集合</param>
        /// <param name="field">字段</param>
        /// <param name="isVarchar">值是否是字符</param>
        /// <returns></returns>
        private string GetOidsJoin(Entities ens, string field, bool isVarchar)
        {
            string oids = string.Empty;

            foreach (Entity en1 in ens)
            {
                oids += (isVarchar ? "'" : "") + en1.GetValByKey(field) + (isVarchar ? "'" : "") + ",";
            }

            return "(" + oids.TrimEnd(',') + ")";
        }
    }
}