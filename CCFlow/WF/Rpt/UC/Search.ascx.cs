using System;
using System.Data;
using System.Collections.Generic;
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
                if (string.IsNullOrEmpty(s))
                    return this.EnsName;
                return s;
            }
        }
        public new  string EnsName
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
                if (string.IsNullOrEmpty(s))
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
            if (string.IsNullOrEmpty(flowNo))
            {
                this.currMapRpt.FK_Flow = this.FK_Flow;
                this.currMapRpt.Update();
            }

           Flow fl = new Flow(this.currMapRpt.FK_Flow);

           this.Page.Title = fl.Name;

            //初始化查询工具栏.
            this.ToolBar1.InitToolbarOfMapRpt(fl, currMapRpt, this.RptNo, en, 1);


            //增加发起.
            if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, BP.Web.WebUser.No) == true)
            {
                string str = "<div style='float:right'><a href=\"javascript:WinOpen('/WF/MyFlow.aspx?FK_Flow=" + this.FK_Flow + "','df');\" ><img src='/WF/Img/Start.png' width='12px' border=0/>&nbsp;发起</a></div>";
                this.ToolBar1.Add(str);
            }

            if (BP.Web.WebUser.No == "admin")
            {
                string url = "/WF/Rpt/OneFlow.aspx?FK_MapData=ND" + int.Parse(this.FK_Flow) + "Rpt&FK_Flow=" + this.FK_Flow;

                //  string str = "<div style='float:right'><a href=\"javascript:Setting('"+this.RptNo+"','"+this.FK_Flow+"');\" >设置</a></div>";
                string str = "<div style='float:right'><a href='" + url + "' ><img src='/WF/Img/Setting.png' width='12px' border=0/>&nbsp;设置</a></div>";
                this.ToolBar1.Add(str);
            }


            this.ToolBar1.AddLinkBtn(BP.Web.Controls.NamesOfBtn.Export); //导出.

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

            #endregion 处理查询权限

         

            //处理按钮.
            this.SetDGData();
        }

        void ddl_SelectedIndexChanged_GoTo(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            string item = ddl.SelectedItemStringVal;

            string tKey = DateTime.Now.ToString("MMddhhmmss");
            this.Response.Redirect(item + ".aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow+"&T="+tKey,true);
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
               
                MapRpt myRpt=new MapRpt( "ND"+int.Parse(this.FK_Flow)+"MyRpt");
                myRpt.ResetIt();
                //清缓存
                BP.DA.Cash.Map_Cash.Clear();

                return this.SetDGData(this.PageIdx);
            }
        }

        public Entities SetDGData(int pageIdx)
        {
            #region 执行数据分页查询，并绑定分页控件.

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
                                case DataType.AppRate:
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
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            #endregion 定义变量.

            this.UCSys1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%;line-height:22px'");

            #region  生成表格标题
            this.UCSys1.AddTR();
            this.UCSys1.AddTDGroupTitle("style='text-align:center;width:40px;'","序");
            this.UCSys1.AddTDGroupTitle("标题");

            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr
                    || attr.Key == "Title" 
                    || attr.Key=="MyNum")
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
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.aspx?FK_Flow=" + this.currMapRpt.FK_Flow + "&WorkID=" + en.GetValStrByKey("OID") +   "','tdr');\" >" + en.GetValByKey("Title") + "</a>");
                
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr || attr.Key == "MyNum" || attr.Key == "Title")
                        continue;

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        string s = en.GetValRefTextByKey(attr.Key);
                        if (string.IsNullOrEmpty(s))
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
                        case DataType.AppRate:
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
                this.UCSys1.AddTD();
                this.UCSys1.AddTD("合计");
                foreach (Attr attr in attrs)
                {
                    //if (attr.Key == "MyNum")
                    //    continue;
                    if (attr.MyFieldType == FieldType.RefText
                    || attr.Key == "Title"
                    || attr.Key == "MyNum")
                        continue;

                    if (attr.UIContralType == UIContralType.DDL)
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
            try
            {
                var btn = (LinkBtn)sender;

                switch (btn.ID)
                {
                    case NamesOfBtn.Export: //数据导出.
                    case NamesOfBtn.Excel: //数据导出
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
                            myDT.Columns.Add(new DataColumn(attr.Name, typeof(string)));
                        }

                        foreach (DataRow dr in dt.Rows)
                        {
                            DataRow myDR = myDT.NewRow();
                            foreach (MapAttr attr in attrs)
                            {
                                myDR[attr.Name] = dr[attr.Field];
                            }
                            myDT.Rows.Add(myDR);
                        }

                        try
                        {
                            ExportDGToExcel(myDT, en.EnDesc);
                        }
                        catch (Exception ex)
                        {
                                throw new Exception("数据没有正确导出可能的原因之一是:系统管理员没正确的安装Excel组件，请通知他，参考安装说明书解决。@系统异常信息：" + ex.Message);
                        }

                        this.SetDGData();
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
        }
    }
}