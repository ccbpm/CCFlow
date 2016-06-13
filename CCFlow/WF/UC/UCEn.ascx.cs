//===========================================================================
// 此文件是作为 ASP.NET 2.0 Web 项目转换的一部分修改的。
// 类名已更改，且类已修改为从文件“App_Code\Migrated\comm\uc\Stub_ucen_ascx_cs.cs”的抽象基类 
// 继承。
// 在运行时，此项允许您的 Web 应用程序中的其他类使用该抽象基类绑定和访问.
// 代码隐藏页。 
// 关联的内容页“comm\uc\ucen.ascx”也已修改，以引用新的类名。
// 有关此代码模式的更多信息，请参考 http://go.microsoft.com/fwlink/?LinkId=46995
//===========================================================================
namespace CCFlow.WF.UC
{
    using System;
    using System.IO;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Web.UI;
    using BP;
    using BP.En;
    using BP.Sys;
    using BP.Sys.XML;
    using BP.DA;
    using BP.Web;
    using BP.Web.Controls;
    using BP.WF;
    using BP.WF.Template;
    /// <summary>
    ///	UCEn 的摘要说明。
    /// </summary>
    public partial class UCEn : BP.Web.UC.UCBase3
    {
        #region add 2010-07-24 处理实体绑定的第二个算法

        #region add varable.
        public BP.Sys.GroupField currGF = new BP.Sys.GroupField();
        public MapDtls dtls;
        public MapFrames frames;
        public MapM2Ms m2ms;
        public FrmAttachments aths;
        private GroupFields gfs;
        public int rowIdx = 0;
        public bool isLeftNext = true;
        public int FK_Node = 0;
        public string CCFlowAppPath = BP.WF.Glo.CCFlowAppPath;


        #endregion add varable.

        public void BindColumn2(Entity en, string enName)
        {
            this.ctrlUseSta = "";
            this.EnName = enName;
            this.HisEn = en;
            this.mapData = new MapData(enName);
            currGF = new GroupField();
            MapAttrs mattrs = this.mapData.MapAttrs;
            gfs = this.mapData.GroupFields;
            dtls = this.mapData.MapDtls;
            frames = this.mapData.MapFrames;
            m2ms = this.mapData.MapM2Ms;
            aths = this.mapData.FrmAttachments;
            mes = this.mapData.MapExts;

            #region 处理事件.
            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadBefore, en);
                    if (string.IsNullOrEmpty(msg) == false)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            //处理默认值.
            this.DealDefVal(mattrs);
            //处理装载前填充.
            this.LoadData(mattrs, en);
            string appPath = BP.WF.Glo.CCFlowAppPath; //this.Page.Request.ApplicationPath;

            this.Add("<table width=100% >");
            foreach (BP.Sys.GroupField gf in gfs)
            {
                currGF = gf;
                this.AddTR();
                this.AddTD("colspan=2 class=GroupField valign='top' align=left ", "<div style='text-align:left; float:left'>&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                this.AddTREnd();

                int idx = -1;
                isLeftNext = true;
                rowIdx = 0;

                #region 增加字段.
                foreach (MapAttr attr in mattrs)
                {
                    #region 排除
                    if (attr.GroupID != gf.OID)
                    {
                        if (gf.Idx == 0 && attr.GroupID == 0)
                        {
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                        continue;
                    #endregion 排除

                    #region 设置
                    rowIdx++;
                    this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "'");
                    if (attr.UIIsEnable == false)
                    {
                        if (this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                        {
                            MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                            string url = meLink.Tag;
                            if (url.Contains("?") == false)
                                url = url + "?a3=2";
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + enName;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + CCFlowAppPath);
                            if (url.Contains("@"))
                            {
                                Attrs attrs = en.EnMap.Attrs;
                                foreach (Attr item in attrs)
                                {
                                    url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                            }
                            this.AddTD("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                            this.AddTREnd();
                            continue;
                        }
                    }
                    #endregion 设置

                    #region 加入字段
                    // 显示的顺序号.
                    idx++;
                    if (attr.IsBigDoc && attr.UIIsLine)
                    {
                        if (attr.UIIsEnable)
                            this.Add("<TD colspan=2 height='" + attr.UIHeight.ToString() + "px'    width='100%' valign=top align=left>" + attr.Name + "<br>");
                        else
                            this.Add("<TD colspan=2 height='" + attr.UIHeight.ToString() + "px'   width='100%' valign=top class=TBReadonly>" + attr.Name + "<br>");

                        TB mytbLine = new TB();
                        if (attr.IsBigDoc)
                        {
                            mytbLine.TextMode = TextBoxMode.MultiLine;
                            mytbLine.Attributes["class"] = "TBDoc";
                        }

                        mytbLine.ID = "TB_" + attr.KeyOfEn;
                        if (attr.IsBigDoc)
                        {
                            //  mytbLine = 5;
                            // mytbLine.Columns = 30;
                        }

                        mytbLine.Attributes["style"] = "width:98%;height:100%;padding: 0px;margin: 0px;";
                        mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn);
                        mytbLine.Enabled = attr.UIIsEnable;

                        this.Add(mytbLine);
                        this.AddTDEnd();
                        this.AddTREnd();
                        rowIdx++;
                        continue;
                    }

                    TB tb = new TB();
                    tb.Attributes["width"] = "100%";
                    tb.Attributes["border"] = "1px";
                    tb.Columns = 40;
                    tb.ID = "TB_" + attr.KeyOfEn;
                    Control ctl = tb;

                    #region add contrals.
                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            tb.Enabled = attr.UIIsEnable;
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                    tb.ShowType = TBType.TB;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    break;
                                case BP.DA.DataType.AppDate:
                                    tb.ShowType = TBType.Date;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker();";
                                    break;
                                case BP.DA.DataType.AppDateTime:
                                    tb.ShowType = TBType.DateTime;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    CheckBox cb = new CheckBox();
                                    cb.Text = attr.Name;
                                    cb.ID = "CB_" + attr.KeyOfEn;
                                    cb.Checked = attr.DefValOfBool;
                                    cb.Enabled = attr.UIIsEnable;
                                    cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                    this.AddTD("colspan=2", cb);
                                    continue;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppFloat:
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    break;
                                case BP.DA.DataType.AppInt:
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d]/g,'')");
                                    break;
                                case BP.DA.DataType.AppMoney:
                                case BP.DA.DataType.AppRate:
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = decimal.Parse(en.GetValStrByKey(attr.KeyOfEn)).ToString("0.00");
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    break;
                                default:
                                    break;
                            }
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
                            DDL ddle = new DDL();
                            ddle.ID = "DDL_" + attr.KeyOfEn;
                            ddle.BindSysEnum(attr.UIBindKey);
                            ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            ddle.Enabled = attr.UIIsEnable;
                            ctl = ddle;
                            break;
                        case FieldTypeS.FK:
                            DDL ddl1 = new DDL();
                            ddl1.ID = "DDL_" + attr.KeyOfEn;
                            try
                            {
                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                ens.RetrieveAll();
                                ddl1.BindEntities(ens);
                                ddl1.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            }
                            catch
                            {

                            }
                            ddl1.Enabled = attr.UIIsEnable;
                            ctl = ddl1;
                            break;
                        default:
                            break;
                    }
                    #endregion add contrals.

                    string desc = attr.Name.Replace("：", "");
                    desc = desc.Replace(":", "");
                    desc = desc.Replace(" ", "");

                    if (desc.Length >= 5)
                    {
                        this.Add("<TD colspan=2 class=FDesc width='100%' ><div style='float:left'>" + desc + "</div><br>");
                        this.Add(ctl);
                        this.AddTREnd();
                    }
                    else
                    {
                        this.AddTDDesc(desc);
                        this.AddTD("width='100%' class=TBReadonly", ctl);
                        this.AddTREnd();
                    }
                    #endregion 加入字段

                }
                #endregion 增加字段.

                // 插入col.
                string fid = "0";
                try
                {
                    fid = en.GetValStrByKey("FID");
                }
                catch
                {
                }
                this.InsertObjects2Col(true, en.PKVal.ToString(), fid);
            }
            this.AddTableEnd();


