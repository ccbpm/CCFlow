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
using BP;
using BP.WF.Template;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_MapDef : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 是不是第一次进入，如果是就需要执行一个检查方法。
        /// </summary>
        public bool IsFirst
        {
            get
            {
                if (this.Request.QueryString["IsFirst"] == null)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                string key = this.Request.QueryString["MyPK"];
                if (key == null)
                    key = this.Request.QueryString["PK"];
                if (key == null)
                    key = this.Request.QueryString["FK_MapData"];
                if (key == null)
                    key = "ND1601";
                return key;
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.MyPK;
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
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
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                if (this.FK_Flow == null)
                    return 0;
                return int.Parse(this.FK_MapData.Replace("ND", ""));
            }
        }
        #endregion 属性.

        public MapFoolForm md = null;
        protected void Page_Load(object sender, EventArgs e)
        {

            string fk_node = this.Request.QueryString["FK_Node"];
            md = new MapFoolForm(this.FK_MapData);

            //如果是第一次进入，就执行旧版本的升级检查.
            if (this.IsFirst == true)
            {
                MapFoolForm cols = new MapFoolForm(this.FK_MapData);
                cols.DoCheckFixFrmForUpdateVer();
                this.Response.Redirect("Designer.aspx?FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "&MyPK=" + this.MyPK + "&IsEditMapData=" + this.IsEditMapData, true);
                return;
            }

            MapAttrs mattrs = new MapAttrs(md.No);
            int count = mattrs.Count;

            #region 计算出来列的宽度.
            //int labCol = 50;
           // int ctrlCol = 300;
          //  int width = (labCol + ctrlCol) * md.TableCol / 2;
         //   int width = md.FrmW; // (labCol + ctrlCol) * md.TableCol / 2;
            #endregion 计算出来列的宽度.



            this.Pub1.Add("<div style='width:" + md.TableWidth + ";height:" + md.TableHeight + ";background-color:white;border:1px solid #666;padding:5px;' align='center'>");
            this.Pub1.Add("\t\n<Table style='width:98%;border:1px;padding:5px; padding-top:11px;' >");

            int myidx = 0;
            string src = "";

            /*
             * 根据 GroupField 循环出现菜单。
             */
            foreach (GroupField gf in gfs)
            {
                #region 首先判断是否是框架分组？
                switch (gf.CtrlType)
                {
                    case GroupCtrlType.Frame: // 框架 类型.
                        #region 框架
                        foreach (MapFrame fram in frams)
                        {
                            if (fram.MyPK != gf.CtrlID)
                                continue;

                            fram.IsUse = true;
                             myidx = rowIdx + 20;
                            this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                            this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 /><a href=\"javascript:EditFrame('" + this.FK_MapData + "','" + fram.MyPK + "')\" >" + fram.Name + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                            this.Pub1.AddTREnd();

                            myidx++;
                            this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                            this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TD" + fram.MyPK + "'  >");

                            src = fram.URL;
                            if (src.Contains("?"))
                                src += "&FK_Node=" + this.RefNo + "&WorkID=" + this.RefOID;
                            else
                                src += "?FK_Node=" + this.RefNo + "&WorkID=" + this.RefOID;

                            this.Pub1.Add("<iframe ID='F" + fram.MyPK + "' frameborder=0 style='padding:0px;border:0px;width:100%;height:" + fram.H + "'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto'  /></iframe>");
                            this.Pub1.AddTDEnd();
                            this.Pub1.AddTREnd();
                        }
                        #endregion 框架
                        continue;
                    case GroupCtrlType.Dtl : //增加从表.
                        #region 增加从表
                        foreach (MapDtl dtl in dtls)
                        {
                            if (dtl.No != gf.CtrlID)
                                continue;
                            dtl.IsUse = true;
                            myidx = rowIdx + 10;

                            this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                            this.Pub1.Add("<TD colspan=" + md.TableCol + " class=GroupField  ><div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 /><a href=\"javascript:EditDtl('" + this.FK_MapData + "','" + dtl.No + "')\" >" + dtl.Name + "</a></div>  <div style='text-align:left; float:left'></div><div style='text-align:right; float:right'><a href=\"javascript:document.getElementById('F" + dtl.No + "').contentWindow.AddF('" + dtl.No + "');\"><img src='../../Img/Btn/New.gif' border=0/>插入列</a><a href=\"javascript:document.getElementById('F" + dtl.No + "').contentWindow.AddFGroup('" + dtl.No + "');\"><img src='../../Img/Btn/New.gif' border=0/>插入列组</a><a href=\"javascript:document.getElementById('F" + dtl.No + "').contentWindow.CopyF('" + dtl.No + "');\"><img src='../../Img/Btn/Copy.gif' border=0/>复制列</a><a href=\"javascript:document.getElementById('F" + dtl.No + "').contentWindow.HidAttr('" + dtl.No + "');\"><img src='../../Img/Btn/Copy.gif' border=0/>隐藏字段</a><a href=\"javascript:document.getElementById('F" + dtl.No + "').contentWindow.DtlMTR('" + dtl.No + "');\"><img src='../../Img/Btn/Copy.gif' border=0/>多表头</a> <a href='Action.aspx?FK_MapData=" + dtl.No + "' >从表事件</a>  <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div></td>");
                            this.Pub1.AddTREnd();

                            myidx++;
                            this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                            this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TD" + dtl.No + "'   > ");
                            src = "MapDtlDe.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&FK_MapDtl=" + dtl.No;
                            this.Pub1.Add("<iframe ID='F" + dtl.No + "' frameborder=0 style='padding:0px;border:0px;width:100%;height:" + dtl.H + "px;'  leftMargin='0'  topMargin='0' src='" + src + "'  scrolling='auto' /></iframe>");
                            this.Pub1.AddTDEnd();
                            this.Pub1.AddTREnd();
                        }
                        #endregion 增加从表
                        continue;
                    case GroupCtrlType.Ath: //增加附件.
                        #region 增加附件
                        foreach (FrmAttachment ath in this.aths)
                        {
                            if (ath.MyPK != gf.CtrlID)
                                continue;

                            ath.IsUse = true;

                            myidx = rowIdx + 10;

                            this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                            this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 /><a href=\"javascript:EditAth('" + this.FK_MapData + "','" + ath.NoOfObj + "')\" >" + ath.Name + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                            this.Pub1.AddTREnd();

                            myidx++;
                            this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                            this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TD" + ath.MyPK + "' height='" + ath.H + "px' width='100%' >");

                            src = "../../CCForm/AttachmentUpload.aspx?PKVal=0&Ath=" + ath.NoOfObj + "&FK_MapData=" + this.FK_MapData + "&FK_FrmAttachment=" + ath.MyPK;

                            this.Pub1.Add("<iframe ID='F" + ath.MyPK + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' width='100%' height='" + ath.H + "' scrolling=auto  /></iframe>");

                            this.Pub1.AddTDEnd();
                            this.Pub1.AddTREnd();
                        }
                        #endregion 增加附件
                        continue;
                    case GroupCtrlType.FWC: //审核组件.
                        #region 审核组件
                        FrmWorkCheck fwc = new FrmWorkCheck(this.FK_MapData);
                        if (fwc.HisFrmWorkCheckSta == FrmWorkCheckSta.Disable)
                        {
                            gf.Delete();
                            continue;
                        }
                        
                        myidx = rowIdx + 10;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 /><a href=\"javascript:EditFWC('" + fwc.NodeID + "')\" >" + fwc.FWCLab + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                        this.Pub1.AddTREnd();

                        myidx++;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TDFWC" + fwc.No + "' height='" + fwc.FWC_H + "px' width='100%' >");

                        src = "NodeFrmComponents.aspx?DoType=FWC&FK_MapData=" + fwc.NodeID;
                        this.Pub1.Add("<iframe ID='F" + gf.CtrlID + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' width='100%' height='" + fwc.FWC_H + "px' scrolling=auto  /></iframe>");
                        this.Pub1.AddTDEnd();
                        this.Pub1.AddTREnd();
                        #endregion  审核组件
                        continue;
                    case GroupCtrlType.SubFlow: //子流程..
                        #region 子流程.
                        FrmSubFlow subflow = new FrmSubFlow(this.FK_MapData);
                        if (subflow.HisFrmSubFlowSta == FrmSubFlowSta.Disable)
                        {
                            gf.Delete();
                            continue;
                        }

                        myidx = rowIdx + 10;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='ImgSub" + subflow.NodeID + "'  border=0 /><a href=\"javascript:EditSubFlow('" + subflow.NodeID + "')\" >" + subflow.SFLab + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                        this.Pub1.AddTREnd();

                        myidx++;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TDFWC" + subflow.No + "' height='" + subflow.SF_H + "px' width='100%' >");

                        src = "NodeFrmComponents.aspx?DoType=SubFlow&FK_MapData=" + subflow.NodeID;
                        this.Pub1.Add("<iframe ID='F" + gf.CtrlID + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' width='100%' height='" + subflow.SF_H + "px' scrolling=auto  /></iframe>");
                        this.Pub1.AddTDEnd();
                        this.Pub1.AddTREnd();
                        #endregion 子线程.
                        continue;
                    case GroupCtrlType.Track: //轨迹图.
                        #region 轨迹图.
                        FrmTrack track = new FrmTrack(this.FK_MapData);
                        if (track.FrmTrackSta == FrmTrackSta.Disable)
                        {
                            gf.Delete();
                            continue;
                        }

                        myidx = rowIdx + 10;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 /><a href=\"javascript:EditTrack('" + track.NodeID + "')\" >" + track.FrmTrackLab + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                        this.Pub1.AddTREnd();

                        myidx++;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TDFWC" + track.No + "' height='" + track.FrmTrack_H + "px' width='100%' >");

                        src = "NodeFrmComponents.aspx?DoType=FrmTrack&FK_MapData=" + track.NodeID;
                        this.Pub1.Add("<iframe ID='F" + gf.CtrlID + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' width='100%' height='" + track.FrmTrack_H + "px' scrolling=auto  /></iframe>");
                        this.Pub1.AddTDEnd();
                        this.Pub1.AddTREnd();
                        #endregion 轨迹图.
                        continue;
                    case GroupCtrlType.Thread: //子线程.
                        #region 子线程.

                        FrmThread thread = new FrmThread(this.FK_MapData);
                        if (thread.FrmThreadSta == FrmThreadSta.Disable)
                        {
                            gf.Delete();
                            continue;
                        }

                        myidx = rowIdx + 10;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 /><a href=\"javascript:EditThread('" + thread.NodeID + "')\" >" + thread.FrmThreadLab + "</a></div><div style='text-align:right; float:right'> <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                        this.Pub1.AddTREnd();

                        myidx++;
                        this.Pub1.AddTR(" ID='" + currGF.Idx + "_" + myidx + "' ");
                        this.Pub1.Add("<TD colspan=" + md.TableCol + " ID='TDFWC" + thread.No + "' height='" + thread.FrmThread_H + "px' width='100%' >");

                        src = "NodeFrmComponents.aspx?DoType=FrmThread&FK_MapData=" + thread.NodeID;
                        this.Pub1.Add("<iframe ID='F" + gf.CtrlID + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' width='100%' height='" + thread.FrmThread_H + "px' scrolling=auto  /></iframe>");
                        this.Pub1.AddTDEnd();
                        this.Pub1.AddTREnd();
                        #endregion 轨迹图.
                        continue;
                    default:
                        break;
                }
                #endregion

                rowIdx = 0;
                string gfAttr = "";
                currGF = gf;

                #region 输出分组栏.
                this.Pub1.AddTR(gfAttr);
                if (gfs.Count == 1)
                    this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top' align:left style='height: 24px;align:left' ", "<div style='text-align:left; float:left'>&nbsp;<a href=\"javascript:GroupField('" + this.FK_MapData + "','" + gf.OID + "')\" >" + gf.Lab + "</a></div><div style='text-align:right; float:right'></div>");
                else
                    this.Pub1.AddTD("colspan=" + md.TableCol + " class=GroupField valign='top'  style='height: 24px;align:left' ", "<div style='text-align:left; float:left'><img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 />&nbsp;<a href=\"javascript:GroupField('" + this.FK_MapData + "','" + gf.OID + "')\" >" + gf.Lab + "</a></div> <div style='text-align:right; float:right'>  <a href=\"javascript:AddField('" + gf.EnName + "','" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-new',plain:true\"> </a>  <a href=\"javascript:GFDoUp('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-up',plain:true\"> </a> <a href=\"javascript:GFDoDown('" + gf.OID + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-down',plain:true\"> </a></div>");
                this.Pub1.AddTREnd();
                #endregion 输出分组栏.

                this.idx = 0; // 设置字段的顺序号为0.

                //是否是沾满的状态.
                bool isLeft = true;
                for (int i = 0; i < mattrs.Count; i++)
                {
                    MapAttr attr = mattrs[i] as MapAttr;

                    #region 过滤不需要显示的字段.
                    if (attr.GroupID == 0)
                    {
                       // attr.GroupID = gf.OID;
                        attr.Update(MapAttrAttr.GroupID, gf.OID);
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
                  
                    #endregion 过滤不需要显示的字段.

                    /* 
                     * 以下就是一列标签一列控件的方式展现了.
                     */

                    #region 增加控件与描述.
                  
                    TB tb = new TB();
                    tb.Attributes["width"] = "100%";
                    tb.ID = "TB_" + attr.KeyOfEn;

                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:

                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:

                                    #region 大块文本的输出.
                                    if ( attr.IsBigDoc == true)
                                    {
                                        if (attr.ColSpan == 1)
                                        {
                                            if (isLeft == true)
                                            {
                                                // 如果是灌满的状态.
                                                this.Pub1.AddTR();
                                            }

                                            /*是大块文本，并且跨度在占领了整个剩余行单元格. */
                                            this.Pub1.Add("<TD colspan=" + md.TableCol + " width='100%' height='" + attr.UIHeight.ToString() + "px' >");
                                            this.Pub1.Add("<span style='float:left' height='" + attr.UIHeight.ToString() + "px' >" + this.GenerLab(attr, 0, count) + "</span>");
                                            this.Pub1.Add("<span style='float:right' height='" + attr.UIHeight.ToString() + "px'  >");

                                            Label lab = new Label();
                                            lab.ID = "Lab" + attr.KeyOfEn;
                                            lab.Text = "默认值";
                                            this.Pub1.Add(lab);
                                            this.Pub1.Add("</span><br>");

                                            TB mytbLine = new TB();
                                            mytbLine.ID = "TB_" + attr.KeyOfEn;
                                            mytbLine.TextMode = TextBoxMode.MultiLine;
                                            mytbLine.Attributes["style"] = "width:100%;height:100%;padding: 0px;margin: 0px;";
                                            mytbLine.Enabled = attr.UIIsEnable;
                                            this.Pub1.Add(mytbLine);
                                            mytbLine.Attributes["width"] = "100%";
                                            lab = this.Pub1.GetLabelByID("Lab" + attr.KeyOfEn);
                                            string ctlID = mytbLine.ClientID;
                                            lab.Text = "<a href=\"javascript:TBHelp('" + ctlID + "','" + this.Request.ApplicationPath + "','" + attr.KeyOfEn + "','" + md.No + "')\">默认值</a>";
                                            this.Pub1.AddTDEnd();

                                            if (isLeft == false)
                                            {
                                                this.Pub1.AddTREnd();
                                                isLeft = true;
                                            }
                                        }

                                        if (attr.ColSpan == 3 || attr.ColSpan == 4)
                                        {
                                            if (isLeft == false)
                                            {
                                                this.Pub1.AddTD("colspan=2", "");
                                                this.Pub1.AddTREnd();
                                                isLeft = true;
                                            }

                                            this.Pub1.AddTR();
                                            /*是大块文本，并且跨度在占领了整个剩余行单元格. */
                                            this.Pub1.Add("<TD colspan=" + md.TableCol + " width='100%' height='" + attr.UIHeight.ToString() + "px' >");
                                            this.Pub1.Add("<span style='float:left' height='" + attr.UIHeight.ToString() + "px' >" + this.GenerLab(attr, 0, count) + "</span>");
                                            this.Pub1.Add("<span style='float:right' height='" + attr.UIHeight.ToString() + "px'  >");

                                            Label lab = new Label();
                                            lab.ID = "Lab" + attr.KeyOfEn;
                                            lab.Text = "默认值";
                                            this.Pub1.Add(lab);
                                            this.Pub1.Add("</span><br>");

                                            TB mytbLine = new TB();
                                            mytbLine.ID = "TB_" + attr.KeyOfEn;
                                            mytbLine.TextMode = TextBoxMode.MultiLine;
                                            mytbLine.Attributes["style"] = "width:100%;height:100%;padding: 0px;margin: 0px;";
                                            mytbLine.Enabled = attr.UIIsEnable;
                                            this.Pub1.Add(mytbLine);
                                            mytbLine.Attributes["width"] = "100%";
                                            lab = this.Pub1.GetLabelByID("Lab" + attr.KeyOfEn);
                                            string ctlID = mytbLine.ClientID;
                                            lab.Text = "<a href=\"javascript:TBHelp('" + ctlID + "','" + this.Request.ApplicationPath + "','" + attr.KeyOfEn + "','" + md.No + "')\">默认值</a>";
                                            this.Pub1.AddTDEnd();
                                            this.Pub1.AddTREnd();
                                        }
                                        continue;
                                    }
                                    #endregion 大块文本的输出.

                                    #region 整行输出.
                                    if (attr.ColSpan == 3)
                                    {
                                        if (isLeft == false)
                                        {
                                            this.Pub1.AddTD();
                                            this.Pub1.AddTREnd();
                                            isLeft = true;
                                        }

                                        this.Pub1.AddTR();
                                        this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                        tb.ShowType = TBType.TB;
                                        tb.Text = attr.DefVal;

                                        if (attr.UIIsEnable == false)
                                            tb.CssClass = "TBReadonly";

                                        if (attr.IsSigan)
                                            this.Pub1.AddTD("colspan=3", "<img src='/DataUser/Siganture/" + WebUser.No + ".jpg'  style='border:0px;Width:70px;' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"/>");
                                        else
                                            this.Pub1.AddTD("colspan=3", tb);

                                        this.Pub1.AddTREnd();
                                        continue;
                                    }
                                    #endregion 整行输出.

                                    #region 单行输出.
                                    if (attr.ColSpan == 1)
                                    {
                                        if (isLeft == true)
                                            this.Pub1.AddTR();

                                        this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                        tb.ShowType = TBType.TB;
                                        tb.Text = attr.DefVal;

                                        if (attr.UIIsEnable == false)
                                            tb.CssClass = "TBReadonly";

                                        if (attr.IsSigan)
                                            this.Pub1.AddTD("colspan=1", "<img src='/DataUser/Siganture/" + WebUser.No + ".jpg'  style='border:0px;Width:70px;' onerror=\"this.src='../../DataUser/Siganture/UnName.jpg'\"/>");
                                        else
                                            this.Pub1.AddTD("colspan=1", tb);
                                    }
                                    #endregion 单行输出.

                                    break;
                                case BP.DA.DataType.AppDate:
                                    if (isLeft == true)
                                        this.Pub1.AddTR();

                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    TB tbD = new TB();
                                    tbD.Text = attr.DefVal;
                                    if (attr.UIIsEnable)
                                    {
                                        tbD.Attributes["onfocus"] = "WdatePicker();";
                                        tbD.Attributes["class"] = "TBcalendar";
                                    }
                                    else
                                    {
                                        tbD.Enabled = false;
                                        tbD.ReadOnly = true;
                                        tbD.Attributes["class"] = "TBcalendar";
                                    }
                                    this.Pub1.AddTD(" colspan=1", tbD);
                                    break;
                                case BP.DA.DataType.AppDateTime:

                                    if (isLeft == true)
                                        this.Pub1.AddTR();

                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    TB tbDT = new TB();
                                    tbDT.Text = attr.DefVal;
                                    if (attr.UIIsEnable)
                                    {
                                        tbDT.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                        tbDT.Attributes["class"] = "TBcalendar";
                                    }
                                    else
                                    {
                                        tbDT.Enabled = false;
                                        tbDT.ReadOnly = true;
                                        tbDT.Attributes["class"] = "TBcalendar";
                                    }
                                    this.Pub1.AddTD(" colspan=1", tbDT);
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    if (isLeft == true)
                                        this.Pub1.AddTR();

                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    CheckBox cb = new CheckBox();
                                    cb.Text = attr.Name;
                                    cb.Checked = attr.DefValOfBool;
                                    cb.Enabled = attr.UIIsEnable;
                                    cb.ID = "CB_" + attr.KeyOfEn;
                                    this.Pub1.AddTD(" colspan=1", cb);
                                    break;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppFloat:
                                case BP.DA.DataType.AppInt:
                                    if (isLeft == true)
                                        this.Pub1.AddTR();

                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    tb.ShowType = TBType.Num;
                                    tb.Text = attr.DefVal;
                                    if (attr.IsNull)
                                        tb.Text = "";
                                    this.Pub1.AddTD(" colspan=1", tb);
                                    break;
                                case BP.DA.DataType.AppMoney:
                                case BP.DA.DataType.AppRate:

                                    if (isLeft == true)
                                        this.Pub1.AddTR();

                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = attr.DefVal;
                                    if (attr.IsNull)
                                        tb.Text = "";
                                    this.Pub1.AddTD(" colspan=1", tb);
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
                            if (attr.UIContralType == UIContralType.DDL)
                            {
                                if (isLeft == true)
                                    this.Pub1.AddTR();
                                this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                DDL ddle = new DDL();
                                ddle.ID = "DDL_" + attr.KeyOfEn;
                                ddle.BindSysEnum(attr.UIBindKey);
                                ddle.SetSelectItem(attr.DefVal);
                                ddle.Enabled = attr.UIIsEnable;
                                this.Pub1.AddTD("colspan=" + attr.ColSpan, ddle);
                            }
                            else
                            {
                                if (attr.ColSpan == 1)
                                {
                                    if (isLeft == true)
                                        this.Pub1.AddTR();

                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    this.Pub1.AddTDBegin();
                                    SysEnums ses = new SysEnums(attr.UIBindKey);
                                    foreach (SysEnum item in ses)
                                    {
                                        RadioButton rb = new RadioButton();
                                        rb.ID = "RB_" + attr.KeyOfEn + "_" + item.IntKey;
                                        rb.Text = item.Lab;
                                        if (item.IntKey.ToString() == attr.DefVal)
                                            rb.Checked = true;
                                        else
                                            rb.Checked = false;
                                        rb.GroupName = item.EnumKey + attr.KeyOfEn;
                                        this.Pub1.Add(rb);
                                        if (attr.RBShowModel == 1)
                                            this.Pub1.AddBR();
                                    }
                                    this.Pub1.AddTDEnd();
                                }

                                if (attr.ColSpan == 3)
                                {
                                    if (isLeft == false)
                                    {
                                        /*补充空白行.*/
                                        this.Pub1.AddTD();
                                        this.Pub1.AddTD();
                                        this.Pub1.AddTREnd();
                                        isLeft = true;
                                    }

                                    this.Pub1.AddTR();
                                    this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
                                    this.Pub1.AddTDBegin("colspan=3");
                                    SysEnums ses = new SysEnums(attr.UIBindKey);
                                    foreach (SysEnum item in ses)
                                    {
                                        RadioButton rb = new RadioButton();
                                        rb.ID = "RB_" + attr.KeyOfEn + "_" + item.IntKey;
                                        rb.Text = item.Lab;
                                        if (item.IntKey.ToString() == attr.DefVal)
                                            rb.Checked = true;
                                        else
                                            rb.Checked = false;
                                        rb.GroupName = item.EnumKey + attr.KeyOfEn;
                                        this.Pub1.Add(rb);
                                        if (attr.RBShowModel == 1)
                                            this.Pub1.AddBR();
                                    }
                                    this.Pub1.AddTDEnd();
                                    this.Pub1.AddTREnd();
                                }
                            }

                            if (attr.ColSpan == 4)
                            {
                                if (isLeft == false)
                                {
                                    /*补充空白行.*/
                                    this.Pub1.AddTD();
                                    this.Pub1.AddTD();
                                    this.Pub1.AddTREnd();
                                    isLeft = true;
                                }

                                this.Pub1.AddTR();
                                this.Pub1.AddTDBegin("colspan=4");
                                this.Pub1.Add(this.GenerLab(attr, i, count));

                                SysEnums ses = new SysEnums(attr.UIBindKey);
                                foreach (SysEnum item in ses)
                                {
                                    RadioButton rb = new RadioButton();
                                    rb.ID = "RB_" + attr.KeyOfEn + "_" + item.IntKey;
                                    rb.Text = item.Lab;
                                    if (item.IntKey.ToString() == attr.DefVal)
                                        rb.Checked = true;
                                    else
                                        rb.Checked = false;
                                    rb.GroupName = item.EnumKey + attr.KeyOfEn;
                                    this.Pub1.Add(rb);
                                    if (attr.RBShowModel == 1)
                                        this.Pub1.AddBR();
                                }
                                this.Pub1.AddTDEnd();
                                this.Pub1.AddTREnd();
                            }
                            break;
                        case FieldTypeS.FK:
                            if (isLeft == true)
                                this.Pub1.AddTR();

                            this.Pub1.AddTDDesc(this.GenerLab(attr, i, count));
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
                            this.Pub1.AddTD("colspan=1", ddl1);
                            break;
                        default:
                            break;
                    }
                    #endregion 增加控件.

                    if (isLeft == false)
                    {
                        this.Pub1.AddTREnd();
                    }

                    isLeft = !isLeft;

                } // end循环字段分组.

                if (isLeft == false)
                {
                    this.Pub1.AddTD();
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                }

            } // end循环分组.
            
            this.Pub1.AddTableEnd();
            this.Pub1.AddDivEnd();

            #region 处理扩展信息。
            MapExts mes = new MapExts(this.FK_MapData);
            if (mes.Count != 0)
            {
                string appPath = this.Request.ApplicationPath;

                this.Page.RegisterClientScriptBlock("s",
              "<script language='JavaScript' src='../Scripts/jquery-1.4.1.min.js' ></script>");

                this.Page.RegisterClientScriptBlock("b",
             "<script language='JavaScript' src='../CCForm/MapExt.js' defer='defer' type='text/javascript' ></script>");

                this.Page.RegisterClientScriptBlock("dC",
         "<script language='JavaScript' src='" + appPath + "DataUser/JSLibData/" + this.FK_MapData + ".js' ></script>");

                this.Pub1.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");
            }

            foreach (MapExt me in mes)
            {
                switch (me.ExtType)
                {
                    case MapExtXmlList.DDLFullCtrl: // 自动填充.
                        DDL ddlOper = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper);
                        if (ddlOper == null)
                            continue;
                        ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                        break;
                    case MapExtXmlList.ActiveDDL:
                        DDL ddlPerant = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper);
                        DDL ddlChild = this.Pub1.GetDDLByID("DDL_" + me.AttrsOfActive);
                        if (ddlChild == null || ddlPerant == null)
                        {
                            me.Delete();
                            continue;
                        }

                        ddlPerant.Attributes["onchange"] = "DDLAnsc(this.value,\'" + ddlChild.ClientID + "\', \'" + me.MyPK + "\')";
                        // ddlChild.Attributes["onchange"] = "ddlCity_onchange(this.value,'" + me.MyPK + "')";
                        break;
                    case MapExtXmlList.TBFullCtrl: // 自动填充.
                        TB tbAuto = this.Pub1.GetTBByID("TB_" + me.AttrOfOper);
                        if (tbAuto == null)
                        {
                            me.Delete();
                            continue;
                        }
                        tbAuto.Attributes["onkeyup"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                        tbAuto.Attributes["AUTOCOMPLETE"] = "OFF";
                        break;
                    case MapExtXmlList.InputCheck: /*js 检查 */
                        TB tbJS = this.Pub1.GetTBByID("TB_" + me.AttrOfOper);
                        if (tbJS != null)
                            tbJS.Attributes[me.Tag2] += me.Tag1 + "(this);";
                        else
                            me.Delete();
                        break;
                    case MapExtXmlList.PopVal: //弹出窗.
                        TB tbPop = this.Pub1.GetTBByID("TB_" + me.AttrOfOper);
                        //tb.Attributes["ondblclick"] = "ReturnVal(this,'" + me.Doc + "','sd');";
                        break;
                    case MapExtXmlList.AutoFull: //自动填充.
                        string js = "\t\n <script type='text/javascript' >";
                        TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper);
                        if (tb == null)
                            continue;

                        string left = "\n  document.forms[0]." + tb.ClientID + ".value = ";
                        string right = me.Doc;
                        foreach (MapAttr mattr in mattrs)
                        {
                            if (mattr.IsNum == false)
                                continue;

                            if (me.Doc.Contains("@" + mattr.KeyOfEn)
                                || me.Doc.Contains("@" + mattr.Name))
                            {
                            }
                            else
                            {
                                continue;
                            }

                            string tbID = "TB_" + mattr.KeyOfEn;
                            TB mytb = this.Pub1.GetTBByID(tbID);
                            if (mytb == null)
                                continue;

                            this.Pub1.GetTBByID(tbID).Attributes["onkeyup"] = "javascript:Auto" + me.AttrOfOper + "();";
                            right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                            right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                        }

                        js += "\t\n function Auto" + me.AttrOfOper + "() { ";
                        js += left + right + ";";
                        js += " \t\n  document.forms[0]." + tb.ClientID + ".value= VirtyMoney(document.forms[0]." + tb.ClientID + ".value ) ;";
                        js += "\t\n } ";
                        js += "\t\n</script>";
                        this.Pub1.Add(js);
                        break;
                    default:
                        break;
                }
            }
            #endregion 处理扩展信息。

            #region 处理输入最小，最大验证.
            foreach (MapAttr attr in mattrs)
            {
                if (attr.MyDataType != DataType.AppString || attr.MinLen == 0)
                    continue;

                if (attr.UIIsEnable == false || attr.UIVisible == false)
                    continue;

                this.Pub1.GetTextBoxByID("TB_" + attr.KeyOfEn).Attributes["onblur"] = "checkLength(this,'" + attr.MinLen + "','" + attr.MaxLen + "')";
            }
            #endregion 处理输入最小，最大验证.

            #region 处理iFrom 的自适应的问题。
           // string myjs = "\t\n<script type='text/javascript' >";
            //foreach (MapDtl dtl in dtls)
            //{
            //    myjs += "\t\n window.setInterval(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
            //}

            //foreach (MapM2M M2M in dot2dots)
            //{
            //    if (M2M.ShowWay == FrmShowWay.FrmAutoSize)
            //        myjs += "\t\n window.setInterval(\"ReinitIframe('F" + M2M.NoOfObj + "','TD" + M2M.NoOfObj + "')\", 200);";
            //}
            //foreach (FrmAttachment ath in aths)
            //{
            //    if (ath.IsAutoSize)
            //        myjs += "\t\n window.setInterval(\"ReinitIframe('F" + ath.MyPK + "','TD" + ath.MyPK + "')\", 200);";
            //}
            //foreach (MapFrame fr in frams)
            //{
            //    myjs += "\t\n window.setInterval(\"ReinitIframe('F" + fr.MyPK + "','TD" + fr.MyPK + "')\", 200);";
            //}
            //myjs += "\t\n</script>";
            //this.Pub1.Add(myjs);
            #endregion 处理iFrom 的自适应的问题。

            #region 处理隐藏字段。
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.FK_MapData + "' AND GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.FK_MapData + "')");
            if (dt.Rows.Count != 0)
            {
                int gfid = gfs[0].GetValIntByKey("OID");
                foreach (DataRow dr in dt.Rows)
                    DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID=" + gfid + " WHERE MyPK='" + dr["MyPK"] + "'");

                this.Response.Redirect(this.Request.RawUrl);
            }
            #endregion 处理隐藏字段。
        }

        #region varable.
        private FrmAttachments _aths;
        public FrmAttachments aths
        {
            get
            {
                if (_aths == null)
                    _aths = new FrmAttachments(this.FK_MapData);
                return _aths;
            }
        }

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
        private MapFrames _frams;
        public MapFrames frams
        {
            get
            {
                if (_frams == null)
                    _frams = new MapFrames(this.FK_MapData);
                return _frams;
            }
        }
        private MapM2Ms _dot2dots;
        public MapM2Ms dot2dots
        {
            get
            {
                if (_dot2dots == null)
                    _dot2dots = new MapM2Ms(this.FK_MapData);
                return _dot2dots;
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

        
        /// <summary>
        /// 字段or控件的顺序号.
        /// </summary>
        public int idx = 0;
        /// <summary>
        /// 属性
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="idx"></param>
        /// <param name="i"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string GenerLab(MapAttr attr, int i, int count)
        {
            idx++;
            string divAttr = "";
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
                return "<div " + divAttr + " >" + lab + "<a href=\"javascript:Down('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-right',plain:true\" alt='向右动顺序'> </a></div>";
            }

            if (idx == count - 1)
            {
                /*到数第一个。*/
                return "<div " + divAttr + " ><a href=\"javascript:Up('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-left',plain:true\" alt='向左动顺序'> </a>" + lab + "</div>";
            }
            return "<div " + divAttr + " ><a href=\"javascript:Up('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-left',plain:true\" alt='向左动顺序'> </a>" + lab + "<a href=\"javascript:Down('" + this.FK_MapData + "','" + attr.MyPK + "','1');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-right',plain:true\" alt='向右动顺序'> </a></div>";
        }
    }

}