            #region 处理iFrom 的自适应的问题。
            string js = "\t\n<script type='text/javascript' >";
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                js += "\t\n window.setInterval(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
            }
            foreach (MapFrame fr in frames)
            {
                //  if (fr.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + fr.NoOfObj + "','TD" + fr.NoOfObj + "')\", 200);";
            }
            foreach (MapM2M m2m in m2ms)
            {
                //  if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + m2m.NoOfObj + "','TD" + m2m.NoOfObj + "')\", 200);";
            }
            foreach (FrmAttachment ath in aths)
            {
                // if (ath.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + ath.MyPK + "','TD" + ath.MyPK + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom 的自适应的问题。

            // 处理扩展。
            this.AfterBindEn_DealMapExt(enName, mattrs, en);
            if (this.IsReadonly == false)
            {
                #region 处理iFrom SaveDtlData。
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveDtl(dtl) { ";
                //    js += "\t\n    GenerPageKVs(); //调用产生kvs ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveDtlData(); ";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom SaveDtlData。

                #region 处理iFrom  SaveM2M Save
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveM2M(dtl) { ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom  SaveM2M Save。
            }
        }
        public string _tempAddDtls = "";
        public void InsertObjects2Col(bool isJudgeRowIdx, string pk, string fid)
        {
            #region 从表
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                if (_tempAddDtls.Contains(dtl.No))
                    continue;

                //if (dtl.IsUse)
                //    continue;

                //if (isJudgeRowIdx)
                //{
                //    if (dtl.RowIdx != rowIdx)
                //        continue;
                //}

                if (dtl.GroupID != currGF.OID)
                    continue;

                if (dtl.GroupID == 0 && rowIdx == 0)
                {
                    dtl.GroupID = currGF.OID;
                    dtl.RowIdx = 0;
                    dtl.Update();
                }

                dtl.IsUse = true;
                rowIdx++;

                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=2 ID='TD" + dtl.No + "' height='100px' width='100%' style='align:left'>");
                string src = "";
                try
                {
                    src = CCFlowAppPath + "WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&FID=" + this.HisEn.GetValStringByKey("FID") + "&IsWap=0&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }
                catch
                {
                    src = CCFlowAppPath + "WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&IsWap=0&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }

                if (this.IsReadonly || dtl.IsReadonly)
                    this.Add("<iframe ID='F" + dtl.No + "'  src='" + src +
                             "&IsReadonly=1' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='100px' />");
                else
                {
                    AddLoadFunction(dtl.No, "blur", "SaveDtl");
                    //this.Add("<iframe ID='F" + dtl.No + "'   Onblur=\"SaveDtl('" + dtl.No + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='100px' />");

                    this.Add("<iframe ID='F" + dtl.No + "'  onload='" + dtl.No + "load();'    src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='100px' />");

                }


                this.AddTDEnd();
                this.AddTREnd();
                _tempAddDtls += dtl.No;

                // 下面使用Link 的方案.
                //// myidx++;
                //this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                //string src = "";
                //try
                //{
                //    src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&FID=" + this.HisEn.GetValStringByKey("FID") + "&IsWap=1&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                //}
                //catch
                //{
                //    src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&IsWap=1&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                //}
                //_tempAddDtls += dtl.No;
                //this.Add("<TD colspan=2 class=FDesc ID='TD" + dtl.No + "'><a href='" + src + "'>" + dtl.Name + "</a></TD>");
                //// this.Add("<iframe ID='F" + dtl.No + "' frameborder=0 Onblur=\"SaveDtl('" + dtl.No + "');\" style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' height='10px' scrolling=no  /></iframe>");
                ////this.AddTDEnd();
                //this.AddTREnd();
            }
            #endregion 从表

            #region 框架表
            foreach (MapFrame fram in frames)
            {
                if (fram.IsUse)
                    continue;

                if (isJudgeRowIdx)
                {
                    if (fram.RowIdx != rowIdx)
                        continue;
                }

                if (fram.GroupID == 0 && rowIdx == 0)
                {
                    fram.GroupID = currGF.OID;
                    fram.RowIdx = 0;
                    fram.Update();
                }
                else if (fram.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                fram.IsUse = true;
                rowIdx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                string src = fram.URL;

                if (src.Contains("?"))
                    src += "&Table=" + fram.FK_MapData + "&WorkID=" + pk + "&FID=" + fid;
                else
                    src += "?Table=" + fram.FK_MapData + "&WorkID=" + pk + "&FID=" + fid;
                this.Add("<TD colspan=2 class=FDesc ID='TD" + fram.NoOfObj + "'><a href='" + src + "'>" + fram.Name + "</a></TD>");
                this.AddTREnd();
            }
            #endregion 从表

            #region 附件
            foreach (FrmAttachment ath in aths)
            {
                if (ath.IsUse)
                    continue;
                if (isJudgeRowIdx)
                {
                    if (ath.RowIdx != rowIdx)
                        continue;
                }

                if (ath.GroupID == 0 && rowIdx == 0)
                {
                    ath.GroupID = currGF.OID;
                    ath.RowIdx = 0;
                    ath.Update();
                }
                else if (ath.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                ath.IsUse = true;
                rowIdx++;

                string src = CCFlowAppPath + "WF/CCForm/AttachmentUpload.aspx?IsWap=1&PKVal=" + this.HisEn.PKVal + "&NoOfObj=" + ath.NoOfObj + "&FK_MapData=" + EnsName + "&FK_FrmAttachment=" + ath.MyPK + this.RequestParas;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=2 class=FDesc ID='TD" + ath.NoOfObj + "'><a href='" + src + "'>" + ath.Name + "</a></TD>");
                this.AddTREnd();
            }
            #endregion 附件

            #region 多对多的关系
            foreach (MapM2M m2m in m2ms)
            {
                if (m2m.IsUse)
                    continue;

                if (isJudgeRowIdx)
                {
                    if (m2m.RowIdx != rowIdx)
                        continue;
                }

                if (m2m.GroupID == 0 && rowIdx == 0)
                {
                    m2m.GroupID = currGF.OID;
                    m2m.RowIdx = 0;
                    m2m.Update();
                }
                else if (m2m.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                m2m.IsUse = true;
                rowIdx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                    this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='50px' width='100%'  >");
                else
                    this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='" + m2m.H + "' width='" + m2m.W + "'  >");

                string src = "";
                if (m2m.HisM2MType == M2MType.M2M)
                    src = CCFlowAppPath + "WF/CCForm/M2M.aspx?NoOfObj=" + m2m.NoOfObj;
                else
                    src = CCFlowAppPath + "WF/CCForm/M2MM.aspx?NoOfObj=" + m2m.NoOfObj;

                string paras = this.RequestParas;

                if (paras.Contains("FID=") == false)
                    paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");

                src += "&r=q" + paras;

                if (src.Contains("FK_MapData") == false)
                    src += "&FK_MapData=" + m2m.FK_MapData;

                switch (m2m.ShowWay)
                {
                    case FrmShowWay.FrmAutoSize:
                        if (m2m.IsEdit)
                        {
                            AddLoadFunction(m2m.NoOfObj, "blur", "SaveM2M");
                            // this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");

                            this.Add("<iframe ID='F" + m2m.NoOfObj + "' onload='" + m2m.NoOfObj + "load();'    src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");

                        }
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                        break;
                    case FrmShowWay.FrmSpecSize:
                        if (m2m.IsEdit)
                        {
                            // this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                            AddLoadFunction(m2m.NoOfObj, "blur", "SaveM2M");

                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   onload='" + m2m.NoOfObj + "load();' src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");

                        }
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'    src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                        break;
                    case FrmShowWay.Hidden:
                        break;
                    case FrmShowWay.WinOpen:
                        this.Add("<a href=\"javascript:WinOpen('" + src + "&IsOpen=1','" + m2m.W + "','" + m2m.H + "');\"  />" + m2m.Name + "</a>");
                        break;
                    default:
                        break;
                }
            }
            #endregion 多对多的关系
        }
        public MapExts mes = null;
        public bool IsLoadData = false;

        public void LoadData(MapAttrs mattrs, Entity en)
        {

            this.LinkFields = "";
            if (mes.Count == 0)
                return;
            foreach (MapExt myitem in mes)
            {
                if (myitem.ExtType == MapExtXmlList.Link)
                    this.LinkFields += "," + myitem.AttrOfOper + ",";
            }

            if (this.IsLoadData == false)
                return;


            MapExt item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            if (item == null)
                return;

            //执行通用的装载方法.
            en = BP.WF.Glo.DealPageLoadFull(en, item, mattrs, dtls);
        }
        public string ctrlUseSta = "";
        public int idx = 0;
        public void BindColumn4(Entity en, string enName)
        {
            this.ctrlUseSta = "";
            this.EnName = enName;
            this.HisEn = en;
            this.mapData = new MapData(enName);
            currGF = new GroupField();
            MapAttrs mattrs = this.mapData.MapAttrs;
            gfs = this.mapData.GroupFields;
            dtls = this.mapData.MapDtls;
            frames = this.mapData.MapFrames;
            m2ms = this.mapData.MapM2Ms;
            aths = this.mapData.FrmAttachments;
            mes = this.mapData.MapExts;

            #region 处理事件.
            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadBefore, en);
                    if (string.IsNullOrEmpty(msg) == false)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    //string msg = ex.Message;
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            //处理默认值.
            this.DealDefVal(mattrs);
            //处理装载前填充.
            this.LoadData(mattrs, en);
            string appPath = CCFlowAppPath; //this.Page.Request.ApplicationPath;

            #region 计算出来列的宽度.
            int labCol = 80;
            int ctrlCol = 260;
            int width = (labCol + ctrlCol) * mapData.TableCol / 2;
            #endregion 计算出来列的宽度.

            #region 生成表头.
            this.Add("\t\n<Table style='width:" + width + "px;' align=left>");

            this.AddTREnd();
            #endregion 生成表头.

            foreach (GroupField gf in gfs)
            {
                currGF = gf;
                this.AddTR();
                if (gfs.Count == 1)
                    this.AddTD("colspan=" + this.mapData.TableCol + " style='width:" + width + "px' class=GroupField valign='top' align=left ", "<div style='text-align:left; float:left'>&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                else
                    this.AddTD("colspan=" + this.mapData.TableCol + " style='width:" + width + "px' class=GroupField valign='top' align=left  onclick=\"GroupBarClick('" + gf.Idx + "')\"  ", "<div style='text-align:left; float:left'>&nbsp;<img src='" + CCFlowAppPath + "WF/Style/Min.gif' alert='Min' id='Img" + gf.Idx + "' border=0 />&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                this.AddTREnd();

                idx = -1;

                rowIdx = 0;
                int colSpan = this.mapData.TableCol;  // 定义colspan的宽度.
                this.AddTR();
                for (int i = 0; i < mattrs.Count; i++)
                {
                    MapAttr attr = mattrs[i] as MapAttr;

                    #region 过滤不显示的字段.
                    if (attr.GroupID != gf.OID)
                    {
                        if (gf.Idx == 0 && attr.GroupID == 0)
                        {
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                        continue;

                    if (colSpan == 0)
                        this.InsertObjects(true);
                    #endregion 过滤不显示的字段.

                    #region 补充空白的列.
                    if (colSpan <= 0)
                    {
                        /*如果列已经用完.*/
                        this.AddTREnd();
                        colSpan = this.mapData.TableCol; //补充列.
                        rowIdx++;
                    }
                    #endregion 补充空白的列.

                    #region 处理大块文本的输出.
                    // 显示的顺序号.
                    idx++;
                    if (attr.IsBigDoc && (attr.ColSpan == this.mapData.TableCol || attr.ColSpan == 0))
                    {
                        int h = attr.UIHeightInt + 20;
                        if (attr.UIIsEnable)
                            this.Add("<TD height='" + h.ToString() + "px'  colspan=" + this.mapData.TableCol + " width='100%' valign=top align=left>");
                        else
                            this.Add("<TD height='" + h.ToString() + "px'  colspan=" + this.mapData.TableCol + " width='100%' valign=top class=TBReadonly>");

                        this.Add("<div style='font-size:12px;color:black;' >");
                        Label lab = new Label();
                        lab.ID = "Lab" + attr.KeyOfEn;
                        lab.Text = attr.Name;
                        this.Add(lab);
                        this.Add("</div>");
                        if (attr.TBModel == 2)
                        {
                            //富文本输出.
                            this.AddRichTextBox(en, attr);
                        }
                        else
                        {
                            TB mytbLine = new TB();
                            mytbLine.TextMode = TextBoxMode.MultiLine;
                            mytbLine.ID = "TB_" + attr.KeyOfEn;
                            mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn).Replace("\\n", "\n");

                            mytbLine.Enabled = attr.UIIsEnable;
                            if (mytbLine.Enabled == false)
                                mytbLine.Attributes.Add("readonly", "true");
                            else
                                mytbLine.Attributes["class"] = "TBDoc";

                            mytbLine.Attributes["style"] = "width:98%;height:" + attr.UIHeight + "px;padding: 0px;margin: 0px;";
                            this.Add(mytbLine);

                            if (mytbLine.Enabled)
                            {
                                string ctlID = mytbLine.ClientID;
                                Label mylab = this.GetLabelByID("Lab" + attr.KeyOfEn);
                                mylab.Text = "<a href=\"javascript:TBHelp('" + ctlID + "','" + appPath + "','" + enName + "','" + attr.KeyOfEn + "','" + enName + "')\">" + attr.Name + "</a>";
                            }
                        }

                        this.AddTDEnd();
                        this.AddTREnd();
                        rowIdx++;
                        isLeftNext = true;
                        continue;
                    }

                    if (attr.IsBigDoc)
                    {
                        if (colSpan == this.mapData.TableCol)
                        {
                            /*已经加满了*/
                            this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                            colSpan = colSpan - attr.ColSpan; // 减去已经占用的col.
                        }

                        this.Add("<TD class=FDesc colspan=" + attr.ColSpan + " height='" + attr.UIHeight.ToString() + "px' >");
                        this.Add(attr.Name);
                        TB mytbLine = new TB();
                        mytbLine.ID = "TB_" + attr.KeyOfEn;
                        mytbLine.TextMode = TextBoxMode.MultiLine;
                        mytbLine.Attributes["class"] = "TBDoc";
                        mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn);
                        if (mytbLine.Enabled == false)
                        {
                            mytbLine.Attributes["class"] = "TBReadonly";
                            mytbLine.Attributes.Add("readonly", "true");
                        }
                        mytbLine.Attributes["style"] = "width:98%;height:100%;padding: 0px;margin: 0px;";
                        this.Add(mytbLine);
                        this.AddTDEnd();
                        continue;
                    }
                    #endregion 大块文本的输出.

                    #region 处理超链接
                    if (attr.UIIsEnable == false)
                    {
                        /* 判断是否有隐藏的超链接字段. */
                        if (this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                        {
                            MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                            string url = meLink.Tag;
                            if (url.Contains("?") == false)
                                url = url + "?a3=2";
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + enName;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + CCFlowAppPath);
                            if (url.Contains("@"))
                            {
                                Attrs attrs = en.EnMap.Attrs;
                                foreach (Attr item in attrs)
                                {
                                    url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                            }
                            this.AddTD("colspan=" + colSpan, "<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                            continue;
                        }
                    }
                    #endregion 处理超链接

                    #region  首先判断当前剩余的单元格是否满足当前控件的需要。
                    if (attr.ColSpan + 1 > mapData.TableCol)
                        attr.ColSpan = this.mapData.TableCol - 1; //如果设置的

                    if (colSpan < attr.ColSpan + 1 || colSpan == 1 || colSpan == 0)
                    {
                        /*如果剩余的列不能满足当前的单元格，就补充上它，让它换行.*/
                        if (colSpan != 0)
                            this.AddTD("colspan=" + colSpan, "");
                        this.AddTREnd();

                        colSpan = mapData.TableCol;
                        this.AddTR();
                    }
                    #endregion  首先判断当前剩余的单元格是否满足当前控件的需要。

                    #region 其它的就是增加一列控件一列描述的字段.
                    TB tb = new TB();
                    tb.ID = "TB_" + attr.KeyOfEn;
                    tb.Enabled = attr.UIIsEnable;
                    colSpan = colSpan - 1 - attr.ColSpan; // 首先减去当前的占位.
                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                    this.AddTDDesc(attr.Name);
                                    if (attr.IsSigan)
                                    {
                                        string v = en.GetValStrByKey(attr.KeyOfEn);
                                        if (v.Length == 0)
                                            this.AddTD("colspan=" + attr.ColSpan, "<img src='" + CCFlowAppPath + "DataUser/Siganture/" + WebUser.No + ".jpg' border=0 onerror=\"this.src='" + CCFlowAppPath + "DataUser/Siganture/UnName.jpg'\"/>");
                                        else
                                            this.AddTD("colspan=" + attr.ColSpan, "<img src='" + CCFlowAppPath + "DataUser/Siganture/" + v + ".jpg' border=0 onerror=\"this.src='" + CCFlowAppPath + "DataUser/Siganture/UnName.jpg'\"/>");
                                    }
                                    else
                                    {
                                        tb.ShowType = TBType.TB;
                                        tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                        tb.Attributes["width"] = "100%";
                                        this.AddTD("colspan=" + attr.ColSpan, tb);
                                    }
                                    break;
                                case BP.DA.DataType.AppDate:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Date;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker();";

                                    this.AddTD("colspan=" + attr.ColSpan, tb);
                                    break;
                                case BP.DA.DataType.AppDateTime:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.DateTime;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";

                                    this.AddTD("colspan=" + attr.ColSpan, tb);
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    this.AddTDDesc("");
                                    CheckBox cb = new CheckBox();
                                    cb.Text = attr.Name;
                                    cb.ID = "CB_" + attr.KeyOfEn;
                                    cb.Checked = attr.DefValOfBool;
                                    cb.Enabled = attr.UIIsEnable;
                                    cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                    this.AddTD("colspan=" + attr.ColSpan, cb);
                                    break;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppFloat:
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    this.AddTD("colspan=" + attr.ColSpan, tb);
                                    break;
                                case BP.DA.DataType.AppInt:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Num;
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d]/g,'')");
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    this.AddTD("colspan=" + attr.ColSpan, tb);
                                    break;
                                case BP.DA.DataType.AppMoney:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Moneny;

                                    if (SystemConfig.AppSettings["IsEnableNull"] == "1")
                                    {
                                        decimal v = en.GetValMoneyByKey(attr.KeyOfEn);
                                        if (v == 567567567)
                                            tb.Text = "";
                                        else
                                            tb.Text = v.ToString("0.00");
                                    }
                                    else
                                        tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");

                                    //tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");

                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    this.AddTD("colspan=" + attr.ColSpan, tb);
                                    break;
                                case BP.DA.DataType.AppRate:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    this.AddTD("colspan=" + attr.ColSpan, tb);
                                    break;
                                default:
                                    break;
                            }
                            // tb.Attributes["width"] = "100%";
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                case BP.DA.DataType.AppDateTime:
                                case BP.DA.DataType.AppDate:
                                    if (tb.Enabled)
                                    {
                                        tb.MaxLength = attr.MaxLen;
                                    }
                                    else
                                    {
                                        tb.Attributes["class"] = "TBReadonly";
                                    }
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
                            if (attr.UIContralType == UIContralType.DDL)
                            {
                                this.AddTDDesc(attr.Name);
                                DDL ddle = new DDL();
                                ddle.ID = "DDL_" + attr.KeyOfEn;
                                ddle.BindSysEnum(attr.UIBindKey);
                                ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                                ddle.Enabled = attr.UIIsEnable;
                                this.AddTD("colspan=" + attr.ColSpan, ddle);
                            }
                            else
                            {
                                this.AddTDDesc(attr.Name);
                                this.Add("<TD class=TD colspan='" + attr.ColSpan + "'>");
                                SysEnums ses = new SysEnums(attr.UIBindKey);
                                foreach (SysEnum item in ses)
                                {
                                    RadioButton rb = new RadioButton();
                                    rb.ID = "RB_" + attr.KeyOfEn + "_" + item.IntKey;
                                    rb.Text = item.Lab;
                                    if (item.IntKey == en.GetValIntByKey(attr.KeyOfEn))
                                        rb.Checked = true;
                                    else
                                        rb.Checked = false;
                                    rb.GroupName = attr.KeyOfEn;
                                    this.Add(rb);
                                }
                                this.AddTDEnd();
                            }
                            break;
                        case FieldTypeS.FK:
                            this.AddTDDesc(attr.Name);
                            DDL ddl1 = new DDL();
                            ddl1.ID = "DDL_" + attr.KeyOfEn;
                            try
                            {
                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                ens.RetrieveAll();
                                ddl1.BindEntities(ens);
                                ddl1.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            }
                            catch
                            {
                            }
                            ddl1.Enabled = attr.UIIsEnable;
                            this.AddTD("colspan=" + attr.ColSpan, ddl1);
                            break;
                        default:
                            break;
                    }
                    #endregion 其它的就是增加一列控件一列描述的字段

                } // 结束字段集合循环.

                // 在分组后处理它, 首先判断当前剩余的单元格是否满足当前控件的需要。
                if (colSpan != this.mapData.TableCol)
                {
                    /* 如果剩余的列不能满足当前的单元格，就补充上它，让它换行.*/
                    if (colSpan != 0)
                        this.AddTD("colspan=" + colSpan, "");

                    this.AddTREnd();
                    colSpan = mapData.TableCol;
                }
                this.InsertObjects(false);
            } // 结束分组循环.


            #region 审核组件
            FrmWorkCheck fwc = new FrmWorkCheck(enName);
            if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Disable)
            {
                rowIdx++;

                this.AddTR();
                this.AddTD("colspan=" + this.mapData.TableCol + " class=GroupField valign='top' align=left ", "<div style='text-align:left; float:left'>&nbsp;审核信息</div><div style='text-align:right; float:right'></div>");
                this.AddTREnd();

                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + enName + "' height='50px' width='100%' style='align:left'>");
                string src = CCFlowAppPath + "WF/WorkOpt/WorkCheck.aspx?s=2";
                string paras = this.RequestParas;
                try
                {
                    if (paras.Contains("FID=") == false)
                        paras += "&FID=" + en.GetValStrByKey("FID");
                }
                catch
                {
                }
                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + en.GetValStrByKey("OID");
                src += "&r=q" + paras;
                this.Add("<iframe ID='F33" + fwc.No + "'  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0'  width='100%'  scrolling=auto/></iframe>");
                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 审核组件


            this.AddTREnd();
            this.AddTableEnd();

            #region 处理iFrom 的自适应的问题。
            string js = "\t\n<script type='text/javascript' >";
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                js += "\t\n window.setInterval(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
            }
            foreach (MapFrame fr in frames)
            {
                //  if (fr.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + fr.NoOfObj + "','TD" + fr.NoOfObj + "')\", 200);";
            }
            foreach (MapM2M m2m in m2ms)
            {
                //  if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + m2m.NoOfObj + "','TD" + m2m.NoOfObj + "')\", 200);";
            }
            foreach (FrmAttachment ath in aths)
            {
                // if (ath.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + ath.MyPK + "','TD" + ath.MyPK + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom 的自适应的问题。

            // 处理扩展。
            this.AfterBindEn_DealMapExt(enName, mattrs, en);
            if (this.IsReadonly == false)
            {
                #region 处理iFrom SaveDtlData。
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveDtl(dtl) { ";
                //    js += "\t\n    GenerPageKVs(); //调用产生kvs ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveDtlData(); ";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom SaveDtlData。

                #region 处理iFrom  SaveM2M Save
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveM2M(dtl) { ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom  SaveM2M Save。
            }
        }
        private void AfterBindEn_DealMapExt(string enName, MapAttrs mattrs, Entity en)
        {
            #region 处理事件.
            if (dtls.Count >= 1)
            {
                string scriptSaveDtl = "";
                scriptSaveDtl = "\t\n<script type='text/javascript' >";
                scriptSaveDtl += "\t\n function SaveDtlAll(){ ";

                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.IsUpdate == true || dtl.IsInsert == true)
                    {
                        scriptSaveDtl += "\t\n try{  ";

                        if (dtl.HisDtlShowModel == DtlShowModel.Table)
                            scriptSaveDtl += "\t\n  SaveDtl('" + dtl.No + "'); ";

                        scriptSaveDtl += "\t\n } catch(e) { ";
                        scriptSaveDtl += "\t\n  alert(e.name  + e.message);  return false;";
                        scriptSaveDtl += "\t\n } ";
                    }
                }

                scriptSaveDtl += "\t\n  return true; } ";
                scriptSaveDtl += "\t\n</script>";

                this.Add(scriptSaveDtl);
            }
            else
            {
                string scriptSaveDtl = "";
                scriptSaveDtl = "\t\n<script type='text/javascript' >";
                scriptSaveDtl += "\t\n function SaveDtlAll() { ";
                scriptSaveDtl += "\t\n return true; } ";
                scriptSaveDtl += "\t\n</script>";
                this.Add(scriptSaveDtl);
            }


            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadAfter, en);
                    if (msg != null)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert("载入之前错误:" + ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            #region 处理扩展设置
            if (mes.Count != 0)
            {
                #region load js.
                this.Page.RegisterClientScriptBlock("s4",
              "<script language='JavaScript' src='" + CCFlowAppPath + "WF/Scripts/jquery-1.7.2.min.js' ></script>");

                this.Page.RegisterClientScriptBlock("b7",
             "<script language='JavaScript' src='" + CCFlowAppPath + "WF/CCForm/MapExt.js' defer='defer' type='text/javascript' ></script>");

                this.Page.RegisterClientScriptBlock("y7",
            "<script language='JavaScript' src='" + CCFlowAppPath + "DataUser/JSLibData/" + enName + ".js' ></script>");

                this.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");
                #endregion load js.

                #region 首先处理 下拉框的数据过滤。
                foreach (MapExt me in mes)
                {
                    switch (me.ExtType)
                    {
                        case MapExtXmlList.AutoFullDLL: // 下拉框的数据过滤.
                            DDL ddlFull = this.GetDDLByID("DDL_" + me.AttrOfOper);
                            if (ddlFull == null)
                            {
                                me.Delete();
                                continue;
                            }

                            string valOld = ddlFull.SelectedItemStringVal;
                            string fullSQL = me.Doc.Clone() as string;
                            if (!IsLoadData)
                            {
                                //替换保存的时候EN中表单中变量
                                foreach (Attr item in en.EnMap.Attrs)
                                {
                                    if (fullSQL.Contains("@" + item.Key + ";"))
                                    {
                                        foreach (string value in this.Request.Params.AllKeys)
                                        {
                                            if (value.EndsWith(item.Key))
                                            {
                                                en.SetValByKey(item.Key, Request[value]);
                                            }
                                        }
                                    }
                                }
                            }

                            fullSQL = BP.WF.Glo.DealExp(fullSQL, en, "");

                            ddlFull.Items.Clear();
                            DataTable table = DBAccess.RunSQLReturnTable(fullSQL);
                            if (table.Rows.Count == 0)
                            {
                                DataRow row = table.NewRow();
                                row["No"] = "";
                                row["Name"] = "*请选择";
                                table.Rows.Add(row);

                            }

                            ddlFull.Bind(table, "No", "Name");

                            string val = "";
                            if (!IsLoadData)
                            {
                                foreach (string value in this.Request.Params.AllKeys)
                                {
                                    if (value.EndsWith(me.AttrOfOper))
                                    {
                                        val = Request[value];
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(val))
                                ddlFull.SetSelectItem(en.GetValStrByKey(me.AttrOfOper));
                            else
                                ddlFull.SetSelectItem(val);

                            break;
                        case MapExtXmlList.AutoFull: // 自动填充下拉框.
                            break;
                        default:
                            break;
                    }
                }
                #endregion 首先处理自动填充，下拉框数据。

                #region 在处理其它。
                System.Data.DataTable dt = new DataTable();
                foreach (MapExt me in mes)
                {
                    switch (me.ExtType)
                    {
                        case MapExtXmlList.DDLFullCtrl: // 自动填充其他的控件..
                            DDL ddlOper = this.GetDDLByID("DDL_" + me.AttrOfOper);
                            if (ddlOper == null)
                                continue;

                            ddlOper.Attributes["onchange"] = "Change('" + enName + "');DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                            if (me.Tag != "")
                            {
                                /* 下拉框填充范围. */
                                string[] strs = me.Tag.Split('$');
                                foreach (string str in strs)
                                {
                                    if (string.IsNullOrEmpty(str))
                                        continue;

                                    string[] myCtl = str.Split(':');
                                    string ctlID = myCtl[0];
                                    DDL ddlC1 = this.GetDDLByID("DDL_" + ctlID);
                                    if (ddlC1 == null)
                                    {
                                        //me.Tag = "";
                                        //me.Update();
                                        continue;
                                    }

                                    //如果触发的dll 数据为空，则不处理.
                                    if (string.IsNullOrEmpty(ddlOper.SelectedItemStringVal.Trim()))
                                        continue;

                                    string sql = myCtl[1].Replace("~", "'");
                                    sql = sql.Replace("@Key", ddlOper.SelectedItemStringVal.Trim());
                                    sql = BP.WF.Glo.DealExp(sql, en, null);

                                    dt = DBAccess.RunSQLReturnTable(sql);
                                    string valC1 = ddlC1.SelectedItemStringVal;
                                    if (dt.Rows.Count != 0)
                                    {
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            ListItem li = ddlC1.Items.FindByValue(dr[0].ToString());
                                            if (li == null)
                                            {
                                                ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                            }
                                            else
                                            {
                                                li.Attributes["visable"] = "false";
                                            }
                                        }
                                        // ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                    }
                                    ddlC1.SetSelectItem(valC1);
                                }
                            }

                            break;
                        case MapExtXmlList.ActiveDDL: /*自动初始化ddl的下拉框数据.*/
                            DDL ddlPerant = this.GetDDLByID("DDL_" + me.AttrOfOper);
                            DDL ddlChild = this.GetDDLByID("DDL_" + me.AttrsOfActive);
                            if (ddlPerant == null || ddlChild == null)
                                continue;
                            ddlPerant.Attributes["onchange"] = "DDLAnsc(this.value,\'" + ddlChild.ClientID + "\', \'" + me.MyPK + "\')";
                            // 处理默认选择。
                            //string val = ddlPerant.SelectedItemStringVal;
                            string valClient = en.GetValStrByKey(me.AttrsOfActive); // ddlChild.SelectedItemStringVal;
                            //此处移至在加载级联菜单控件的地方，edited by liuxc,2015-10-22
                            //string fullSQL = me.Doc.Clone() as string;
                            //fullSQL = fullSQL.Replace("~", ",");
                            //fullSQL = fullSQL.Replace("@Key", val).Replace("@key", val);
                            //fullSQL = fullSQL.Replace("@WebUser.No", WebUser.No);
                            //fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                            //fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                            //fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);

                            //dt = DBAccess.RunSQLReturnTable(fullSQL);
                            ////ddlChild.Items.Clear();
                            //foreach (DataRow dr in dt.Rows)
                            //    ddlChild.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));

                            ddlChild.SetSelectItem(valClient);
                            break;
                        case MapExtXmlList.AutoFullDLL: // 自动填充下拉框.
                            continue; //已经处理了。
                        case MapExtXmlList.TBFullCtrl: // 自动填充.
                            TextBox tbAuto = this.GetTextBoxByID("TB_" + me.AttrOfOper);
                            if (tbAuto == null)
                                continue;

                            // onpropertychange
                            // tbAuto.Attributes["onpropertychange"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            // tbAuto.Attributes["onkeydown"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            // tbAuto.Attributes["onkeyup"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            // tbAuto.Attributes["ondblclick"] = "ReturnValTBFullCtrl(this,'" + me.MyPK + "','sd');";

                            tbAuto.Attributes["ondblclick"] = "ReturnValTBFullCtrl(this,'" + me.MyPK + "');";
                            tbAuto.Attributes["onkeyup"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            tbAuto.Attributes["AUTOCOMPLETE"] = "OFF";
                            if (me.Tag != "")
                            {
                                /* 处理下拉框的选择范围的问题 */
                                string[] strs = me.Tag.Split('$');
                                foreach (string str in strs)
                                {
                                    if (string.IsNullOrEmpty(str))
                                        continue;

                                    string[] myCtl = str.Split(':');
                                    string ctlID = myCtl[0];
                                    DDL ddlC1 = this.GetDDLByID("DDL_" + ctlID);
                                    if (ddlC1 == null)
                                    {
                                        continue;
                                    }

                                    //如果文本库数值为空，就让其返回.
                                    string txt = tbAuto.Text.Trim();
                                    if (string.IsNullOrEmpty(txt))
                                        continue;

                                    //获取要填充 ddll 的SQL.
                                    string sql = myCtl[1].Replace("~", "'");
                                    sql = sql.Replace("@Key", txt);
                                    sql = BP.WF.Glo.DealExp(sql, en, null);

                                    try
                                    {
                                        dt = DBAccess.RunSQLReturnTable(sql);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Clear();
                                        this.AddFieldSet("配置错误");
                                        this.Add(me.ToStringAtParas() + "<hr>错误信息:<br>" + ex.Message);
                                        this.AddFieldSetEnd();
                                        return;
                                    }

                                    if (dt.Rows.Count != 0)
                                    {
                                        string valC1 = ddlC1.SelectedItemStringVal;
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            ListItem li = ddlC1.Items.FindByValue(dr[0].ToString());
                                            if (li == null)
                                            {
                                                ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                            }
                                            else
                                            {
                                                li.Attributes["enable"] = "false";
                                                li.Attributes["display"] = "false";

                                            }
                                        }
                                        ddlC1.SetSelectItem(valC1);
                                    }
                                }
                            }
                            break;
                        case MapExtXmlList.InputCheck:
                            TextBox tbJS = this.GetTextBoxByID("TB_" + me.AttrOfOper);
                            if (tbJS != null)
                            {
                                tbJS.Attributes[me.Tag2] = me.Tag1 + "(this);";
                            }
                            else
                            {
                                DDL ddl = this.GetDDLByID("DDL_" + me.AttrOfOper);
                                if (ddl != null)
                                    ddl.Attributes[me.Tag2] = me.Tag1 + "(this);";
                            }
                            break;
                        case MapExtXmlList.PopVal: // 弹出窗.
                            TB tb = this.GetTBByID("TB_" + me.AttrOfOper);
                            if (tb == null)
                                continue;

                            //移除常用词汇事件
                            if (tb.Rows > 1)
                                tb.Attributes.Remove("ondblclick");

                            if (tb.CssClass != "TBReadonly")
                            {
                                if (me.PopValWorkModel == 0)
                                {
                                    tb.Attributes["onclick"] = "ShowHelpDiv('" + tb.ID + "','" + BP.WF.Glo.DealExp(me.Doc, en, null) + "','','','returnval');";
                                    tb.Attributes["ondblclick"] = "ReturnVal(this,'" + BP.WF.Glo.DealExp(me.Doc, en, null) + "','sd');";
                                }
                                else
                                {
                                    tb.Attributes["onclick"] = "ShowHelpDiv('" + tb.ID + "','','" + me.MyPK + "','" + en.PKVal + "','returnvalccformpopval');";
                                    tb.Attributes["ondblclick"] = "ReturnValCCFormPopVal(this,'" + me.MyPK + "','" + en.PKVal + "');";
                                }

                                //tb.Attributes["onkeyup"] = "this.value='';";    //added by liuxc,2015.6.26,应新昌增加不允许修改
                            }
                            break;
                        case MapExtXmlList.RegularExpression://正则表达式,对数据控件处理
                            WebControl tbExp = this.GetTBByID("TB_" + me.AttrOfOper);

                            if (tbExp == null)
                                tbExp = this.GetCBByID("CB_" + me.AttrOfOper);

                            if (tbExp == null)
                                tbExp = this.GetDDLByID("DDL_" + me.AttrOfOper);

                            if (tbExp == null || me.Tag == "onsubmit")
                                continue;

                            //验证输入的正则格式
                            string regFilter = me.Doc;
                            if (regFilter.LastIndexOf("/g") < 0 && regFilter.LastIndexOf('/') < 0)
                                regFilter = "'" + regFilter + "'";
                            //处理事件
                            if (me.Tag == "onkeyup" || me.Tag == "onkeypress")
                            {
                                tbExp.Attributes.Add("" + me.Tag + "", "return txtTest_Onkeyup(this," + regFilter + ",'" + me.Tag1 + "')");//[me.Tag] += "this.value=this.value.replace(" + regFilter + ",'')";
                                //tbExp.Attributes[me.Tag] += "value=value.replace(" + regFilter + ",'')";
                            }
                            else if (me.Tag == "onclick")
                            {
                                tbExp.Attributes[me.Tag] += me.Doc;
                            }
                            else
                            {
                                tbExp.Attributes[me.Tag] += "EleInputCheck2(this," + regFilter + ",'" + me.Tag1 + "');";
                            }
                            break;
                        default:
                            break;
                    }
                }
                #endregion 在处理其它。




            }

            #region 保存时处理正则表达式验证
            string scriptCheckFrm = "";
            scriptCheckFrm = "\t\n<script type='text/javascript' >";
            scriptCheckFrm += "\t\n function SysCheckFrm(){ ";
            scriptCheckFrm += "\t\n var isPass = true;";
            scriptCheckFrm += "\t\n var alloweSave = true;";
            scriptCheckFrm += "\t\n var erroMsg = '提示信息:';";
            foreach (MapExt me in mes)
            {
                if (me.ExtType == MapExtXmlList.RegularExpression && me.Tag == "onsubmit")
                {
                    TB tb = this.GetTBByID("TB_" + me.AttrOfOper);
                    if (tb == null)
                        continue;
                    scriptCheckFrm += "\t\n try{  ";
                    scriptCheckFrm += "\t\n  var element = document.getElementById('" + tb.ClientID + "');";
                    //验证输入的正则格式
                    string regFilter = me.Doc;
                    if (regFilter.LastIndexOf("/g") < 0 && regFilter.LastIndexOf('/') < 0)
                        regFilter = "'" + regFilter + "'";

                    //scriptCheckFrm += "\t\n  debugger ";
                    scriptCheckFrm += "\t\n isPass = EleSubmitCheck(element," + regFilter + ",'" + me.Tag1 + "');";
                    //scriptCheckFrm += "\t\n var reg =new RegExp(" + regFilter + ");   isPass = reg.test(element.value); ";
                    scriptCheckFrm += "\t\n  if(isPass == false){";
                    scriptCheckFrm += "\t\n   //EleSubmitCheck(element," + regFilter + ",'" + me.Tag1 + "'); alloweSave = false;";
                    scriptCheckFrm += "\t\n   alloweSave = false;";
                    scriptCheckFrm += "\t\n    erroMsg += '" + me.Tag1 + ";';";
                    scriptCheckFrm += "\t\n  }";
                    scriptCheckFrm += "\t\n } catch(e) { ";
                    scriptCheckFrm += "\t\n  alert('" + me.AttrOfOper + "'+e.name +':'+ e.message);  return false;";
                    scriptCheckFrm += "\t\n } ";
                }
            }
            scriptCheckFrm += "\t\n if(alloweSave == false){";
            scriptCheckFrm += "\t\n     alert(erroMsg);";
            scriptCheckFrm += "\t\n  } ";
            scriptCheckFrm += "\t\n return alloweSave; } ";

            //打开表单后的必填提示
            scriptCheckFrm += "\t\n function FormOnLoadCheckIsNull(){ ";
            foreach (MapExt me in mes)
            {
                if (me.ExtType == MapExtXmlList.RegularExpression)
                {
                    if (me.Tag != "onsubmit")
                        continue;

                    TB tb = this.GetTBByID("TB_" + me.AttrOfOper);
                    if (tb == null)
                        continue;
                    //验证输入的正则格式
                    string regFilter = me.Doc;
                    if (regFilter.LastIndexOf("/g") < 0 && regFilter.LastIndexOf('/') < 0)
                        regFilter = "'" + regFilter + "'";

                    scriptCheckFrm += "\t\n try{  ";
                    scriptCheckFrm += "\t\n var element = document.getElementById('" + tb.ClientID + "');";
                    //验证输入的正则格式
                    scriptCheckFrm += "\t\n if(element && element.readOnly == true) return;";
                    scriptCheckFrm += "\t\n   EleSubmitCheck(element," + regFilter + ",'" + me.Tag1 + "');";
                    scriptCheckFrm += "\t\n } catch(e) { ";
                    scriptCheckFrm += "\t\n } ";
                }
            }
            scriptCheckFrm += "\t\n } ";

            scriptCheckFrm += "\t\n</script>";
            this.Add(scriptCheckFrm);
            #endregion

            #endregion 处理扩展设置

            #region 处理自动计算
            string js = "\t\n <script type='text/javascript' >oid=" + en.PKVal + ";</script>";
            this.Add(js);
            foreach (MapExt ext in mes)
            {
                if (ext.Tag != "1")
                    continue;

                js = "\t\n <script type='text/javascript' >";
                TB tb = null;
                try
                {
                    tb = this.GetTBByID("TB_" + ext.AttrOfOper);
                    if (tb == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                string left = "\n  document.forms[0]." + tb.ClientID + ".value = ";
                string right = ext.Doc;

                Paras ps = new Paras();
                ps.SQL = "SELECT KeyOfEn,Name FROM Sys_MapAttr WHERE FK_MapData=" + ps.DBStr + "FK_MapData AND LGType=0 AND (MyDataType=2 OR MyDataType=3 OR MyDataType=5 OR MyDataType=8 OR MyDataType=9) ORDER BY KeyOfEn DESC";
                ps.Add("FK_MapData", enName);

                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                foreach (DataRow dr in dt.Rows)
                {
                    string keyofen = dr[0].ToString();
                    string name = dr[1].ToString();

                    if (ext.Doc.Contains("@" + keyofen)
                        || (ext.Doc.Contains("@" + name) && !string.IsNullOrEmpty(name)))
                    {
                    }
                    else
                    {
                        continue;
                    }

                    string tbID = "TB_" + keyofen;
                    TB mytb = this.GetTBByID(tbID);
                    this.GetTBByID(tbID).Attributes["onkeyup"] += "javascript:Auto" + ext.AttrOfOper + "();";

                    right = right.Replace("@" + keyofen, " parseFloat( document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' )=='' ? 0 : document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' )) ");
                    if (!string.IsNullOrEmpty(name))
                        right = right.Replace("@" + name, " parseFloat( document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' )=='' ? 0 : document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' )) ");
                }

                int myDataType = BP.DA.DataType.AppMoney;

                //判断类型
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.KeyOfEn == ext.AttrOfOper)
                    {
                        myDataType = attr.MyDataType;
                    }
                }

                js += "\t\n function Auto" + ext.AttrOfOper + "() { ";
                js += left + right + ";";
                if (myDataType == BP.DA.DataType.AppFloat || myDataType == BP.DA.DataType.AppDouble)
                {
                    js += " \t\n  document.forms[0]." + tb.ClientID + ".value= VirtyMoney(document.forms[0]." + tb.ClientID + ".value ) ;";

                }
                else
                {
                    js += " \t\n  document.forms[0]." + tb.ClientID + ".value= document.forms[0]." + tb.ClientID + ".value;";
                }
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js); //加入里面.
            }
            #endregion

        }
        public void InsertObjects(bool isJudgeRowIdx)
        {
            #region 从表
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false || this.ctrlUseSta.Contains(dtl.No))
                    continue;

                if (dtl.GroupID == 0)
                {
                    dtl.GroupID = currGF.OID;
                    dtl.RowIdx = 0;
                    dtl.Update();
                }

                if (isJudgeRowIdx)
                {
                    if (dtl.RowIdx != rowIdx)
                        continue;
                }

                if (dtl.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }

                // dtl.IsUse = true;

                this.ctrlUseSta += dtl.No;

                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + dtl.No + "' height='50px' width='100%' style='align:left'>");
                string src = "";
                try
                {
                    src = CCFlowAppPath + "WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&FID=" + this.HisEn.GetValStringByKey("FID") + "&IsWap=0&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }
                catch
                {
                    src = CCFlowAppPath + "WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&IsWap=0&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }

                if (this.IsReadonly || dtl.IsReadonly)
                    this.Add("<iframe ID='F" + dtl.No + "'  src='" + src +
                             "&IsReadonly=1' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%'  /></iframe>");
                else
                {
                    //this.Add("<iframe ID='F" + dtl.No + "'   Onblur=\"SaveDtl('" + dtl.No + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' /></iframe>");

                    AddLoadFunction(dtl.No, "blur", "SaveDtl");

                    this.Add("<iframe ID='F" + dtl.No + "'   onload='" + dtl.No + "load();'  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%'  /></iframe>");

                }

                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 从表

            #region 多对多的关系
            foreach (MapM2M m2m in m2ms)
            {
                if (this.ctrlUseSta.Contains("@" + m2m.MyPK))
                    continue;

                if (isJudgeRowIdx)
                {
                    if (m2m.RowIdx != rowIdx)
                        continue;
                }

                if (m2m.GroupID == 0 && rowIdx == 0)
                {
                    m2m.GroupID = currGF.OID;
                    m2m.RowIdx = 0;
                    m2m.Update();
                }
                else if (m2m.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }

                this.ctrlUseSta += "@" + m2m.MyPK;


                rowIdx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");

                string src = CCFlowAppPath + "WF/CCForm/M2M.aspx?NoOfObj=" + m2m.NoOfObj;
                string paras = this.RequestParas;
                if (paras.Contains("FID=") == false)
                    paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");

                src += "&r=q" + paras;
                if (src.Contains("FK_MapData") == false)
                    src += "&FK_MapData=" + m2m.FK_MapData;
                switch (m2m.ShowWay)
                {
                    case FrmShowWay.FrmAutoSize:
                        this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + m2m.NoOfObj + "' height='20px' width='100%'  >");
                        if (m2m.HisM2MType == M2MType.M2M)
                        {

                            AddLoadFunction(m2m.NoOfObj, "blur", "SaveM2M");


                            //  this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'  onload='" + m2m.NoOfObj + "load();'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");

                        }
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "' src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                        break;
                    case FrmShowWay.FrmSpecSize:
                        this.Add("<TD colspan=" + this.mapData.TableCol + "  ID='TD" + m2m.NoOfObj + "' height='" + m2m.H + "' width='" + m2m.W + "'  >");
                        if (m2m.HisM2MType == M2MType.M2M)
                        {
                            AddLoadFunction(m2m.NoOfObj, "blur", "SaveM2M");

                            // this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "' onload='" + m2m.NoOfObj + "load();'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");

                        }
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'    src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                        break;
                    case FrmShowWay.Hidden:
                        break;
                    case FrmShowWay.WinOpen:
                        this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + m2m.NoOfObj + "' height='20px' width='100%'  >");
                        this.Add("<a href=\"javascript:WinOpen('" + src + "&IsOpen=1','" + m2m.W + "','" + m2m.H + "');\"  />" + m2m.Name + "</a>");
                        break;
                    default:
                        break;
                }
            }
            #endregion 多对多的关系

            #region 框架
            foreach (MapFrame fram in frames)
            {
                if (this.ctrlUseSta.Contains("@" + fram.MyPK))
                    continue;

                if (isJudgeRowIdx)
                {
                    if (fram.RowIdx != rowIdx)
                        continue;
                }

                if (fram.GroupID == 0 && rowIdx == 0)
                {
                    fram.GroupID = currGF.OID;
                    fram.RowIdx = 0;
                    fram.Update();
                }
                else if (fram.GroupID == currGF.OID)
                {
                }
                else
                {
                    continue;
                }

                this.ctrlUseSta += "@" + fram.MyPK;
                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                if (fram.IsAutoSize)
                    this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + fram.NoOfObj + "' height='50px' width='100%'  >");
                else
                    this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + fram.NoOfObj + "' height='" + fram.H + "' width='" + fram.W + "'  >");

                string paras = this.RequestParas;
                if (paras.Contains("FID=") == false)
                    paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                if (paras.Contains("WorkID=") == false)
                    paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");

                string src = fram.URL;
                if (src.Contains("?"))
                    src += "&r=q" + paras;
                else
                    src += "?r=q" + paras;

                if (fram.IsAutoSize)
                {
                    this.Add("<iframe ID='F" + fram.NoOfObj + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=auto /></iframe>");
                }
                else
                {
                    this.Add("<iframe ID='F" + fram.NoOfObj + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + fram.W + "' height='" + fram.H + "' scrolling=auto /></iframe>");
                }

                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 框架

            #region 附件
            foreach (BP.Sys.FrmAttachment ath in aths)
            {
                if (this.ctrlUseSta.Contains("@" + ath.MyPK))
                    continue;
                if (isJudgeRowIdx)
                {
                    if (ath.RowIdx != rowIdx)
                        continue;
                }

                if (ath.GroupID == 0 && rowIdx == 0)
                {
                    ath.GroupID = currGF.OID;
                    ath.RowIdx = 0;
                    ath.Update();
                }
                else if (ath.GroupID == currGF.OID)
                {
                }
                else
                {
                    continue;
                }
                this.ctrlUseSta += "@" + ath.MyPK;
                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=" + this.mapData.TableCol + " ID='TD" + ath.MyPK + "' height='50px' width='100%' style='align:left'>");
                string src = "";
                if (this.IsReadonly)
                    src = CCFlowAppPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + EnName + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1" + this.RequestParas;
                else
                    src = CCFlowAppPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + EnName + "&FK_FrmAttachment=" + ath.MyPK + this.RequestParas;

                if (ath.IsAutoSize)
                {
                    this.Add("<iframe ID='F" + ath.MyPK + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=auto /></iframe>");
                }
                else
                {
                    this.Add("<iframe ID='F" + ath.MyPK + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ath.W + "' height='" + ath.H + "' scrolling=auto /></iframe>");
                }
                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 附件
        }
        public void AddRichTextBox(Entity en, MapAttr attr)
        {
            /*说明这是富文本输出*/
            this.Page.RegisterClientScriptBlock("c51",
     "<script language='JavaScript' src='" + CCFlowAppPath + "WF/Comm/kindeditor/kindeditor-all.js'  charset='utf-8' ></script>");

            this.Page.RegisterClientScriptBlock("c2",
    "<script language='JavaScript' src='" + CCFlowAppPath + "WF/Comm/kindeditor/lang/zh_CN.js'  charset='utf-8' ></script>");

            this.Page.RegisterClientScriptBlock("c53",
  "<script language='JavaScript' src='" + CCFlowAppPath + "WF/Comm/kindeditor/plugins/code/prettify.js'  charset='utf-8' ></script>");


            this.Page.RegisterClientScriptBlock("s51",
    "<link href='" + CCFlowAppPath + "WF/Comm/kindeditor/plugins/code/prettify.css' rel='stylesheet' type='text/css' />");

            this.Page.RegisterClientScriptBlock("s52",
    "<link href='" + CCFlowAppPath + "WF/Comm/kindeditor/themes/default/default.css' rel='stylesheet' type='text/css' />");

            string strs = "\t\n <script>";
            strs += "\t\n  var editor1; ";
            strs += "\t\n KindEditor.ready(function(K) {";
            strs += "\t\n var fID='TB_" + attr.KeyOfEn + "'; ";
            strs += "\t\n var tbID='ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_'+fID;";

            strs += "\t\n var ctrl =document.getElementById( tbID);";
            strs += "\t\n if (ctrl==null)";
            strs += "\t\n tbID = 'ContentPlaceHolder1_UCEn1_' + fID;";

            strs += "\t\n ctrl =document.getElementById( tbID);";
            strs += "\t\n if (ctrl == null) { ";
            strs += "\t\n tbID = 'ContentPlaceHolder1_WFRpt1_UCEn1_' + fID;";
            strs += "\t\n ctrl =document.getElementById( tbID);";
            strs += "\t\n  } ";

            strs += "\t\n if (ctrl == null) { ";
            strs += "\t\n tbID = 'ContentPlaceHolder1_FHLFlow1_UCEn1_' + fID;";
            strs += "\t\n ctrl =document.getElementById( tbID);";
            strs += "\t\n  } ";


            strs += "\t\n if (ctrl == null) { ";
            strs += "\t\n     alert('没有找到要帮定的控件'); ";
            strs += "\t\n  } ";

            strs += "\t\n   editor1 = K.create('#'+tbID, {";
            strs += "\t\n cssPath : '" + CCFlowAppPath + "WF/Comm/kindeditor/plugins/code/prettify.css',";
            strs += "\t\n uploadJson : '" + CCFlowAppPath + "WF/Comm/kindeditor/asp.net/upload_json.ashx',";
            strs += "\t\n fileManagerJson : '" + CCFlowAppPath + "WF/Comm/kindeditor/asp.net/file_manager_json.ashx',";
            strs += "\t\n allowFileManager : true,";

            strs += "\t\n width : '100%',";
            //strs += "\t\n width : '" + attr.UIWidth + "px',";

            strs += "\t\n height : '" + attr.UIHeight + "px'";

            strs += "\t\n });";
            strs += "\t\n });";

            //strs += "\t\n KindEditor.show(function(K) {";
            //strs += "\t\n KindEditor.ready(function(K) {";

            strs += "\t\n </script>";
            this.Add(strs);

            TB tbd = new TB();
            tbd.TextMode = TextBoxMode.MultiLine;
            tbd.ID = "TB_" + attr.KeyOfEn;
            tbd.Text = en.GetValStrByKey(attr.KeyOfEn);
            tbd.TextMode = TextBoxMode.MultiLine;
            tbd.Attributes["style"] = "width:" + attr.UIWidth + "px;height:" + attr.UIHeight + "px;visibility:hidden;";
            this.Add(tbd);

        }
        #endregion

        #region 输出自由格式的表单.
        public string FK_MapData = null;
        FrmEvents fes = null;
        public new string EnName = null;
        public string LinkFields = "";
        public MapData mapData = null;
        /// <summary>
        /// 处理它的默认值.
        /// </summary>
        /// <param name="mattrs"></param>
        private void DealDefVal(MapAttrs mattrs)
        {
            if (this.IsReadonly)
                return;
            this.Page.RegisterClientScriptBlock("y7",
          "<script language='JavaScript' src='" + CCFlowAppPath + "DataUser/JSLibData/" + this.EnName + "_Self.js' charset='gb2312'></script>");

            this.Page.RegisterClientScriptBlock("yfd7",
          "<script language='JavaScript' src='" + CCFlowAppPath + "DataUser/JSLibData/" + this.EnName + ".js' charset='gb2312'></script>");
            foreach (MapAttr attr in mattrs)
            {
                if (attr.DefValReal.Contains("@") == false)
                    continue;

                this.HisEn.SetValByKey(attr.KeyOfEn, attr.DefVal);
            }
        }
        /// <summary>
        /// 绑定H5手机模式.
        /// </summary>
        /// <param name="mattr"></param>
        /// <param name="en"></param>
        public void BindHtml5Model(MapAttrs mattrs, Entity en)
        {
            this.Add("<table width=100% >");
            foreach (BP.Sys.GroupField gf in gfs)
            {
                currGF = gf;
                this.AddTR();
                this.AddTD("colspan=2 class=GroupField valign='top' align=left ", "<div style='text-align:left; float:left'>&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                this.AddTREnd();

                int idx = -1;
                isLeftNext = true;
                rowIdx = 0;

                #region 增加字段.
                foreach (MapAttr attr in mattrs)
                {
                    #region 排除
                    if (attr.GroupID != gf.OID)
                    {
                        if (gf.Idx == 0 && attr.GroupID == 0)
                        {
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                        continue;
                    #endregion 排除

                    #region 设置
                    rowIdx++;
                    this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "'");
                    if (attr.UIIsEnable == false)
                    {
                        if (this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                        {
                            MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                            string url = meLink.Tag;
                            if (url.Contains("?") == false)
                                url = url + "?a3=2";
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + this.EnName;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + CCFlowAppPath);
                            if (url.Contains("@"))
                            {
                                Attrs attrs = en.EnMap.Attrs;
                                foreach (Attr item in attrs)
                                {
                                    url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                            }
                            this.AddTD("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                            this.AddTREnd();
                            continue;
                        }
                    }
                    #endregion 设置

                    #region 加入字段
                    // 显示的顺序号.
                    idx++;
                    if (attr.IsBigDoc && attr.UIIsLine)
                    {
                        if (attr.UIIsEnable)
                            this.Add("<TD colspan=2 height='" + attr.UIHeight.ToString() + "px'    width='100%' valign=top align=left>" + attr.Name + "<br>");
                        else
                            this.Add("<TD colspan=2 height='" + attr.UIHeight.ToString() + "px'   width='100%' valign=top class=TBReadonly>" + attr.Name + "<br>");

                        TB mytbLine = new TB();
                        if (attr.IsBigDoc)
                        {
                            mytbLine.TextMode = TextBoxMode.MultiLine;
                            mytbLine.Attributes["class"] = "TBDoc";
                        }

                        mytbLine.ID = "TB_" + attr.KeyOfEn;
                        if (attr.IsBigDoc)
                        {
                            //  mytbLine = 5;
                            // mytbLine.Columns = 30;
                        }

                        mytbLine.Attributes["style"] = "width:98%;height:100%;padding: 0px;margin: 0px;";
                        mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn);
                        mytbLine.Enabled = attr.UIIsEnable;

                        this.Add(mytbLine);
                        this.AddTDEnd();
                        this.AddTREnd();
                        rowIdx++;
                        continue;
                    }

                    TB tb = new TB();
                    tb.Attributes["width"] = "100%";
                    tb.Attributes["border"] = "1px";
                    tb.Columns = 40;
                    tb.ID = "TB_" + attr.KeyOfEn;
                    Control ctl = tb;

                    #region add contrals.
                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            tb.Enabled = attr.UIIsEnable;
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                    tb.ShowType = TBType.TB;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    break;
                                case BP.DA.DataType.AppDate:
                                    tb.ShowType = TBType.Date;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker();";
                                    break;
                                case BP.DA.DataType.AppDateTime:
                                    tb.ShowType = TBType.DateTime;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    CheckBox cb = new CheckBox();
                                    cb.Text = attr.Name;
                                    cb.ID = "CB_" + attr.KeyOfEn;
                                    cb.Checked = attr.DefValOfBool;
                                    cb.Enabled = attr.UIIsEnable;
                                    cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                    this.AddTD("colspan=2", cb);
                                    continue;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppFloat:
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    break;
                                case BP.DA.DataType.AppInt:
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d]/g,'')");
                                    break;
                                case BP.DA.DataType.AppMoney:
                                case BP.DA.DataType.AppRate:
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = decimal.Parse(en.GetValStrByKey(attr.KeyOfEn)).ToString("0.00");
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                    break;
                                default:
                                    break;
                            }
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
                            DDL ddle = new DDL();
                            ddle.ID = "DDL_" + attr.KeyOfEn;
                            ddle.BindSysEnum(attr.UIBindKey);
                            ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            ddle.Enabled = attr.UIIsEnable;
                            ctl = ddle;
                            break;
                        case FieldTypeS.FK:
                            DDL ddl1 = new DDL();
                            ddl1.ID = "DDL_" + attr.KeyOfEn;
                            try
                            {
                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                ens.RetrieveAll();
                                ddl1.BindEntities(ens);
                                ddl1.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            }
                            catch
                            {

                            }
                            ddl1.Enabled = attr.UIIsEnable;
                            ctl = ddl1;
                            break;
                        default:
                            break;
                    }
                    #endregion add contrals.

                    string desc = attr.Name.Replace("：", "");
                    desc = desc.Replace(":", "");
                    desc = desc.Replace(" ", "");

                    if (desc.Length >= 5)
                    {
                        this.Add("<TD colspan=2 class=FDesc width='100%' ><div style='float:left'>" + desc + "</div><br>");
                        this.Add(ctl);
                        this.AddTREnd();
                    }
                    else
                    {
                        this.AddTDDesc(desc);
                        this.AddTD("width='100%' class=TBReadonly", ctl);
                        this.AddTREnd();
                    }
                    #endregion 加入字段

                }
                #endregion 增加字段.

                // 插入col.
                string fid = "0";
                try
                {
                    fid = en.GetValStrByKey("FID");
                }
                catch
                {
                }
                this.InsertObjects2Col(true, en.PKVal.ToString(), fid);
            }
            this.AddTableEnd();


            #region 处理iFrom 的自适应的问题。
            string js = "\t\n<script type='text/javascript' >";
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                js += "\t\n window.setInterval(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
            }
            foreach (MapFrame fr in frames)
            {
                //  if (fr.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + fr.NoOfObj + "','TD" + fr.NoOfObj + "')\", 200);";
            }
            foreach (MapM2M m2m in m2ms)
            {
                //  if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + m2m.NoOfObj + "','TD" + m2m.NoOfObj + "')\", 200);";
            }
            foreach (FrmAttachment ath in aths)
            {
                // if (ath.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + ath.MyPK + "','TD" + ath.MyPK + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom 的自适应的问题。

            // 处理扩展。
            if (this.IsReadonly == false)
            {
                #region 处理iFrom SaveDtlData。
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveDtl(dtl) { ";
                //    js += "\t\n    GenerPageKVs(); //调用产生kvs ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveDtlData(); ";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom SaveDtlData。

                #region 处理iFrom  SaveM2M Save
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveM2M(dtl) { ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom  SaveM2M Save。
            }
        }
        public void BindCCForm(Entity en, string enName, bool isReadonly, float srcWidth, bool IsEnableLoadData)
        {
            MapData md = new MapData(enName);
            BindCCForm(en, md, md.MapAttrs, enName, isReadonly, srcWidth, IsEnableLoadData);
        }
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="en"></param>
        /// <param name="md"></param>
        /// <param name="mattrs"></param>
        /// <param name="enName"></param>
        /// <param name="isReadonly"></param>
        /// <param name="srcWidth"></param>
        /// <param name="IsEnableLoadData">是否启用装载填充?在独立表单中，
        /// 如果是节点选用了，自定义方案，就不能启用装载填充。 </param>
        public void BindCCForm(Entity en, MapData md, MapAttrs mattrs, string enName, bool isReadonly, float srcWidth, bool IsEnableLoadData)
        {
            this.ctrlUseSta = "";

            this.EnName = enName;
            this.mapData = md;
            string appPath = BP.WF.Glo.CCFlowAppPath; //this.Request.ApplicationPath;

            //根据宽度计算出来微调.
            float wtX = MapData.GenerSpanWeiYi(md, srcWidth);
            float x = 0;

            mes = this.mapData.MapExts;
            this.IsReadonly = isReadonly;
            this.FK_MapData = enName;
            this.HisEn = en;
            this.EnName = enName;
            this.m2ms = this.mapData.MapM2Ms;
            this.dtls = this.mapData.MapDtls;
            this.mes = this.mapData.MapExts;

            //是否加载CA签名 dll.
            bool IsAddCa = false;

            #region 处理事件.
            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadBefore, en);
                    if (msg == "OK")
                    {
                        en.RetrieveFromDBSources();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(msg) == false)
                        {
                            en.RetrieveFromDBSources();
                            this.Alert(msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            //处理默认值.
            this.DealDefVal(mattrs);

            //处理装载前填充.
            if (IsEnableLoadData == true)
                this.LoadData(mattrs, en);

            #region 判断是否是手机.
            if (BP.Web.WebUser.UserWorkDev == UserWorkDev.Mobile)
            {
                /*判断是否是手机*/
                this.BindHtml5Model(mattrs, en);
                // 处理扩展.
                if (isReadonly == false)
                    this.AfterBindEn_DealMapExt(enName, mattrs, en);
                return;
            }
            #endregion 判断是否是手机.

            //获得活动的控件.
            string activeFilds = BP.WF.Glo.GenerActiveFiels(mes, null, en, md, mattrs);

            #region 输出Ele
            FrmEles eles = this.mapData.FrmEles;
            if (eles.Count >= 1)
            {
                string myjs = "\t\n<script type='text/javascript' >";
                myjs += "\t\n function BPPaint(ctrl,url,w,h,fk_FrmEle)";
                myjs += "\t\n {";
                myjs += "\t\n  var v= window.showModalDialog(url, 'ddf', 'dialogHeight: '+h+'px; dialogWidth: '+w+'px;center: yes; help: no'); ";
                myjs += "\t\n  if (v==null )  ";
                myjs += "\t\n     return ; ";
                myjs += "\t\n  ctrl.src=v+'?temp='+new Date(); ";
                myjs += "\t\n }";
                myjs += "\t\n</script>";
                this.Add(myjs);

                FrmEleDBs dbs = new FrmEleDBs(this.FK_MapData, en.PKVal.ToString());
                foreach (FrmEle ele in eles)
                {
                    float y = ele.Y;
                    x = ele.X + wtX;
                    this.Add("\t\n<DIV id=" + ele.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    switch (ele.EleType)
                    {
                        case FrmEle.HandSiganture:
                            FrmEleDB db = dbs.GetEntityByKey(FrmEleDBAttr.EleID, ele.EleID) as FrmEleDB;
                            string dbFile = appPath + "DataUser/BPPaint/Def.png";
                            if (db != null)
                                dbFile = db.Tag1;

                            if (this.IsReadonly || ele.IsEnable == false)
                            {
                                this.Add("\t\n<img src='" + dbFile + "' onerror=\"this.src='" + appPath + "DataUser/BPPaint/Def.png'\" style='padding: 0px;margin: 0px;border-width: 0px;width:" + ele.W + "px;height:" + ele.H + "px;' />");
                            }
                            else
                            {
                                string url = appPath + "WF/CCForm/BPPaint.aspx?W=" + ele.HandSiganture_WinOpenW + "&H=" + ele.HandSiganture_WinOpenH + "&MyPK=" + ele.PKVal + "&PKVal=" + en.PKVal;
                                myjs = "javascript:BPPaint(this,'" + url + "','" + ele.HandSiganture_WinOpenW + "','" + ele.HandSiganture_WinOpenH + "','" + ele.MyPK + "');";
                                //string myjs = "javascript:window.open('" + appPath + "WF/CCForm/BPPaint.aspx?PKVal=" + en.PKVal + "&MyPK=" + ele.MyPK + "&H=" + ele.HandSiganture_WinOpenH + "&W=" + ele.HandSiganture_WinOpenW + "', 'sdf', 'dialogHeight: " + ele.HandSiganture_WinOpenH + "px; dialogWidth: " + ele.HandSiganture_WinOpenW + "px;center: yes; help: no');";
                                this.Add("\t\n<img id='Ele" + ele.MyPK + "' onclick=\"" + myjs + "\" onerror=\"this.src='" + appPath + "DataUser/BPPaint/Def.png'\" src='" + dbFile + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + ele.W + "px;height:" + ele.H + "px;' />");
                            }
                            break;
                        case FrmEle.iFrame: //输出框架.
                            string paras = this.RequestParas;
                            if (paras.Contains("FID=") == false && this.HisEn.Row.ContainsKey("FID"))
                            {
                                paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                            }

                            if (paras.Contains("WorkID=") == false && this.HisEn.Row.ContainsKey("OID"))
                            {
                                paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");
                            }

                            string src = ele.Tag1.Clone() as string; // url 
                            if (src.Contains("?"))
                                src += "&r=q" + paras;
                            else
                                src += "?r=q" + paras;

                            if (src.Contains("UserNo") == false)
                                src += "&UserNo=" + WebUser.No;
                            if (src.Contains("SID") == false)
                                src += "&SID=" + WebUser.SID;
                            if (src.Contains("@"))
                            {
                                foreach (Attr m in en.EnMap.Attrs)
                                {
                                    if (src.Contains("@") == false)
                                        break;
                                    src = src.Replace("@" + m.Key, en.GetValStrByKey(m.Key));
                                }
                            }

                            if (this.IsReadonly == true)
                            {
                                this.Add("<iframe ID='F" + ele.EleID + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ele.W + "' height='" + ele.H + "' scrolling=auto /></iframe>");
                            }
                            else
                            {
                                AddLoadFunction(ele.EleID, "blur", "SaveDtl");
                                this.Add("<iframe ID='F" + ele.EleID + "' onload= '" + ele.EleID + "load();'  src='" + src + "' frameborder=0  style='position:absolute;width:" + ele.W + "px; height:" + ele.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                            }
                            break;
                        case FrmEle.EleSiganture:
                            this.Add("未处理");
                            break;
                        case FrmEle.SubThread: //子线程.
                            paras = this.RequestParas;
                            if (paras.Contains("FID=") == false && this.HisEn.Row.ContainsKey("FID"))
                            {
                                paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                            }

                            if (paras.Contains("WorkID=") == false && this.HisEn.Row.ContainsKey("OID"))
                            {
                                paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");
                            }

                            src = "/WF/WorkOpt/ThreadDtl.aspx?1=2" + paras;
                            if (src.Contains("UserNo") == false)
                                src += "&UserNo=" + WebUser.No;
                            if (src.Contains("SID") == false)
                                src += "&SID=" + WebUser.SID;
                            if (src.Contains("@"))
                            {
                                foreach (Attr m in en.EnMap.Attrs)
                                {
                                    if (src.Contains("@") == false)
                                        break;
                                    src = src.Replace("@" + m.Key, en.GetValStrByKey(m.Key));
                                }
                            }
                            this.Add("<iframe ID='F" + ele.EleID + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ele.W + "' height='" + ele.H + "' scrolling=auto /></iframe>");
                            break;
                        default:
                            break;
                    }
                    this.Add("\t\n</DIV>");
                }
            }
            #endregion 输出Ele

            #region 输出按钮
            FrmBtns btns = this.mapData.FrmBtns;
            foreach (FrmBtn btn in btns)
            {
                x = btn.X + wtX;
                this.Add("\t\n<DIV id=u2 style='position:absolute;left:" + x + "px;top:" + btn.Y + "px;text-align:left;' >");
                this.Add("\t\n<span >");

                string doDoc = BP.WF.Glo.DealExp(btn.EventContext, en, null);
                switch (btn.HisBtnEventType)
                {
                    case BtnEventType.Disable:
                        this.Add("<input type=button class=Btn value='" + btn.Text.Replace("&nbsp;", " ") + "' disabled='disabled'/>");
                        break;
                    case BtnEventType.RunExe:
                    case BtnEventType.RunJS:
                        this.Add("<input type=button class=Btn value=\"" + btn.Text.Replace("&nbsp;", " ") + "\" enable=true onclick=\"" + doDoc + "\" />");
                        break;
                    default:
                        Button myBtn = new Button();
                        myBtn.Enabled = true;
                        myBtn.CssClass = "Btn";
                        myBtn.ID = btn.MyPK;
                        myBtn.Text = btn.Text.Replace("&nbsp;", " ");
                        myBtn.Click += new EventHandler(myBtn_Click);
                        this.Add(myBtn);
                        break;
                }
                this.Add("\t\n</span>");
                this.Add("\t\n</DIV>");
            }
            #endregion

            #region 输出竖线与标签 & 超连接 Img.
            FrmLabs labs = this.mapData.FrmLabs;
            foreach (FrmLab lab in labs)
            {
                Color col = ColorTranslator.FromHtml(lab.FontColor);
                x = lab.X + wtX;
                this.Add("\t\n<DIV id=u2 style='position:absolute;left:" + x + "px;top:" + lab.Y + "px;text-align:left;' >");
                this.Add("\t\n<span style='color:" + lab.FontColorHtml + ";font-family: " + lab.FontName + ";font-size: " + lab.FontSize + "px;' >" + lab.TextHtml + "</span>");
                this.Add("\t\n</DIV>");
            }

            FrmLines lines = this.mapData.FrmLines;
            foreach (FrmLine line in lines)
            {
                if (line.X1 == line.X2)
                {
                    /* 一道竖线 */
                    float h = line.Y1 - line.Y2;
                    h = Math.Abs(h);
                    if (line.Y1 < line.Y2)
                    {
                        x = line.X1 + wtX;
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y1 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                    else
                    {
                        x = line.X2 + wtX;
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y2 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                }
                else
                {
                    /* 一道横线 */
                    float w = line.X2 - line.X1;

                    if (line.X1 < line.X2)
                    {
                        x = line.X1 + wtX;
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y1 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                    else
                    {
                        x = line.X2 + wtX;
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + x + "px; top:" + line.Y2 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    }
                }
            }

            FrmLinks links = this.mapData.FrmLinks;
            foreach (FrmLink link in links)
            {
                string url = link.URL;
                if (url.Contains("@"))
                {
                    foreach (MapAttr attr in mattrs)
                    {
                        if (url.Contains("@") == false)
                            break;
                        url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                    }
                }
                x = link.X + wtX;
                this.Add("\t\n<DIV id=u2 style='position:absolute;left:" + x + "px;top:" + link.Y + "px;text-align:left;' >");
                this.Add("\t\n<span style='color:" + link.FontColorHtml + ";font-family: " + link.FontName + ";font-size: " + link.FontSize + "px;' > <a href=\"" + url + "\" target='" + link.Target + "'> " + link.Text + "</a></span>");
                this.Add("\t\n</DIV>");
            }

            FrmImgs imgs = this.mapData.FrmImgs;
            foreach (FrmImg img in imgs)
            {
                float y = img.Y;
                string imgSrc = "";
                //imgSrc = appPath + "DataUser/ICON/" + BP.Sys.SystemConfig.CustomerNo + "/LogBiger.png";
                //图片类型
                if (img.HisImgAppType == ImgAppType.Img)
                {
                    //数据来源为本地.
                    if (img.SrcType == 0)
                    {
                        if (img.ImgPath.Contains(";") == false)
                            imgSrc = img.ImgPath;
                    }

                    //数据来源为指定路径.
                    if (img.SrcType == 1)
                    {
                        //图片路径不为默认值
                        imgSrc = img.ImgURL;
                        if (imgSrc.Contains("@"))
                        {
                            /*如果有变量*/
                            imgSrc = BP.WF.Glo.DealExp(imgSrc, en, "");
                        }
                    }

                    x = img.X + wtX;
                    // 由于火狐 不支持onerror 所以 判断图片是否存在
                    if (imgSrc == "" || !File.Exists(Server.MapPath("~/" + imgSrc)))
                        imgSrc = "/DataUser/ICON/LogBig.png";

                    this.Add("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    if (string.IsNullOrEmpty(img.LinkURL) == false)
                        this.Add("\t\n<a href='" + img.LinkURL + "' target=" + img.LinkTarget + " ><img src='" + imgSrc + "'  onerror=\"this.src='/DataUser/ICON/CCFlow/LogBig.png'\"  style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' /></a>");
                    else
                        this.Add("\t\n<img src='" + imgSrc + "'  onerror=\"this.src='/DataUser/ICON/CCFlow/LogBig.png'\"  style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' />");
                    this.Add("\t\n</DIV>");
                    continue;
                }

                #region 电子签章
                //获取登录人岗位
                string stationNo = "";
                //签章对应部门
                string fk_dept = WebUser.FK_Dept;
                //部门来源类别
                string sealType = "0";
                //签章对应岗位
                string fk_station = img.Tag0;
                //表单字段
                string sealField = "";
                string sql = "";

                //重新加载 可能有缓存
                img.RetrieveFromDBSources();
                //0.不可以修改，从数据表中取，1可以修改，使用组合获取并保存数据
                if ((img.IsEdit == 1 && this.IsReadonly == false) || activeFilds.Contains(img.MyPK + ","))
                {
                    #region 加载签章
                    //如果设置了部门与岗位的集合进行拆分
                    if (!string.IsNullOrEmpty(img.Tag0) && img.Tag0.Contains("^") && img.Tag0.Split('^').Length == 4)
                    {
                        fk_dept = img.Tag0.Split('^')[0];
                        fk_station = img.Tag0.Split('^')[1];
                        sealType = img.Tag0.Split('^')[2];
                        sealField = img.Tag0.Split('^')[3];
                        //如果部门没有设定，就获取部门来源
                        if (fk_dept == "all")
                        {
                            //默认当前登陆人
                            //  fk_dept = WebUser.FK_Dept;
                            //发起人
                            if (sealType == "1")
                            {
                                sql = "SELECT FK_Dept FROM WF_GenerWorkFlow WHERE WorkID=" + this.HisEn.GetValStrByKey("OID");
                                fk_dept = BP.DA.DBAccess.RunSQLReturnString(sql);
                            }
                            //表单字段
                            if (sealType == "2" && !string.IsNullOrEmpty(sealField))
                            {
                                //判断字段是否存在
                                foreach (MapAttr attr in mattrs)
                                {
                                    if (attr.KeyOfEn == sealField)
                                    {
                                        fk_dept = this.HisEn.GetValStrByKey(sealField);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //判断本部门下是否有此人
                    //sql = "SELECT fk_station from port_deptEmpStation where fk_dept='" + fk_dept + "' and fk_emp='" + WebUser.No + "'";
                    sql = string.Format(" select FK_Station from Port_DeptStation where FK_Dept ='{0}' and FK_Station in (select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Emp='{1}')", fk_dept, WebUser.No);
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (fk_station.Contains(dr[0].ToString() + ","))
                        {
                            stationNo = dr[0].ToString();
                            break;
                        }
                    }
                    #endregion 加载签章

                    imgSrc = CCFlowAppPath + "DataUser/Seal/" + fk_dept + "_" + stationNo + ".png";
                    //设置主键
                    string myPK = string.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                    myPK = myPK + "_" + this.HisEn.GetValStrByKey("OID") + "_" + img.MyPK;

                    FrmEleDB imgDb = new FrmEleDB();
                    QueryObject queryInfo = new QueryObject(imgDb);
                    queryInfo.AddWhere(FrmEleAttr.MyPK, myPK);
                    queryInfo.DoQuery();
                    //判断是否存在
                    if (imgDb == null || string.IsNullOrEmpty(imgDb.FK_MapData))
                    {
                        imgDb.FK_MapData = string.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                        imgDb.EleID = this.HisEn.GetValStrByKey("OID");
                        imgDb.RefPKVal = img.MyPK;
                        imgDb.Tag1 = imgSrc;
                        imgDb.Insert();
                    }
                    else if (!IsPostBack) //edited by liuxc,2015-10-16,因新昌方面出现此处回发时将正确的签章图片路径改成了错误的路径，所以将此处的逻辑在回发时去掉，即回发时不进行签章图片的更新
                    {
                        imgDb.FK_MapData = string.IsNullOrEmpty(img.EnPK) ? "seal" : img.EnPK;
                        imgDb.EleID = this.HisEn.GetValStrByKey("OID");
                        imgDb.RefPKVal = img.MyPK;
                        imgDb.Tag1 = imgSrc;
                        imgDb.Update();
                    }
                    //添加控件
                    x = img.X + wtX;
                    this.Add("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    this.Add("\t\n<img src='" + imgSrc + "' onerror=\"javascript:this.src='" + appPath + "DataUser/Seal/Def.png'\" style=\"padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;\" />");
                    this.Add("\t\n</DIV>");
                }
                else
                {
                    FrmEleDB realDB = null;
                    FrmEleDB imgDb = new FrmEleDB();
                    QueryObject objQuery = new QueryObject(imgDb);
                    objQuery.AddWhere(FrmEleAttr.FK_MapData, img.EnPK);
                    objQuery.addAnd();
                    objQuery.AddWhere(FrmEleAttr.EleID, this.HisEn.GetValStrByKey("OID"));

                    if (objQuery.DoQuery() == 0)
                    {
                        FrmEleDBs imgdbs = new FrmEleDBs();
                        QueryObject objQuerys = new QueryObject(imgdbs);
                        objQuerys.AddWhere(FrmEleAttr.EleID, this.HisEn.GetValStrByKey("OID"));
                        if (objQuerys.DoQuery() > 0)
                        {
                            foreach (FrmEleDB single in imgdbs)
                            {
                                if (single.FK_MapData.Substring(6, single.FK_MapData.Length - 6).Equals(img.EnPK.Substring(6, img.EnPK.Length - 6)))
                                {
                                    single.FK_MapData = img.EnPK;
                                    single.MyPK = img.EnPK + "_" + this.HisEn.GetValStrByKey("OID") + "_" + img.EnPK;
                                    single.RefPKVal = img.EnPK;
                                    //  single.DirectInsert();
                                    //  realDB = single; cut by zhoupeng .没有看明白.
                                    break;
                                }
                            }
                        }
                        else
                        {
                            realDB = imgDb;
                        }
                    }
                    else
                    {
                        realDB = imgDb;
                    }

                    if (realDB != null)
                    {
                        imgSrc = realDB.Tag1;
                        //如果没有查到记录，控件不显示。说明没有走盖章的一步
                        x = img.X + wtX;
                        this.Add("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + x + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                        this.Add("\t\n<img src='" + imgSrc + "' onerror='javascript:this.src='" + appPath + "DataUser/ICON/" + BP.Sys.SystemConfig.CustomerNo + "/LogBiger.png';' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' />");
                        this.Add("\t\n</DIV>");
                    }
                }
                #endregion
            }
            #endregion 输出竖线与标签

            #region 输出数据控件.
            TB tb = new TB();
            //DDL ddl = new DDL();
            //CheckBox cb = new CheckBox();
            int fSize = 0;
            foreach (MapAttr attr in mattrs)
            {
                //处理隐藏字段，如果是不可见并且是启用的就隐藏.
                if (attr.UIVisible == false && attr.UIIsEnable)
                {
                    TB tbH = new TB();
                    //tbH.Visible = false;
                    tbH.Attributes["Style"] = "display:none;";
                    tbH.ID = "TB_" + attr.KeyOfEn;
                    tbH.Text = en.GetValStrByKey(attr.KeyOfEn);
                    this.Add(tbH);
                    continue;
                }

                if (attr.UIVisible == false)
                    continue;

                x = attr.X + wtX;
                if (attr.LGType == FieldTypeS.Enum || attr.LGType == FieldTypeS.FK)
                    this.Add("<DIV id='F" + attr.KeyOfEn + "' style='position:absolute; left:" + x + "px; top:" + attr.Y + "px;  height:16px;text-align: left;word-break: keep-all;' >");
                else
                    this.Add("<DIV id='F" + attr.KeyOfEn + "' style='position:absolute; left:" + x + "px; top:" + attr.Y + "px; width:auto" + attr.UIWidth + "px; height:16px;text-align: left;word-break: keep-all;' >");

                this.Add("<span>");

                #region add contrals.
                if (attr.UIIsEnable == false && this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                {
                    MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                    string url = meLink.Tag;
                    if (url.Contains("?") == false)
                        url = url + "?a3=2";
                    url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + enName;
                    if (url.Contains("@AppPath"))
                        url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + appPath);
                    if (url.Contains("@"))
                    {
                        Attrs attrs = en.EnMap.Attrs;
                        foreach (Attr item in attrs)
                        {
                            url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                            if (url.Contains("@") == false)
                                break;
                        }
                    }
                    this.Add("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                    this.Add("</span>");
                    this.Add("</DIV>");
                    continue;
                }

                #region 数字签名
                if (attr.IsSigan)
                {
                    #region 判断权限
                    bool isEdit = false;//是否可以编辑签名
                    string v = en.GetValStrByKey(attr.KeyOfEn);

                    //如果为空，默认使用当前登录人签名
                    if (string.IsNullOrEmpty(v) && activeFilds.Contains(attr.KeyOfEn + ","))
                        v = WebUser.No;

                    //如果为只读并且为空，显示为未签名
                    if (this.IsReadonly)
                        v = "sigan-readonly";

                    if (attr.PicType == PicType.ShouDong)
                    {
                        isEdit = true;
                        v = "sigan-readonly";
                    }

                    if (this.FK_Node != 0 && this.IsReadonly == false)
                    {
                        //获取表单方案，如果为可编辑，则对属性设置为true
                        v = en.GetValStrByKey(attr.KeyOfEn);
                        long workId = Convert.ToInt64(this.HisEn.GetValStrByKey("OID"));
                        FrmField keyOfEn = new FrmField();
                        QueryObject info = new QueryObject(keyOfEn);
                        info.AddWhere(FrmFieldAttr.FK_Node, this.FK_Node);
                        info.addAnd();
                        info.AddWhere(FrmFieldAttr.FK_MapData, attr.FK_MapData);
                        info.addAnd();
                        info.AddWhere(FrmFieldAttr.KeyOfEn, attr.KeyOfEn);
                        info.addAnd();
                        info.AddWhere(MapAttrAttr.UIIsEnable, "1");
                        if (info.DoQuery() > 0)
                        {
                            isEdit = true;//可编辑，如果值为空显示可编辑图片
                            if (string.IsNullOrEmpty(v))
                                v = "siganture";
                        }
                        else
                        {
                            isEdit = false;
                            //不可编辑，如果值为空显示不可编辑图片
                            if (string.IsNullOrEmpty(v))
                                v = "sigan-readonly";
                        }
                    }
                    #endregion 判断权限

                    #region 图片签名 (dai guoqiang)
                    if (attr.SignType == SignType.Pic)
                    {
                        //如果为可编辑，对签名进行修改.
                        if (isEdit)
                        {
                            this.Add("<img src='" + appPath + "DataUser/Siganture/" + v + ".jpg' "
                            + "ondblclick=\"SigantureAct(this,'" + WebUser.No + "','" + attr.FK_MapData + "','" + attr.KeyOfEn
                            + "','" + this.HisEn.GetValStrByKey("OID") + "');\" border=\"0\" alt=\"双击进行签名或取消签名\" onerror=\"this.src='" + appPath + "DataUser/Siganture/UnName.jpg'\"/>");
                        }
                        else
                        {
                            //zhoupeng 增加业务逻辑判断, 不影响以前的逻辑 for 动态字段的需要..
                            if (activeFilds != "")
                            {
                                /* 有动态字段的权限情况. */
                                string myuser = en.GetValStringByKey(attr.KeyOfEn);
                                if (myuser == "" && activeFilds.Contains(attr.KeyOfEn + ",") == false)
                                {
                                    /*说明没有签名,直接输出图片.*/
                                    v = "sigan-readonly";
                                }
                                if (myuser == "" && activeFilds.Contains(attr.KeyOfEn + ",") == true)
                                {
                                    v = BP.Web.WebUser.No;

                                    //直接更新到数据库里.
                                    en.Update(attr.KeyOfEn, WebUser.No);
                                }
                            }

                            this.Add("<img  style='border:0px;width:90px;' src='" + appPath + "DataUser/Siganture/" + v + ".jpg' border=0 onerror=\"this.src='" + appPath + "DataUser/Siganture/UnName.jpg'\"/>");

                        }
                    } //结束图片签名.

                    #endregion 结束图片签名

                    #region 广东CA签名 (zhoupeng 2016-03-12)
                    if (attr.SignType == SignType.GDCA)
                    {
                        if (IsAddCa == false && isEdit == true)
                        {
                            IsAddCa = true;
                            HtmlGenericControl loadWebSignJs = new HtmlGenericControl("script");
                            loadWebSignJs.Attributes["type"] = "text/javascript";
                            loadWebSignJs.Attributes["src"] = "/WF/Activex/GDCASign/Loadwebsign.js";
                            Page.Header.Controls.Add(loadWebSignJs);
                            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "mainCA", "/WF/Activex/GDCASign/main.js");
                        }

                        if (!string.IsNullOrEmpty(attr.Para_SiganField))
                        {
                            //string signClient = this.GetTBByID("TB_" + attr.Para_SiganField).ClientID;
                            string signClient = "";
                            if (this.PageID == "Frm")
                                signClient = "ctl00$ContentPlaceHolder1$UCEn1$TB_" + attr.Para_SiganField;
                            else
                                signClient = "ctl00$ContentPlaceHolder1$MyFlowUC1$MyFlow1$UCEn1$TB_" + attr.Para_SiganField;

                            this.Add("<span id='" + signClient + "sealpostion' />");
                            if (isEdit)
                            {
                                this.Add("<img  src='" + appPath + "DataUser/Siganture/setting.JPG' ondblclick=\"addseal('" + signClient + "');\"  border=0 onerror=\"this.src='" + appPath + "DataUser/Siganture/UnName.jpg'\"/>");
                            }
                            else
                            {
                                string image =
                                    Server.MapPath(string.Format("~" + appPath + "DataUser/Siganture/{0}",
                                        this.HisEn.GetValStrByKey("OID")));
                                string realImage = "";
                                if (Directory.Exists(image))
                                {
                                    image =
                                        Server.MapPath(string.Format("~" + appPath + "DataUser/Siganture/{0}/{1}.jpg",
                                            this.HisEn.GetValStrByKey("OID"), v));
                                    if (File.Exists(image))
                                        realImage = string.Format(appPath + "DataUser/Siganture/{0}/{1}.jpg", this.HisEn.GetValStrByKey("OID"), v);
                                    else
                                        realImage = appPath + "DataUser/Siganture/" + v + ".jpg";
                                }
                                else
                                    realImage = appPath + "DataUser/Siganture/" + v + ".jpg";
                                this.Add("<img src='" + realImage + "' border=0 onerror=\"this.src='" + appPath + "DataUser/Siganture/UnName.jpg'\"/>");
                            }
                        }
                    }
                    #endregion 结束CA签名

                    #region 山东CA签名 (song honggang 2014-06-08)
                    if (attr.SignType == SignType.CA)
                    {
                        if (IsAddCa == false && isEdit == true)
                        {
                            IsAddCa = true;
                            HtmlGenericControl loadWebSignJs = new HtmlGenericControl("script");
                            loadWebSignJs.Attributes["type"] = "text/javascript";
                            loadWebSignJs.Attributes["src"] = "/WF/Activex/Sign/Loadwebsign.js";
                            Page.Header.Controls.Add(loadWebSignJs);
                            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "mainCA", "/WF/Activex/Sign/main.js");
                        }

                        if (!string.IsNullOrEmpty(attr.Para_SiganField))
                        {
                            //string signClient = this.GetTBByID("TB_" + attr.Para_SiganField).ClientID;
                            string signClient = "";
                            if (this.PageID == "Frm")
                                signClient = "ctl00$ContentPlaceHolder1$UCEn1$TB_" + attr.Para_SiganField;
                            else
                                signClient = "ctl00$ContentPlaceHolder1$MyFlowUC1$MyFlow1$UCEn1$TB_" + attr.Para_SiganField;

                            this.Add("<span id='" + signClient + "sealpostion' />");
                            if (isEdit)
                            {
                                this.Add("<img  src='" + appPath + "DataUser/Siganture/setting.JPG' ondblclick=\"addseal('" + signClient + "');\"  border=0 onerror=\"this.src='" + appPath + "DataUser/Siganture/UnName.jpg'\"/>");
                            }
                            else
                            {
                                string image =
                                    Server.MapPath(string.Format("~" + appPath + "DataUser/Siganture/{0}",
                                        this.HisEn.GetValStrByKey("OID")));
                                string realImage = "";
                                if (Directory.Exists(image))
                                {
                                    image =
                                        Server.MapPath(string.Format("~" + appPath + "DataUser/Siganture/{0}/{1}.jpg",
                                            this.HisEn.GetValStrByKey("OID"), v));
                                    if (File.Exists(image))
                                        realImage = string.Format(appPath + "DataUser/Siganture/{0}/{1}.jpg", this.HisEn.GetValStrByKey("OID"), v);
                                    else
                                        realImage = appPath + "DataUser/Siganture/" + v + ".jpg";
                                }
                                else
                                    realImage = appPath + "DataUser/Siganture/" + v + ".jpg";
                                this.Add("<img src='" + realImage + "'  onerror=\"this.src='" + appPath + "DataUser/Siganture/UnName.jpg'\"/>");
                            }
                        }
                    }
                    #endregion 结束CA签名

                    this.Add("</span>");
                    this.Add("</DIV>");
                    continue;

                }
                #endregion

                if (attr.MaxLen >= 3999 && attr.TBModel == 2)
                {
                    this.AddRichTextBox(en, attr);
                    this.Add("</span>");
                    this.Add("</DIV>");
                    continue;
                }

                if (attr.UIContralType == UIContralType.TB)
                {
                    tb = new TB();
                    tb.ID = "TB_" + attr.KeyOfEn;
                    if (attr.UIIsEnable == false || isReadonly == true)
                    {
                        if (activeFilds.Contains(attr.KeyOfEn + ",") == false)
                        {
                            /*如果动态字段不包含他们. */
                            tb.Attributes.Add("readonly", "true");
                            tb.CssClass = "TBReadonly";
                            tb.ReadOnly = true;
                        }
                        else
                        {
                            tb.Attributes["onchange"] += "Change('" + attr.FK_MapData + "');";
                        }
                    }
                    else
                    {
                        //add by dgq 2013-4-9 添加修改事件
                        tb.Attributes["onchange"] += "Change('" + attr.FK_MapData + "');";
                    }
                    tb.Attributes["tabindex"] = attr.Idx.ToString();
                }

                #region 通过逻辑类型，输出相关的控件.
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:  // 输出普通类型字段.

                        #region 判断外部数据或WS类型.
                        if (attr.UIContralType == UIContralType.DDL)
                        {
                            /* 说明这是外部数据或者WS字段. */
                            DDL ddl1 = new DDL();
                            ddl1.ID = "DDL_" + attr.KeyOfEn;
                            ddl1.Attributes["tabindex"] = attr.Idx.ToString();
                            ddl1.Enabled = attr.UIIsEnable;
                            if (ddl1.Enabled)
                            {
                                string val = en.GetValStrByKey(attr.KeyOfEn);
                                ddl1.Bind(attr.HisDT, "No", "Name", val);
                                ddl1.Items.Insert(0, new ListItem("请选择", ""));
                                ddl1.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                            }
                            else
                            {
                                if (ddl1.Enabled == true && isReadonly == true)
                                    ddl1.Enabled = false;

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    ddl1.Attributes["style"] = "width: " + attr.UIWidth + "px; text-left: right; height: 19px;";
                                else
                                    ddl1.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: " + attr.UIHeight + "px;";

                                string val = en.GetValStrByKey(attr.KeyOfEn);
                                string text = en.GetValStrByKey(attr.KeyOfEn + "T");

                                ddl1.Items.Add(new ListItem(text, val));
                            }
                            if (attr.UIIsEnable == true && this.IsReadonly == true)
                                ddl1.Enabled = false;
                            this.Add(ddl1);
                            break;
                        }
                        #endregion 判断外部数据或WS类型.

                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppString:
                                if (attr.UIRows == 1)
                                {
                                    if (!string.IsNullOrEmpty(en.GetValStringByKey(attr.KeyOfEn)))
                                        tb.Text = en.GetValStringByKey(attr.KeyOfEn);
                                    else
                                        tb.Text = attr.DefVal;

                                    if ((attr.UIIsEnable && isReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                    {
                                        tb.CssClass = "TB";
                                    }
                                    else
                                    {
                                        tb.CssClass = "TBReadonly";
                                    }

                                    fSize = attr.Para_FontSize;
                                    if (fSize == 0)
                                        tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 15px;padding: 0px;margin: 0px;";
                                    else
                                        tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px; text-align: left; height: " + attr.UIHeightInt + "px;padding: 0px;margin: 0px;";
                                    this.Add(tb);
                                }
                                else
                                {
                                    tb.TextMode = TextBoxMode.MultiLine;

                                    if (!string.IsNullOrEmpty(en.GetValStringByKey(attr.KeyOfEn)))
                                        tb.Text = en.GetValStringByKey(attr.KeyOfEn);
                                    else
                                        tb.Text = attr.DefVal;

                                    fSize = attr.Para_FontSize;
                                    if (fSize == 0)
                                        tb.Attributes["style"] = "width: " + attr.UIWidth + "px;height:" + attr.UIHeightInt + "px; text-align: left;padding: 0px;margin: 0px;";
                                    else
                                        tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px;height:" + attr.UIHeightInt + "px; text-align: left;padding: 0px;margin: 0px;";


                                    tb.Attributes["maxlength"] = attr.MaxLen.ToString();
                                    tb.Rows = attr.UIRows;

                                    if ((attr.UIIsEnable && isReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                    {
                                        tb.CssClass = "TBDoc";
                                        tb.Attributes["onclick"] = "ShowHelpDiv('" + tb.ID + "','" + appPath + "','" + attr.KeyOfEn + "','" + enName + "','wordhelp');";
                                    }
                                    else
                                    {
                                        tb.CssClass = "TBReadonly";
                                    }
                                    this.Add(tb);
                                }
                                break;
                            case BP.DA.DataType.AppDate:
                                tb.ShowType = TBType.Date;
                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                if ((attr.UIIsEnable && this.IsReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                {
                                    tb.Attributes["onfocus"] = "WdatePicker();";
                                    tb.Attributes["class"] = "TB";

                                    if (tb.Text == "" && attr.DefValReal == "@RDT")
                                        tb.Text = BP.DA.DataType.CurrentData;
                                }
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 19px;";
                                else
                                    tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + fSize * 10 + "px; text-align: left;height: " + fSize + "px;";
                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppDateTime:
                                tb.ShowType = TBType.DateTime;
                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);

                                if ((attr.UIIsEnable && this.IsReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                {
                                    tb.Attributes["class"] = "TBcalendar";
                                    tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";

                                    if (tb.Text == "" && attr.DefValReal == "@RDT")
                                        tb.Text = BP.DA.DataType.CurrentData;

                                }
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 19px;";
                                else
                                    tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + fSize * 10 + "px; text-align: left; height: " + fSize + "px;";

                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppBoolean:
                                CheckBox cb = new CheckBox();
                                cb.Text = attr.Name;
                                //cb.Width = attr.UIWidthInt;
                                cb.ID = "CB_" + attr.KeyOfEn;
                                cb.Checked = attr.DefValOfBool;
                                cb.Enabled = attr.UIIsEnable;
                                cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                if ((cb.Enabled == false && activeFilds.Contains(attr.KeyOfEn + ",") == false) || isReadonly == true)
                                {
                                    cb.Enabled = false;
                                }
                                else
                                {
                                    //add by dgq 2013-4-9,添加内容修改后的事件
                                    cb.Attributes["onmousedown"] = "Change('" + attr.FK_MapData + "')";
                                    cb.Enabled = true;
                                }
                                this.Add(cb);
                                break;
                            case BP.DA.DataType.AppDouble:
                            case BP.DA.DataType.AppFloat:

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: right; height: 19px;ime-mode:disabled;";
                                else
                                    tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px; text-align: right; height: " + fSize + "px;";

                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);

                                if ((attr.UIIsEnable && isReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                {
                                    //增加验证
                                    //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');Change('" + attr.FK_MapData + "');");
                                    tb.Attributes.Add("onkeyup", @"Change('" + attr.FK_MapData + "');");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');TB_ClickNum(this,0);");

                                    tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                    tb.Attributes.Add("onkeydown", @"VirtyNum(this,'float')");
                                    tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'float');";
                                    tb.Attributes["class"] = "TBNum";
                                }
                                else
                                {
                                    tb.Attributes["class"] = "TBReadonly";
                                }

                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppInt:
                                // tb.ShowType = TBType.Num;

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: right; height: 19px;";
                                else
                                    tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px; text-align: right; height: " + fSize + "px;";

                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                if ((attr.UIIsEnable && isReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                {
                                    //增加验证
                                    tb.Attributes.Add("onkeyup", @"Change('" + attr.FK_MapData + "');");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d]/g,'');TB_ClickNum(this,0);");

                                    tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                    tb.Attributes.Add("onkeydown", "VirtyNum(this,'int')");
                                    tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'int');";
                                    tb.Attributes["class"] = "TBNum";
                                }
                                else
                                {
                                    tb.Attributes["class"] = "TBReadonly";
                                }
                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppMoney:

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: right; height: 19px;";
                                else
                                    tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px; text-align: right; height: " + fSize + "px;";


                                if ((attr.UIIsEnable && isReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                {
                                    //增加验证
                                    tb.Attributes.Add("onkeyup", @"Change('" + attr.FK_MapData + "');");
                                    tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');TB_ClickNum(this,'0.00');");
                                    tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                    tb.Attributes.Add("onkeydown", "VirtyNum(this,'float')");
                                    tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'float');";
                                    tb.Attributes["class"] = "TBNum";
                                }
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                if (SystemConfig.AppSettings["IsEnableNull"] == "1")
                                {
                                    decimal v = en.GetValMoneyByKey(attr.KeyOfEn);
                                    if (v == 567567567)
                                        tb.Text = "";
                                    else
                                        tb.Text = v.ToString("0.00");
                                }
                                else
                                {
                                    tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");
                                }

                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppRate:
                                if ((attr.UIIsEnable && isReadonly == false) || activeFilds.Contains(attr.KeyOfEn + ","))
                                    tb.Attributes["class"] = "TBNum";
                                else
                                    tb.Attributes["class"] = "TBReadonly";
                                tb.ShowType = TBType.Moneny;
                                tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");

                                fSize = attr.Para_FontSize;
                                if (fSize == 0)
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: right; height: 19px;";
                                else
                                    tb.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px; text-align: right; height: " + fSize + "px;";

                                //增加验证
                                //tb.Attributes.Add("onkeyup", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'')");
                                this.Add(tb);
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum:
                        if (attr.UIContralType == UIContralType.DDL)
                        {
                            DDL ddle = new DDL();
                            ddle.ID = "DDL_" + attr.KeyOfEn;
                            ddle.Attributes["tabindex"] = attr.Idx.ToString();
                            if (attr.UIIsEnable || activeFilds.Contains(attr.KeyOfEn + ","))
                            {
                                ddle.BindSysEnum(attr.UIBindKey);
                                ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                                ddle.Enabled = true;

                                //add by dgq 2013-4-9,添加内容修改后的事件
                                ddle.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                            }
                            else
                            {
                                ddle.Items.Add(new ListItem(en.GetValRefTextByKey(attr.KeyOfEn), en.GetValStringByKey(attr.KeyOfEn)));
                                ddle.Enabled = false;
                            }
                            this.Add(ddle);
                        }
                        else
                        {
                            //BP.Sys.FrmRBs rbs = new FrmRBs();
                            //rbs.Retrieve(FrmRBAttr.FK_MapData, enName,
                            //    FrmRBAttr.KeyOfEn, attr.KeyOfEn);
                        }
                        break;
                    case FieldTypeS.FK:
                        DDL ddlFK = new DDL();
                        ddlFK.ID = "DDL_" + attr.KeyOfEn;
                        ddlFK.Attributes["tabindex"] = attr.Idx.ToString();
                        this.Add(ddlFK);
                        if (ddlFK.Enabled || activeFilds.Contains(attr.KeyOfEn + ","))
                        {
                            EntitiesNoName ens = attr.HisEntitiesNoName;
                            ens.RetrieveAll();
                            ddlFK.Enabled = true;

                            //added by liuxc，2015-10-22
                            //此处判断是否含有级联下拉框的情况，如果此属性是级联的下拉框，则判断其引起级联的属性值，根据此值只加载其级联子项
                            MapExt me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                                                           MapExtAttr.AttrsOfActive, attr.KeyOfEn) as MapExt;
                            if (me != null)
                            {
                                string valOper = en.GetValStringByKey(me.AttrOfOper);

                                if (IsPostBack)
                                {
                                    valOper = Request.Params[ddlFK.UniqueID.Substring(0, ddlFK.UniqueID.Length - ddlFK.ID.Length) + "DDL_" + me.AttrOfOper];
                                }

                                if (!string.IsNullOrWhiteSpace(valOper))
                                {
                                    string fullSQL = me.Doc.Clone() as string;
                                    fullSQL = fullSQL.Replace("~", ",");
                                    fullSQL = fullSQL.Replace("@Key", valOper).Replace("@key", valOper);
                                    fullSQL = fullSQL.Replace("@WebUser.No", WebUser.No);
                                    fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                                    fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                    fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);

                                    DataTable dt = DBAccess.RunSQLReturnTable(fullSQL);
                                    ddlFK.Items.Clear();
                                    foreach (DataRow dr in dt.Rows)
                                        ddlFK.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                }
                                else
                                {
                                    ddlFK.Items.Clear();
                                }
                            }
                            else
                            {
                                ddlFK.BindEntities(ens);
                            }

                            if (ddlFK.Items.Count > 0)
                            {
                                if (ddlFK.Items.FindByText("请选择") == null)
                                {
                                    ddlFK.Items.Insert(0, new ListItem("请选择", ""));
                                }
                            }
                            else
                            {
                                if (ddlFK.Items.FindByText("无数据") == null)
                                {
                                    ddlFK.Items.Insert(0, new ListItem("无数据", ""));
                                }
                            }
                            //if (ddlFK.Items.FindByText("请选择") == null)
                            //{
                            //    if (ddlFK.Items.Count > 0)
                            //        ddlFK.Items.Insert(0, new ListItem("请选择", ""));
                            //    else
                            //        ddlFK.Items.Add(new ListItem("请选择", ""));
                            //}

                            string val = en.GetValStrByKey(attr.KeyOfEn);
                            if (string.IsNullOrEmpty(val) == true)
                            {
                                ddlFK.SetSelectItem("");
                            }
                            else
                                ddlFK.SetSelectItem(val);

                            //add by dgq 2013-4-9,添加内容修改后的事件
                            ddlFK.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                        }
                        else
                        {
                            if (ddlFK.Enabled == true && isReadonly == true)
                                ddlFK.Enabled = false;

                            fSize = attr.Para_FontSize;
                            if (fSize == 0)
                                ddlFK.Attributes["style"] = "width: " + attr.UIWidth + "px; text-left: right; height: 19px;";
                            else
                                ddlFK.Attributes["style"] = "font-size: " + fSize + "px;width: " + attr.UIWidth + "px; text-align: left; height: " + fSize + "px;";

                            string val = en.GetValStrByKey(attr.KeyOfEn);
                            string text = en.GetValRefTextByKey(attr.KeyOfEn);

                            if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(val) == false)
                            {
                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                Entity myen = ens.GetNewEntity;
                                myen.PKVal = val;
                                if (myen.RetrieveFromDBSources() != 0)
                                    text = myen.GetValStringByKey("Name");
                                else
                                    text = val;
                            }
                            ddlFK.Items.Add(new ListItem(text, val));

                            ddlFK.Enabled = false;
                        }
                        break;
                    default:
                        break;
                }
                #endregion 通过逻辑类型，输出相关的控件.

                #endregion add contrals.

                this.Add("</span>");
                this.Add("</DIV>");
            }

            #region  输出 rb.
            BP.Sys.FrmRBs myrbs = this.mapData.FrmRBs;
            MapAttr attrRB = new MapAttr();
            foreach (BP.Sys.FrmRB rb in myrbs)
            {
                x = rb.X + wtX;
                this.Add("<DIV id='F" + rb.MyPK + "' style='position:absolute; left:" + x + "px; top:" + rb.Y + "px; width:100%; height:16px;text-align: left;word-break: keep-all;' >");
                this.Add("<span style='word-break: keep-all;font-size:12px;'>");

                System.Web.UI.WebControls.RadioButton rbCtl = new RadioButton();
                rbCtl.ID = "RB_" + rb.KeyOfEn + "_" + rb.IntKey.ToString();
                rbCtl.GroupName = rb.KeyOfEn;
                rbCtl.Text = rb.Lab;
                this.Add(rbCtl);

                if (attrRB.KeyOfEn != rb.KeyOfEn)
                {
                    foreach (MapAttr ma in mattrs)
                    {
                        if (ma.KeyOfEn == rb.KeyOfEn)
                        {
                            attrRB = ma;
                            break;
                        }
                    }
                }
                if (isReadonly == true || attrRB.UIIsEnable == false)
                    rbCtl.Enabled = false;
                else
                {
                    //add by dgq 2013-4-9,添加内容修改后的事件
                    rbCtl.Attributes["onmousedown"] = "Change('" + attrRB.FK_MapData + "')";
                }
                this.Add("</span>");
                this.Add("</DIV>");
            }

            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    string id = "RB_" + attr.KeyOfEn + "_" + en.GetValStrByKey(attr.KeyOfEn);
                    RadioButton rb = this.GetRBLByID(id);
                    if (rb != null)
                        rb.Checked = true;
                }
            }
            #endregion  输出 rb.

            #endregion 输出数据控件.

            #region 输出明细.
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                x = dtl.X + wtX;
                float y = dtl.Y;

                this.Add("<DIV id='Fd" + dtl.No + "' style='position:absolute; left:" + x + "px; top:" + y + "px; width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;' >");
                this.Add("<span>");

                string src = "";
                if (dtl.HisDtlShowModel == DtlShowModel.Table)
                {
                    if (isReadonly == true)
                        src = appPath + "WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1&FID=" + en.GetValStrByKey("FID", "0") + "&FK_Node=" + this.Request.QueryString["FK_Node"];
                    else
                        src = appPath + "WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=0&FID=" + en.GetValStrByKey("FID", "0") + "&FK_Node=" + this.Request.QueryString["FK_Node"];
                }
                else
                {
                    if (isReadonly == true)
                        src = appPath + "WF/CCForm/DtlCard.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1&FID=" + en.GetValStrByKey("FID", "0");
                    else
                        src = appPath + "WF/CCForm/DtlCard.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=0&FID=" + en.GetValStrByKey("FID", "0");
                }

                if (this.IsReadonly == true || dtl.IsReadonly)
                {
                    this.Add("<iframe ID='F" + dtl.No + "' src='" + src +
                             "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H +
                             "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                }
                else
                {
                    if (dtl.DtlSaveModel == DtlSaveModel.AutoSave)
                        AddLoadFunction(dtl.No, "blur", "SaveDtl");

                    //this.Add("<iframe ID='F" + dtl.No + "' Onblur=\"SaveDtl('" + dtl.No + "');\"  src='" + src + "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                    this.Add("<iframe ID='F" + dtl.No + "' onload= '" + dtl.No + "load();'  src='" + src + "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

                }

                this.Add("</span>");
                this.Add("</DIV>");
            }

            string js = "";
            if (this.IsReadonly == false)
            {
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveDtl(dtl) { ";
                js += "\t\n   GenerPageKVs(); //调用产生kvs ";
                js += "\t\n   var iframe = document.getElementById('F' + dtl );";
                js += "\t\n   if(iframe && iframe.contentWindow){ ";
                js += "\t\n      iframe.contentWindow.SaveDtlData(); ";
                js += "\t\n   } ";
                js += "\t\n } ";
                js += "\t\n function SaveM2M(dtl) { ";
                js += "\t\n   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
                js += "\t\n } ";

                js += "\t\n</script>";
                this.Add(js);
            }
            #endregion 输出明细.

            #region 输出报表.
            foreach (FrmRpt rpt in md.FrmRpts)
            {
                if (rpt.IsView == false)
                    continue;

                x = rpt.X + wtX;
                float y = rpt.Y;

                this.Add("<DIV id='Fd" + rpt.No + "' style='position:absolute; left:" + x + "px; top:" + y + "px; width:" + rpt.W + "px; height:" + rpt.H + "px;text-align: left;' >");
                this.Add("<span>");

                string src = "";
                if (rpt.HisDtlShowModel == DtlShowModel.Table)
                {
                    if (isReadonly == true)
                        src = appPath + "WF/CCForm/Dtl.aspx?EnsName=" + rpt.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1&FID=" + en.GetValStrByKey("FID", "0");
                    else
                        src = appPath + "WF/CCForm/Dtl.aspx?EnsName=" + rpt.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=0&FID=" + en.GetValStrByKey("FID", "0");
                }
                else
                {
                    if (isReadonly == true)
                        src = appPath + "WF/CCForm/DtlCard.aspx?EnsName=" + rpt.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1&FID=" + en.GetValStrByKey("FID", "0");
                    else
                        src = appPath + "WF/CCForm/DtlCard.aspx?EnsName=" + rpt.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=0&FID=" + en.GetValStrByKey("FID", "0");
                }

                if (this.IsReadonly == true || rpt.IsReadonly)
                    this.Add("<iframe ID='F" + rpt.No + "' src='" + src +
                             "' frameborder=0  style='position:absolute;width:" + rpt.W + "px; height:" + rpt.H +
                             "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                else
                {
                    AddLoadFunction(rpt.No, "blur", "SaveDtl");

                    //this.Add("<iframe ID='F" + rpt.No + "' Onblur=\"SaveDtl('" + rpt.No + "');\"  src='" + src + "' frameborder=0  style='position:absolute;width:" + rpt.W + "px; height:" + rpt.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                    this.Add("<iframe ID='F" + rpt.No + "' onload='" + rpt.No + "load();'  src='" + src + "' frameborder=0  style='position:absolute;width:" + rpt.W + "px; height:" + rpt.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                }

                this.Add("</span>");
                this.Add("</DIV>");
            }
            #endregion 输出报表.

            #region 审核组件
            FrmWorkCheck fwc = new FrmWorkCheck(enName);
            if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Disable)
            {
                x = fwc.FWC_X + wtX;
                this.Add("<DIV id='DIVWC" + fwc.No + "' style='position:absolute; left:" + x + "px; top:" + fwc.FWC_Y + "px; width:" + fwc.FWC_W + "px; height:" + fwc.FWC_H + "px;text-align: left;' >");
                this.Add("<span>");
                string src = appPath + "WF/WorkOpt/WorkCheck.aspx?s=2";
                string fwcOnload = "";
                string paras = this.RequestParas;
                try
                {
                    if (paras.Contains("FID=") == false)
                        paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                }
                catch
                {

                }

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");

                if (fwc.HisFrmWorkCheckSta == FrmWorkCheckSta.Readonly)
                {
                    src += "&DoType=View";
                }
                else
                {
                    fwcOnload = "onload= 'WC" + fwc.No + "load();'";
                    AddLoadFunction("WC" + fwc.No, "blur", "SaveDtl");
                }
                src += "&r=q" + paras;
                this.Add("<iframe ID='FWC" + fwc.No + "' " + fwcOnload + "  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + fwc.FWC_W + "' height='" + fwc.FWC_H + "'   scrolling=auto/></iframe>");
                this.Add("</span>");
                this.Add("</DIV>");
            }
            #endregion 审核组件

            #region 父子流程组件
            FrmSubFlow subFlow = new FrmSubFlow(enName);
            if (subFlow.HisFrmSubFlowSta != FrmSubFlowSta.Disable)
            {
                x = subFlow.SF_X + wtX;
                this.Add("<DIV id='DIVWC" + fwc.No + "' style='position:absolute; left:" + x + "px; top:" + subFlow.SF_Y + "px; width:" + subFlow.SF_W + "px; height:" + subFlow.SF_H + "px;text-align: left;' >");
                this.Add("<span>");
                string src = appPath + "WF/WorkOpt/SubFlow.aspx?s=2";
                string fwcOnload = "";
                string paras = this.RequestParas;
                try
                {
                    if (paras.Contains("FID=") == false)
                        paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                }
                catch
                {
                }
                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");
                if (subFlow.HisFrmSubFlowSta == FrmSubFlowSta.Readonly)
                {
                    src += "&DoType=View";
                }
                else
                {
                    //  fwcOnload = "onload= 'WC" + fwc.No + "load();'";
                    // AddLoadFunction("WC" + fwc.No, "blur", "SaveDtl");
                }
                src += "&r=q" + paras;
                this.Add("<iframe ID='FWC" + subFlow.No + "' " + fwcOnload + "  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + subFlow.SF_W + "' height='" + subFlow.SF_H + "'   scrolling=auto/></iframe>");
                this.Add("</span>");
                this.Add("</DIV>");
            }
            #endregion 父子流程组件

            #region 多对多的关系
            foreach (MapM2M m2m in m2ms)
            {
                x = m2m.X + wtX;
                this.Add("<DIV id='Fd" + m2m.NoOfObj + "' style='position:absolute; left:" + x + "px; top:" + m2m.Y + "px; width:" + m2m.W + "px; height:" + m2m.H + "px;text-align: left;' >");
                this.Add("<span>");

                string src = ".aspx?NoOfObj=" + m2m.NoOfObj;
                string paras = this.RequestParas;
                try
                {
                    if (paras.Contains("FID=") == false)
                        paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                }
                catch
                {
                }

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");
                src += "&r=q" + paras;
                if (m2m.IsEdit)
                    src += "&IsEdit=1";
                else
                    src += "&IsEdit=0";

                if (src.Contains("FK_MapData") == false)
                    src += "&FK_MapData=" + enName;

                if (m2m.HisM2MType == M2MType.M2MM)
                    src = appPath + "WF/CCForm/M2MM" + src;
                else
                    src = appPath + "WF/CCForm/M2M" + src;

                switch (m2m.ShowWay)
                {
                    case FrmShowWay.FrmAutoSize:
                    case FrmShowWay.FrmSpecSize:
                        if (m2m.IsEdit)
                        {
                            AddLoadFunction(m2m.NoOfObj, "blur", "SaveM2M");

                            // this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "'   scrolling=auto/></iframe>");
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'  onload='" + m2m.NoOfObj + "load();'  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "'   scrolling=auto/></iframe>");

                        }
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "'   scrolling=auto/></iframe>");
                        break;
                    case FrmShowWay.Hidden:
                        break;
                    case FrmShowWay.WinOpen:
                        this.Add("<a href=\"javascript:WinOpen('" + src + "&IsOpen=1','" + m2m.W + "','" + m2m.H + "');\"  />" + m2m.Name + "</a>");
                        break;
                    default:
                        break;
                }
                this.Add("</span>");
                this.Add("</DIV>");
            }
            #endregion 多对多的关系

            #region 输出附件
            FrmAttachments aths = this.mapData.FrmAttachments;
            FrmAttachmentDBs athDBs = null;
            if (aths.Count > 0)
                athDBs = new FrmAttachmentDBs(enName, en.PKVal.ToString());

            foreach (FrmAttachment ath in aths)
            {
                if (ath.IsVisable == false)
                    continue;

                if (ath.UploadType == AttachmentUploadType.Single)
                {
                    /* 单个文件 */
                    FrmAttachmentDB athDB = athDBs.GetEntityByKey(FrmAttachmentDBAttr.FK_FrmAttachment, ath.MyPK) as FrmAttachmentDB;
                    x = ath.X + wtX;
                    float y = ath.Y;
                    this.Add("<DIV id='Fa" + ath.MyPK + "' style='position:absolute; left:" + x + "px; top:" + y + "px; text-align: left;float:left' >");
                    //  this.Add("<span>");
                    this.Add("<DIV>");
                    Label lab = new Label();
                    lab.ID = "Lab" + ath.MyPK;
                    this.Add(lab);

                    string node = "";
                    try
                    {
                        node = this.HisEn.GetValStrByKey("FK_Node");
                        if (node == "0" || node == "")
                            node = ((Work)en).NodeID.ToString();
                    }
                    catch
                    {
                        node = this.Request.QueryString["FK_Node"];
                        //  node = ((Work)en).NodeID.ToString();
                    }

                    if (athDB != null)
                    {

                        //  lab.Text = "<img src='" + appPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName;
                        if (ath.IsWoEnableWF)
                            lab.Text = "<a  href=\"javascript:OpenOfiice('" + athDB.FK_FrmAttachment + "','" + this.HisEn.GetValStrByKey("OID") + "','" + athDB.MyPK + "','" + this.FK_MapData + "','" + ath.NoOfObj + "','" + node + "')\"><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName + "</a>";
                        else
                            lab.Text = "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName;
                        // lab.Text = "<a href='" + this.Request.ApplicationPath + "DataUser/UploadFile/" + athDB.FilePathName + "' target=_blank ><img src='/WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName + "</a>";
                    }
                    this.Add("</DIV>");

                    this.Add("<DIV>");
                    Button mybtn = new Button();
                    mybtn.CssClass = "Btn";

                    if (ath.IsUpload && this.IsReadonly == false)
                    {
                        mybtn.ID = ath.MyPK;
                        mybtn.Text = "上传";
                        mybtn.CssClass = "bg";
                        mybtn.ID = "Btn_Upload_" + ath.MyPK + "_" + this.HisEn.PKVal;
                        mybtn.Attributes["style"] = "display:none;";
                        mybtn.Click += new EventHandler(btnUpload_Click);
                        this.Add(mybtn);
                        FileUpload fu = new FileUpload();
                        fu.ID = ath.MyPK;
                        fu.Attributes["Width"] = ath.W.ToString();
                        string uploadName = "";
                        if (this.PageID == "Frm")
                            uploadName = "ContentPlaceHolder1_UCEn1_" + mybtn.ID;
                        else
                            uploadName = "ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_" + mybtn.ID;
                        fu.Attributes["onchange"] = "   UploadChange('" + uploadName + "');";
                        // fu.Attributes["style"] = "display:none;";
                        this.Add(fu);

                        #region  增加大附件上传
                        //加载js方法
                        /*
                        this.Page.RegisterClientScriptBlock("jquery1.7.2",
                     "<script language='JavaScript' src='" + CCFlowAppPath + "WF/Scripts/jquery-1.7.2.min.js' ></script>");
                        //  this.Page.RegisterClientScriptBlock("jqueryUploadify",
                        //"<script language='JavaScript' src='" + CCFlowAppPath + "WF/Script/Jquery-plug/jquery.uploadify/jquery.uploadify.v2.1.0.js' ></script>");
                        //    this.Page.RegisterClientScriptBlock("jqueryUpSwf",
                        //"<script language='JavaScript' src='" + CCFlowAppPath + "WF/Script/Jquery-plug/jquery.uploadify/swfobject.js' ></script>");
                        //    this.Page.RegisterClientScriptBlock("jqueryUpSwfcss",
                        //     "<link href='" + BP.WF.Glo.CCFlowAppPath + "WF/Script/Jquery-plug/jquery.uploadify/uploadify.css' rel='stylesheet' type='text/css' />");
                        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "jquery1.7.2", CCFlowAppPath + "WF/Scripts/jquery-1.7.2.min.js");
                        ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "uploadify", "" + CCFlowAppPath + "WF/Scripts/Jquery-plug/fileupload/jquery.uploadify.js");
                        //ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "jqueryUpSwf", CCFlowAppPath + "WF/Scripts/Jquery-plug/jquery.uploadify/swfobject.js");
                        // ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "jqueryUpSwfcss", BP.WF.Glo.CCFlowAppPath + "WF/Scripts/Jquery-plug/fileupload/uploadify.css");
                        this.Page.RegisterClientScriptBlock("jqueryUpSwfcss",
                         "<link href='" + BP.WF.Glo.CCFlowAppPath + "WF/Scripts/Jquery-plug/fileupload/uploadify.css' rel='stylesheet' type='text/css' />");
                        //输出按钮
                        //   this.Add("<div style='float:left'>");
                        this.Add("<input type='file' name='file_upload' id='file_upload' />");
                        System.Text.StringBuilder uploadJS = new System.Text.StringBuilder();
                        uploadJS.Append("<script language='javascript' type='text/javascript'> ");

                        uploadJS.Append("\t\n  $(function() {");
                        uploadJS.Append("\t\n $('#file_upload').uploadify({");
                        uploadJS.Append("\t\n 'auto': false,");
                        uploadJS.Append("\t\n 'swf': '" + BP.WF.Glo.CCFlowAppPath + "WF/Scripts/Jquery-plug/fileupload/uploadify.swf',");
                        uploadJS.Append("\t\n 'uploader':  '" + BP.WF.Glo.CCFlowAppPath + "WF/CCForm/JQFileUpload.ashx?AttachPK=" + ath.MyPK + "&WorkID=" + this.HisEn.PKVal + "&DoType=SingelAttach&FK_Node=" + node + "&EnsName=" + this.EnName + "',");
                        uploadJS.Append("\t\n 'cancelImage': '" + BP.WF.Glo.CCFlowAppPath + "WF/Scripts/Jquery-plug/fileupload/uploadify-cancel.png',");
                        uploadJS.Append("\t\n   'auto': true,");
                        uploadJS.Append("\t\n 'fileTypeDesc':'请选择上传文件',");
                        uploadJS.Append("\t\n 'buttonText':'附件上传',");
                        uploadJS.Append("\t\n 'width'     :100,");
                        uploadJS.Append("\t\n   'fileTypeExts':'"+ath.Exts.Replace("|",";")+"',");
                        uploadJS.Append("\t\n  'height'    :26,");
                        uploadJS.Append("\t\n  'multi'     :true,");
                        uploadJS.Append("\t\n    'queueSizeLimit':1,");
                        // uploadJS.Append("\t\n    ' onSelect': function (event, queueID, fileObj) {");
                        //  uploadJS.Append("\t\n     $('#file_upload').uploadifySettings('scriptData', { 'AttachPK':'" + ath.MyPK + "','WorkID':'" + this.HisEn.PKVal + "' ,'DoType':'SingelAttach','FK_Node':'" + node + "','EnsName':'" + this.EnName + "'});");
                        //  uploadJS.Append("\t\n     },");
                        uploadJS.Append("\t\n  'onUploadSuccess': function (file,data,response) {");
                        uploadJS.Append("\t\n        UploadChange('" + uploadName + "');");
                        uploadJS.Append("\t\n       },");
                        uploadJS.Append("\t\n  'removeCompleted' : false");
                        uploadJS.Append("\t\n });");

                          //uploadJS.Append("\t\n  $('#file_upload').css('float':'left');");
                        
                        uploadJS.Append("\t\n });");
                        uploadJS.Append("\t\n </script>");
                        //  uploadJS.Append("\t\n 'fileTypeExts':'*.pdf;*.jpg;*.jpeg;*.gif;*.png'");

                        this.Add(uploadJS.ToString());
                        //   this.Add("</div>");*/
                        #endregion  增加大附件上传

                    }
                    this.Add("<DIV style='float:left'>");
                    if (ath.IsDownload)
                    {
                        mybtn = new Button();
                        mybtn.Text = "下载";
                        mybtn.CssClass = "Btn";

                        mybtn.ID = "Btn_Download_" + ath.MyPK + "_" + this.HisEn.PKVal;
                        mybtn.Click += new EventHandler(btnUpload_Click);
                        mybtn.CssClass = "bg";

                        if (athDB == null)
                            mybtn.Visible = false;
                        else
                            mybtn.Visible = true;
                        this.Add(mybtn);
                    }

                    if (this.IsReadonly == false)
                    {
                        if (ath.IsDelete)
                        {
                            mybtn = new Button();
                            mybtn.CssClass = "Btn";
                            mybtn.Text = "删除";
                            mybtn.Attributes["onclick"] = " return confirm('您确定要执行删除吗？');";

                            mybtn.ID = "Btn_Delete_" + ath.MyPK + "_" + this.HisEn.PKVal;
                            mybtn.Click += new EventHandler(btnUpload_Click);
                            mybtn.CssClass = "bg";
                            if (athDB == null)
                                mybtn.Visible = false;
                            else
                                mybtn.Visible = true;
                            this.Add(mybtn);


                        }
                        if (ath.IsWoEnableWF)
                        {
                            mybtn = new Button();
                            mybtn.CssClass = "Btn";
                            mybtn.Text = "编辑";
                            mybtn.ID = "Btn_Open_" + ath.MyPK + "_" + this.HisEn.PKVal;
                            mybtn.Click += new EventHandler(btnUpload_Click);

                            mybtn.CssClass = "bg";
                            if (athDB == null)
                                mybtn.Visible = false;
                            else
                                mybtn.Visible = true;
                            this.Add(mybtn);
                        }
                    }
                    this.Add("</DIV>");
                    this.Add("</DIV>");
                    this.Add("</DIV>");
                }

                if (ath.UploadType == AttachmentUploadType.Multi)
                {
                    x = ath.X + wtX;
                    this.Add("<DIV id='Fd" + ath.MyPK + "' style='position:absolute; left:" + x + "px; top:" + ath.Y + "px; width:" + ath.W + "px; height:" + ath.H + "px;text-align: left;' >");
                    this.Add("<span>");
                    string src = "";
                    if (this.IsReadonly)
                        src = appPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal.ToString() + "&Ath=" + ath.NoOfObj + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1" + this.RequestParas;
                    else
                        src = appPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal.ToString() + "&Ath=" + ath.NoOfObj + "&FK_FrmAttachment=" + ath.MyPK + this.RequestParas;

                    this.Add("<iframe ID='F" + ath.MyPK + "'    src='" + src + "' frameborder=0  style='position:absolute;width:" + ath.W + "px; height:" + ath.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto></iframe>");
                    this.Add("</span>");
                    this.Add("</DIV>");
                }
            }
            #endregion 输出附件.

            #region 输出 img 附件
            FrmImgAths imgAths = this.mapData.FrmImgAths;
            if (imgAths.Count != 0 && this.IsReadonly == false)
            {
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function ImgAth(url,athMyPK)";
                js += "\t\n {";
                js += "\t\n  var v= window.showModalDialog(url, 'ddf', 'dialogHeight: 650px; dialogWidth: 950px;center: yes; help: no'); ";
                js += "\t\n  if (v==null )  ";
                js += "\t\n     return ;";
                js += "\t\n document.getElementById('Img'+athMyPK ).setAttribute('src', v); ";
                js += "\t\n }";
                js += "\t\n</script>";
                this.Add(js);
            }

            foreach (FrmImgAth ath in imgAths)
            {
                x = ath.X + wtX;
                this.Add("\t\n<DIV id=" + ath.MyPK + " style='position:absolute;left:" + x + "px;top:" + ath.Y + "px;text-align:left;vertical-align:top' >");
                string url = appPath + "WF/CCForm/ImgAth.aspx?W=" + ath.W + "&H=" + ath.H + "&FK_MapData=" + enName + "&MyPK=" + en.PKVal + "&ImgAth=" + ath.MyPK;
                if (isReadonly == false && ath.IsEdit == true)
                    this.AddFieldSet("<a href=\"javascript:ImgAth('" + url + "','" + ath.MyPK + "');\" >编辑</a>");

                FrmImgAthDB imgAthDb = new FrmImgAthDB();
                imgAthDb.MyPK = ath.MyPK + "_" + en.PKVal;
                imgAthDb.RetrieveFromDBSources();
                if (imgAthDb != null && !string.IsNullOrEmpty(imgAthDb.FileName))
                    this.Add("\t\n<img src='" + appPath + "DataUser/ImgAth/Data/" + imgAthDb.FileName + ".png' onerror=\"this.src='" + appPath + "WF/Data/Img/LogH.PNG'\" name='Img" + ath.MyPK + "' id='Img" + ath.MyPK + "' style='padding: 0px;margin: 0px;border-width: 0px;' width=" + ath.W + " height=" + ath.H + " />");
                else
                    this.Add("\t\n<img src='" + appPath + "DataUser/ImgAth/Data/" + ath.MyPK + "_" + en.PKVal + ".png' onerror=\"this.src='" + appPath + "WF/Data/Img/LogH.PNG'\" name='Img" + ath.MyPK + "' id='Img" + ath.MyPK + "' style='padding: 0px;margin: 0px;border-width: 0px;' width=" + ath.W + " height=" + ath.H + " />");

                if (isReadonly == false && ath.IsEdit == true)
                    this.AddFieldSetEnd();
                this.Add("\t\n</DIV>");
            }
            #endregion 输出附件.

            // 处理扩展.
            if (isReadonly == false)
                this.AfterBindEn_DealMapExt(enName, mattrs, en);
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="method"></param>
        void AddLoadFunction(string id, string eventName, string method)
        {
            string js = "";
            js = "\t\n<script type='text/javascript' >";
            js += "\t\n function " + id + "load() { ";
            js += "\t\n  if (document.all) {";
            js += "\t\n     document.getElementById('F" + id + "').attachEvent('on" + eventName + "',function(event){" + method + "('" + id + "');});";
            js += "\t\n } ";

            js += "\t\n else { ";
            js += "\t\n  document.getElementById('F" + id + "').contentWindow.addEventListener('" + eventName + "',function(event){" + method + "('" + id + "');}, false); ";
            js += "\t\n } }";

            js += "\t\n</script>";
            this.Add(js);

        }
        void btnUpload_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string[] ids = btn.ID.Split('_');
            //string athPK = ids[2] + "_" + ids[3] ;

            string doType = ids[1];

            string athPK = btn.ID.Replace("Btn_" + doType + "_", "");
            athPK = athPK.Substring(0, athPK.LastIndexOf('_'));

            string athDBPK = athPK + "_" + this.HisEn.PKVal.ToString();
            FrmAttachment frmAth = new FrmAttachment();
            frmAth.MyPK = athPK;
            frmAth.RetrieveFromDBSources();

            string pkVal = this.HisEn.PKVal.ToString();
            switch (doType)
            {
                case "Delete":
                    FrmAttachmentDB db = new FrmAttachmentDB();
                    db.MyPK = athDBPK;
                    int id = db.Delete();
                    if (id == 0)
                        throw new Exception("@没有删除成功.");
                    try
                    {
                        Button btnDel = this.GetButtonByID("Btn_Delete_" + athDBPK);
                        btnDel.Visible = false;

                        btnDel = this.GetButtonByID("Btn_Download_" + athDBPK);
                        btnDel.Visible = false;

                        btnDel = this.GetButtonByID("Btn_Open_" + athDBPK);
                        btnDel.Visible = false;
                    }
                    catch
                    {

                    }
                    Label lab1 = this.GetLabelByID("Lab" + frmAth.MyPK);
                    lab1.Text = "";
                    break;
                case "Upload":
                    FileUpload fu = this.FindControl(athPK) as FileUpload;
                    if (fu.HasFile == false || fu.FileName.Length <= 2)
                    {
                        this.Alert("请选择上传的文件.");
                        return;
                    }

                    //检查格式是否符合要求.
                    if (frmAth.Exts == "" || frmAth.Exts == "*.*")
                    {
                        /*任何格式都可以上传.*/
                    }
                    else
                    {
                        string fileExt = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
                        fileExt = fileExt.ToLower().Replace(".", "");
                        if (frmAth.Exts.ToLower().Contains(fileExt) == false)
                        {
                            this.Alert("您上传的文件格式不符合要求,要求格式为:" + frmAth.Exts);
                            return;
                        }
                    }

                    if (fu.PostedFile.ContentLength / 1024 / 1024 > 30)
                    {
                        this.Alert("您上传的文件超过30M,请使用大附件上传!");
                        return;
                    }
                    //处理保存路径.
                    string saveTo = frmAth.SaveTo;



                    if (saveTo.Contains("*") || saveTo.Contains("@"))
                    {
                        /*如果路径里有变量.*/
                        saveTo = saveTo.Replace("*", "@");
                        saveTo = BP.WF.Glo.DealExp(saveTo, this.HisEn, null);
                    }

                    try
                    {
                        saveTo = Server.MapPath("~/" + saveTo);
                    }
                    catch (Exception)
                    {
                        //saveTo = saveTo;
                    }

                    if (System.IO.Directory.Exists(saveTo) == false)
                        System.IO.Directory.CreateDirectory(saveTo);

                    saveTo = saveTo + "\\" + athDBPK + "." + fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
                    fu.SaveAs(saveTo);

                    FileInfo info = new FileInfo(saveTo);

                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = athDBPK;
                    dbUpload.FK_FrmAttachment = athPK;
                    dbUpload.RefPKVal = this.HisEn.PKVal.ToString();
                    if (this.EnName == null)
                        dbUpload.FK_MapData = this.HisEn.ToString();
                    else
                        dbUpload.FK_MapData = this.EnName;

                    dbUpload.FileExts = info.Extension;

                    #region 处理文件路径，如果是保存到数据库，就存储pk.
                    if (frmAth.SaveWay == 0)
                    {
                        //文件方式保存
                        dbUpload.FileFullName = saveTo;
                    }

                    if (frmAth.SaveWay == 1)
                    {
                        //保存到数据库
                        dbUpload.FileFullName = dbUpload.MyPK;
                    }
                    #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                    dbUpload.FileName = fu.FileName;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.Rec = WebUser.No;
                    dbUpload.RecName = WebUser.Name;
                    dbUpload.RDT = BP.DA.DataType.CurrentDataTime;

                    if (this.Request.QueryString["FK_Node"] == null)
                    {
                        if (this.Request.QueryString["FK_Flow"] != null)
                        {
                            dbUpload.NodeID = this.Request.QueryString["FK_Flow"] + "01";
                        }
                    }
                    else
                    {
                        dbUpload.NodeID = this.Request.QueryString["FK_Node"];
                    }

                    dbUpload.Save();

                    // 执行文件保存到数据库.
                    if (frmAth.SaveWay == 1)
                        BP.DA.DBAccess.SaveFileToDB(saveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");


                    Button myBtnDel = this.GetButtonByID("Btn_Delete_" + athDBPK);
                    if (myBtnDel != null)
                    {
                        myBtnDel.Visible = true;
                        myBtnDel = this.GetButtonByID("Btn_Download_" + athDBPK);
                        myBtnDel.Visible = true;
                    }

                    Button myBtnOpen = this.GetButtonByID("Btn_Open_" + athDBPK);

                    if (myBtnOpen != null)
                    {
                        myBtnOpen.Visible = true;
                        myBtnOpen = this.GetButtonByID("Btn_Download_" + athDBPK);
                        myBtnOpen.Visible = true;
                    }

                    Label lab = this.GetLabelByID("Lab" + frmAth.MyPK);
                    if (lab != null)
                    {
                        if (frmAth.IsWoEnableWF)
                            lab.Text = "<a href=\"javascript:OpenOfiice('" + dbUpload.FK_FrmAttachment + "','" + this.HisEn.GetValStrByKey("OID") + "','" + dbUpload.MyPK + "','" + this.FK_MapData + "','" + frmAth.NoOfObj + "','" + this.FK_Node + "')\"><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + dbUpload.FileExts + ".gif' border=0/>" + dbUpload.FileName + "</a>";
                        else
                            lab.Text = "<img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/FileType/" + dbUpload.FileExts + ".gif' border=0/>" + dbUpload.FileName;
                    }
                    return;
                case "Download":
                    FrmAttachmentDB dbDown = new FrmAttachmentDB();
                    dbDown.MyPK = athDBPK;
                    if (dbDown.RetrieveFromDBSources() == 0)
                    {
                        dbDown.Retrieve(FrmAttachmentDBAttr.FK_MapData, this.HisEn.ClassID,
                            FrmAttachmentDBAttr.RefPKVal, this.HisEn.PKVal.ToString(), FrmAttachmentDBAttr.FK_FrmAttachment, frmAth.FK_MapData + "_" + frmAth.NoOfObj);
                    }
                    string downPath = GetRealPath(dbDown.FileFullName);
                    PubClass.DownloadFile(downPath, dbDown.FileName);
                    break;
                case "Open":
                    var url = CCFlowAppPath + "WF/WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=" + athDBPK + "&FK_FrmAttachment=" + frmAth.MyPK + "&PKVal=" + pkVal + "&FK_Node=" + this.HisEn.GetValStringByKey("FK_Node") + "&FK_MapData=" + frmAth.FK_MapData + "&NoOfObj=" + frmAth.NoOfObj;
                    PubClass.WinOpen(url, "WebOffice编辑", 850, 600);
                    break;
                default:
                    break;
            }
        }

        private string GetRealPath(string fileFullName)
        {
            bool isFile = false;
            string downpath = "";
            try
            {
                //如果相对路径获取不到可能存储的是绝对路径
                FileInfo downInfo = new FileInfo(Server.MapPath("~/" + fileFullName));
                isFile = true;
                downpath = Server.MapPath("~/" + fileFullName);
            }
            catch (Exception)
            {
                FileInfo downInfo = new FileInfo(fileFullName);
                isFile = true;
                downpath = fileFullName;
            }
            if (!isFile)
            {
                throw new Exception("没有找到下载的文件路径！");
            }

            return downpath;
        }

        bool CanEditor(string fileType)
        {
            try
            {
                string fileTypes = BP.Sys.SystemConfig.AppSettings["OpenTypes"];

                if (fileTypes.Contains(fileType))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        void myBtn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            FrmBtn mybtn = new FrmBtn(btn.ID);
            string doc = mybtn.EventContext.Replace("~", "'");

            Attrs attrs = this.HisEn.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                doc = doc.Replace("@" + attr.Key, this.HisEn.GetValStrByKey(attr.Key));
            }
            doc = doc.Replace("@FK_Dept", WebUser.FK_Dept);
            doc = doc.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            doc = doc.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            doc = doc.Replace("@WebUser.No", WebUser.No);
            doc = doc.Replace("@WebUser.Name", WebUser.Name);
            doc = doc.Replace("@MyPK", this.HisEn.PKVal.ToString());

            #region 处理两个变量.
            string alertMsgErr = mybtn.MsgErr;
            string alertMsgOK = mybtn.MsgOK;
            if (alertMsgOK.Contains("@"))
            {
                foreach (Attr attr in attrs)
                    alertMsgOK = alertMsgOK.Replace("@" + attr.Key, this.HisEn.GetValStrByKey(attr.Key));
            }

            if (alertMsgErr.Contains("@"))
            {
                foreach (Attr attr in attrs)
                    alertMsgErr = alertMsgErr.Replace("@" + attr.Key, this.HisEn.GetValStrByKey(attr.Key));
            }
            #endregion 处理两个变量.

            try
            {
                switch (mybtn.HisBtnEventType)
                {
                    case BtnEventType.RunSQL:
                        DBAccess.RunSQL(doc);
                        this.Alert(alertMsgOK);
                        return;
                    case BtnEventType.RunSP:
                        DBAccess.RunSP(doc);
                        this.Alert(alertMsgOK);
                        return;
                    case BtnEventType.RunURL:
                        doc = doc.Replace("@AppPath", BP.WF.Glo.CCFlowAppPath);
                        string text = DataType.ReadURLContext(doc, 800, System.Text.Encoding.UTF8);
                        if (text != null && text.Substring(0, 7).Contains("Err"))
                            throw new Exception(text);
                        alertMsgOK += text;
                        this.Alert(alertMsgOK);
                        return;
                    default:
                        throw new Exception("没有处理的执行类型:" + mybtn.HisBtnEventType);
                }
            }
            catch (Exception ex)
            {
                this.Alert(alertMsgErr + ex.Message);
            }

            #region 处理按钮事件。
            #endregion
        }
        #endregion

        public static string GetRefstrs(string keys, Entity en, Entities hisens)
        {
            string refstrs = "";
            string appPath = BP.WF.Glo.CCFlowAppPath;// System.Web.HttpContext.Current.Request.ApplicationPath;
            int i = 0;

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    //  string url = path + "/Comm/UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    try
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                        }
                        catch
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
                        throw ex;
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinShowModalDialog('" + url + "','onVsM'); \"  >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href=\"javascript:WinShowModalDialog('" + url + "','onVsM'); \"  >" + vsM.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入他门的 方法
            RefMethods myreffuncs = en.EnMap.HisRefMethods;
            if (myreffuncs.Count > 0)
            {
                foreach (RefMethod func in myreffuncs)
                {
                    if (func.Visable == false)
                        continue;

                    // string url = path + "/Comm/RefMethod.aspx?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                    string url = appPath + "WF/Comm/RefMethod.aspx?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                    if (func.Warning == null)
                    {
                        if (func.Target == null)
                            refstrs += "[" + func.GetIcon(appPath) + "<a href='" + url + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                        else
                            refstrs += "[" + func.GetIcon(appPath) + "<a href=\"javascript:WinOpen('" + url + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                    }
                    else
                    {
                        if (func.Target == null)
                            refstrs += "[" + func.GetIcon(appPath) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { window.location.href='" + url + "' }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                        else
                            refstrs += "[" + func.GetIcon(appPath) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { WinOpen('" + url + "','" + func.Target + "') }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                    }
                }
            }
            #endregion

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            //  string path = this.Request.ApplicationPath;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    //string url = path + "/Comm/UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&Key=" + enDtl.RefKey + "&Val=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;
                    string url = appPath + "Comm/UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                    try
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                        }
                        catch
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        enDtl.Ens.GetNewEntity.CheckPhysicsTable();
                        throw ex;
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "', 'dtl" + enDtl.RefKey + "'); \" >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "', 'dtl" + enDtl.RefKey + "'); \"  >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            return refstrs;
        }
        public UCEn()
        {
        }
        public void AddContral()
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% ></td>"));
            this.Controls.Add(new LiteralControl("<td></TD>"));
        }
        public void AddContral(string desc, CheckBox cb)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td>"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }
        public void AddContral(string desc, CheckBox cb, int colspan)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td  colspan='" + colspan + "'>"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }
        //		public void AddContral(string desc, string val)
        public void AddContral(string desc, string val)
        {
            this.Add("<TD class='FDesc' > " + desc + "</TD>");
            this.Add("<TD>" + val + "</TD>");
        }
        public void AddContral(string desc, TB tb, string helpScript)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=500px;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            tb.Attributes["Width"] = "80%";

            this.Add("<td class='FDesc' nowrap width=1% >" + desc + "</td>");
            this.Add("<td >" + helpScript);
            this.Add(tb);
            this.AddTDEnd();
        }
        public void AddContral(string desc, TB tb, string helpScript, int colspan)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }
            this.Add("<td class='FDesc' nowrap width=1% >" + desc + "</td>");
            if (colspan < 3)
            {
                this.Add("<td  colspan=" + colspan + " width='30%' >" + helpScript);
            }
            else
            {
                this.Add("<td  colspan=" + colspan + " width='80%' >" + helpScript);
            }
            this.Add(tb);
            this.AddTDEnd(); // ("</td>");
        }
        public void AddContral(string desc, TB tb, int colSpanOfCtl)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb, colSpanOfCtl);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td>");

            if (colSpanOfCtl < 3)
                this.Add("<td  colspan=" + colSpanOfCtl + " width='30%' >");
            else
                this.Add("<td  colspan=" + colSpanOfCtl + " width='80%' >");

            this.Add(tb);
            this.AddTDEnd();
        }
        /// <summary>
        /// 增加空件
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="tb"></param>
        public void AddContral(string desc, TB tb)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            //if (tb.ReadOnly == false)
            //    desc += "<font color=red><b>*</b></font>";

            tb.Attributes["style"] = "width=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td>");

            this.Add("<td  width='30%'>");
            this.Add(tb);
            this.AddTDEnd(); // ("</td>");
        }
        //		public void AddContralDoc(string desc, TB tb)
        public void AddContralDoc(string desc, TB tb)
        {
            //if (desc.Length>
            this.Add("<td class='FDesc'  colspan='2' nowrap height='100px' width='50%' >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.Attributes["Class"] = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }
        public void AddContralDoc(string desc, TB tb, int colspanOfctl)
        {
            //if (desc.Length>
            this.Add("<td class='FDesc'  colspan='" + colspanOfctl + "' nowrap height='100px' width='50%' >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.Attributes["Class"] = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }
        public void AddContralDoc(string desc, int colspan, TB tb)
        {
            this.Add("<td class='FDesc'  colspan='" + colspan + "' nowrap width=1%  height='100px'  >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.EnsName = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }

        private string ReplaceParamValue(string val)
        {
            if (string.IsNullOrEmpty(val)) return "";

            return val.Replace("@FK_Flow", Request.QueryString["FK_Flow"])
                .Replace("@FK_Node", Request.QueryString["FK_Node"])
                .Replace("@WorkID", Request.QueryString["WorkID"] ?? "")
                .Replace("@FID", Request.QueryString["FID"] ?? "")
                .Replace("@WebUser.No", WebUser.No)
                .Replace("@WebUser.Name", WebUser.Name)
                .Replace("@WebUser.FK_Dept", WebUser.FK_Dept)
                .Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
        }

        #region 方法
        public bool IsReadonly
        {
            get
            {
                string s = this.ViewState["IsReadonly"] as string;
                if (s == "1")
                    return true;
                return false;
            }
            set
            {
                if (value)
                    ViewState["IsReadonly"] = "1";
                else
                    ViewState["IsReadonly"] = "0";
            }
        }
        public bool IsShowDtl
        {
            get
            {
                return (bool)this.ViewState["IsShowDtl"];
            }
            set
            {
                ViewState["IsShowDtl"] = value;
            }
        }
        public void SetValByKey(string key, string val)
        {
            TB tb = new TB();
            tb.ID = "TB_" + key;
            tb.Text = val;
            tb.Visible = false;
            this.Controls.Add(tb);
        }
        public object GetValByKey(string key)
        {
            TB en = (TB)this.FindControl("TB_" + key);
            return en.Text;
        }

        public void BindReadonly(Entity en)
        {
            this.HisEn = en;
            //this.IsReadonly = isReadonly;
            //this.IsShowDtl = isShowDtl;
            this.Attributes["visibility"] = "hidden";
            this.Controls.Clear();
            this.AddTable(); //("<table   width='100%' id='AutoNumber1'  border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (isLeft && isAddTR)
                {
                    this.Add("<tr>");
                }
                isAddTR = true;
                val = en.GetValByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.AddContral(attr.Desc, val.ToString().ToString());
                        isAddTR = false;
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;
                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {
                            this.SetValByKey(attr.Key, val.ToString());
                            continue;
                        }
                        else
                        {

                            if (attr.UIHeight != 0)
                            {
                                this.AddContral(attr.Desc, val.ToString());
                            }
                            else
                            {

                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        //this.AddContral(attr.Desc, val.ToString().ToString("0.00")  );
                                        break;
                                    default:
                                        this.AddContral(attr.Desc, val.ToString());
                                        break;
                                }
                            }
                        }

                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    if (en.GetValBooleanByKey(attr.Key))
                        this.AddContral(attr.Desc, "是");
                    else
                        this.AddContral(attr.Desc, "否");
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    this.AddContral(attr.Desc, val.ToString());
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    //					Sys.SysEnums enums = new BP.Sys.SysEnums(attr.UIBindKey); 
                    //					foreach(SysEnum en in enums)
                    //					{
                    //						return ;
                    //					}
                }

                if (isLeft == false)
                    this.AddTREnd();

                isLeft = !isLeft;
            } // 结束循环.

            this.Add("</TABLE>");



            if (en.IsExit(en.PK, en.PKVal) == false)
                return;

            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
                refstrs += "<hr>";
            this.Add(refstrs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        /// <param name="isReadonly"></param>
        /// <param name="isShowDtl"></param>
        private void btn_Click(object sender, EventArgs e)
        {
        }

        public Entity GetEnData(Entity en)
        {
            try
            {

                string s = null;
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (attr.Key == "MyNum")
                    {
                        en.SetValByKey(attr.Key, 1);
                        continue;
                    }

                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            if (attr.UIVisible)
                            {
                                if (attr.UIHeight == 0)
                                {
                                    // 处理特殊字符.
                                    s = this.GetTBByID("TB_" + attr.Key).Text;
                                    en.SetValByKey(attr.Key, s);
                                    continue;
                                }
                                else
                                {
                                    if (this.IsExit("TB_" + attr.Key))
                                    {
                                        // 处理特殊字符.
                                        s = this.GetTBByID("TB_" + attr.Key).Text;
                                        en.SetValByKey(attr.Key, s);
                                        continue;
                                    }

                                    if (this.IsExit("TBH_" + attr.Key))
                                    {
                                        HtmlInputHidden input = (HtmlInputHidden)this.FindControl("TBH_" + attr.Key);
                                        en.SetValByKey(attr.Key, input.Value);
                                        continue;
                                    }

                                    if (this.IsExit("TBF_" + attr.Key))
                                    {
                                        //FredCK.FCKeditorV2.FCKeditor fck = (FredCK.FCKeditorV2.FCKeditor)this.FindControl("TB_" + attr.Key);
                                        //en.SetValByKey(attr.Key, fck.Value);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                en.SetValByKey(attr.Key, this.GetValByKey(attr.Key));
                            }
                            break;
                        case UIContralType.DDL:
                            en.SetValByKey(attr.Key, this.GetDDLByKey("DDL_" + attr.Key).SelectedItem.Value);
                            break;
                        case UIContralType.CheckBok:
                            en.SetValByKey(attr.Key, this.GetCBByKey("CB_" + attr.Key).Checked);
                            break;
                        case UIContralType.RadioBtn:
                            if (attr.IsEnum)
                            {
                                SysEnums ses = new SysEnums(attr.UIBindKey);
                                foreach (SysEnum se in ses)
                                {
                                    string id = "RB_" + attr.Key + "_" + se.IntKey;
                                    RadioButton rb = this.GetRBLByID(id);
                                    if (rb != null && rb.Checked)
                                    {
                                        en.SetValByKey(attr.Key, se.IntKey);
                                        break;
                                    }
                                }
                            }
                            if (attr.MyFieldType == FieldType.FK)
                            {
                                Entities ens = BP.En.ClassFactory.GetEns(attr.UIBindKey);
                                ens.RetrieveAll();
                                foreach (Entity enNoName in ens)
                                {
                                    RadioButton rb = this.GetRBLByID(attr.Key + "_" + enNoName.GetValStringByKey(attr.UIRefKeyValue));
                                    if (rb != null && rb.Checked)
                                    {
                                        en.SetValByKey(attr.Key, enNoName.GetValStrByKey(attr.UIRefKeyValue));
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetEnData error :" + ex.Message);
            }
            return en;
        }

        public DDL GetDDLByKey(string key)
        {
            return (DDL)this.FindControl(key);
        }
        //		public CheckBox GetCBByKey(string key)
        public CheckBox GetCBByKey(string key)
        {
            return (CheckBox)this.FindControl(key);
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (this.IsPostBack)
            {
                //	this.Bind(this.HisEn,this.IsReadonly,this.IsShowDtl) ;
            }
        }
        public Entity HisEn = null;
        public static string GetRefstrs1(string keys, Entity en, Entities hisens)
        {
            string refstrs = "";

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href='" + url + "'  >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "'  >" + vsM.Desc + "-" + i + "</a>]";

                }
            }
            #endregion

            #region 加入他门的相关功能
            //			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
            //			if ( reffuncs.Count > 0  )
            //			{
            //				foreach(SysUIEnsRefFunc en1 in reffuncs)
            //				{
            //					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
            //					refstrs+="[<a href='"+url+"' >"+en1.Name+"</a>]";
            //				}
            //			}
            #endregion

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    string url = "UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href='" + url + "'  >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "'  >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            return refstrs;
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		设计器支持所需的方法 - 不要使用代码编辑器
        ///		修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

    }
}
