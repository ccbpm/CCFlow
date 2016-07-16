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
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_MapDtl : BP.Web.WebPage
    {
        #region 属性
        public new string DoType
        {
            get
            {
                string v = this.Request.QueryString["DoType"];
                if (v == null || v == "")
                    v = "New";
                return v;
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_MapDtl
        {
            get
            {
                return this.Request.QueryString["FK_MapDtl"];
            }
        }
        public int FK_Node
        {
            get
            {
                if (this.FK_MapData.Contains("ND")==false)
                    return 0;
                try
                {
                    string nodeid = this.FK_MapData.Replace("ND", "");
                    return int.Parse(nodeid);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            this.Title = md.Name + " - 设计明细";
            switch (this.DoType)
            {
                case "Edit":
                    MapDtl dtl = new MapDtl();

                    if (this.FK_MapDtl == null)
                    {
                        dtl.No = this.FK_MapData + "Dtl";
                    }
                    else
                    {
                        dtl.No = this.FK_MapDtl;
                        dtl.Retrieve();
                    }
                    BindEdit(md, dtl);
                    break;
                default:
                case "New":
                    int num = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(No) FROM Sys_MapDtl WHERE FK_MapData='" + this.FK_MapData + "'") + 1;
                    MapDtl dtl1 = new MapDtl();
                    dtl1.Name = "从表" + num;
                    dtl1.No = this.FK_MapData + "Dtl" + num;
                    dtl1.PTable = this.FK_MapData + "Dtl" + num;
                    BindEdit(md, dtl1);
                    break;
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            try
            {
                switch (this.DoType)
                {
                    case "New":
                    default:
                        MapDtl dtlN = new MapDtl();
                        dtlN = (MapDtl)this.Pub1.Copy(dtlN);
                        try
                        {
                            dtlN.GroupField = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemStringVal;
                        }
                        catch
                        {
                        }

                        if (this.DoType == "New")
                        {
                            if (dtlN.IsExits)
                            {
                                this.Alert("已存在编号：" + dtlN.No);
                                return;
                            }
                        }
                        dtlN.FK_MapData = this.FK_MapData;
                        dtlN.GroupID = 0;
                        dtlN.RowIdx = 0;

                        // 参数属性.
                        dtlN.DtlSaveModel = (DtlSaveModel)this.Pub1.GetDDLByID("DDL_"+MapDtlAttr.DtlSaveModel).SelectedItemIntVal;
                        dtlN.DtlAddRecModel = (DtlAddRecModel)this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.DtlAddRecModel).SelectedItemIntVal;

                        dtlN.Insert();
                        if (btn.ID.Contains("AndClose"))
                        {
                            this.WinClose();
                            return;
                        }
                        this.Response.Redirect("MapDtl.aspx?DoType=Edit&FK_MapDtl=" + dtlN.No + "&FK_MapData=" + this.FK_MapData, true);
                        break;
                    case "Edit": 
                        MapDtl dtl = new MapDtl(this.FK_MapDtl);
                        dtl = (MapDtl)this.Pub1.Copy(dtl);

                        //参数保存.
                        dtl.IsEnableLink = this.Pub1.GetCBByID("CB_" + MapDtlAttr.IsEnableLink).Checked;

                        //手动保存，added by liuxc,2016-4-7
                        dtl.DtlSaveModel = (DtlSaveModel)this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.DtlSaveModel).SelectedItemIntVal;
                        dtl.DtlAddRecModel = (DtlAddRecModel)this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.DtlAddRecModel).SelectedItemIntVal;


                        if (this.FK_Node != 0)
                        {
                            //是否是分流. add 2015-06-30.
                            dtl.IsFLDtl = this.Pub1.GetCBByID("CB_" + MapDtlAttr.IsFLDtl).Checked;

                            //子线程处理字段.
                            if (this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.SubThreadGroupMark).Items.Count > 0)
                                dtl.SubThreadGroupMark = this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.SubThreadGroupMark).SelectedItemStringVal;

                            if (this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.SubThreadWorker).Items.Count > 0)
                                dtl.SubThreadWorker = this.Pub1.GetDDLByID("DDL_" + MapDtlAttr.SubThreadWorker).SelectedItemStringVal;

                        }
                       

                        dtl.LinkLabel = this.Pub1.GetTBByID("TB_" + MapDtlAttr.LinkLabel).Text;
                        dtl.LinkTarget = this.Pub1.GetTBByID("TB_" + MapDtlAttr.LinkTarget).Text;
                        dtl.LinkUrl = this.Pub1.GetTBByID("TB_" + MapDtlAttr.LinkUrl).Text;

                        //锁定.
                        dtl.IsRowLock = this.Pub1.GetCBByID("CB_" + MapDtlAttr.IsRowLock).Checked;

                        //分组字段。
                        try
                        {
                            dtl.GroupField = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemStringVal;
                        }
                        catch
                        {
                        }

                        if (this.DoType == "New")
                        {
                            if (dtl.IsExits)
                            {
                                this.Alert("已存在编号：" + dtl.No);
                                return;
                            }
                        }

                        dtl.FK_MapData = this.FK_MapData;
                        GroupFields gfs = new GroupFields(dtl.FK_MapData);
                        if (gfs.Count > 1)
                            dtl.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;

                        if (dtl.IsEnableAthM)
                        {
                            /*如果启用了多附件.*/
                            FrmAttachment ath = new FrmAttachment();
                            ath.MyPK = dtl.No + "_AthMDtl";
                            ath.FK_MapData = dtl.No;
                            ath.NoOfObj = "AthMDtl";
                            ath.Name = "从表行记录附件";
                            if (ath.RetrieveFromDBSources() == 0)
                                ath.Insert();
                        }

                        if (this.DoType == "New")
                            dtl.Insert();
                        else
                            dtl.Update();

                        if (btn.ID.Contains("AndC"))
                        {
                            this.WinClose();
                            return;
                        }
                        this.Response.Redirect("MapDtl.aspx?DoType=Edit&FK_MapDtl=" + dtl.No + "&FK_MapData=" + this.FK_MapData, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                MapDtl dtl = new MapDtl();
                dtl.No = this.FK_MapDtl;
                dtl.Delete();
                this.WinClose();
                //this.Response.Redirect("MapDtl.aspx?DoType=DtlList&FK_MapData=" + this.FK_MapData, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }

        void btn_MapAth_Click(object sender, EventArgs e)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.MyPK = this.FK_MapDtl + "_AthMDtl";
            if (ath.RetrieveFromDBSources() == 0)
            {
                ath.FK_MapData = this.FK_MapDtl;
                ath.NoOfObj = "AthMDtl";
                ath.Name = "我的从表附件";
                ath.UploadType = AttachmentUploadType.Multi;
                ath.Insert();
            }
            this.Response.Redirect("Attachment.aspx?DoType=Edit&FK_MapData=" + this.FK_MapDtl + "&UploadType=1&Ath=AthMDtl", true);
        }
        void btn_MapExt_Click1(object sender, EventArgs e)
        {
            this.Response.Redirect("MapExt.aspx?DoType=New&FK_MapData=" + this.FK_MapDtl, true);
        }
        void btn_New_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("MapDtl.aspx?DoType=New&FK_MapData=" + this.FK_MapData, true);
        }
        void btn_Go_Click(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            dtl.IntMapAttrs();
            this.Response.Redirect("MapDtlDe.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&FK_MapDtl=" + this.FK_MapDtl, true);
        }

        public void BindEdit(MapData md, MapDtl dtl)
        {
            bool isItem = false;
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("colspan=3", "基本设置");
            this.Pub1.AddTREnd();

            int idx = 1;
            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("表英文名称");
            TB tb = new TB();
            tb.ID = "TB_No";
            tb.Text = dtl.No;
            if (this.DoType == "Edit")
                tb.Enabled = false;

            tb.Columns = 40;
            this.Pub1.AddTD("colspan=2",tb);
            this.Pub1.AddTREnd();


            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("表中文名称");
            tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = dtl.Name;
            tb.Columns = 40;
            this.Pub1.AddTD("colspan=2",tb);
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("物理表名");
            tb = new TB();
            tb.ID = "TB_PTable";
            tb.Text = dtl.PTable;
            tb.Columns = 40;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            DDL ddl = new DDL();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("工作模式");
            DDL workModelDDl = new DDL();
            workModelDDl.BindSysEnum(MapDtlAttr.Model, (int)dtl.DtlModel);
            workModelDDl.ID = "DDL_Model";
            this.Pub1.AddTD(workModelDDl);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();


            if (dtl.DtlModel == DtlModel.FixRow)
            {
                tb = new TB();
                tb.ID = "TB_" + MapDtlAttr.ImpFixTreeSql;
                tb.Text = dtl.ImpFixTreeSql;
                tb.Columns = 80;

                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("树形结构数据源");
                this.Pub1.AddTD("colspan=2", tb);
                this.Pub1.AddTREnd();

                tb = new TB();
                tb.ID = "TB_" + MapDtlAttr.ImpFixDataSql;
                tb.Text = dtl.ImpFixDataSql;
                tb.Columns = 80;

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("明细表数据源");
                this.Pub1.AddTD("colspan=2", tb);
                this.Pub1.AddTREnd();

            }

            #region 权限控制.
            CheckBox cb = new CheckBox();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            cb.ID = "CB_IsView";
            cb.Text = "是否可见";
            cb.Checked = dtl.IsView;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsUpdate";
            cb.Text = "是否可以修改行"; // "是否可以修改行";
            cb.Checked = dtl.IsUpdate;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsInsert";
            cb.Text = "是否可以新增行"; // "是否可以新增行";
            cb.Checked = dtl.IsInsert;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsDelete";
            cb.Text = "是否可以删除行"; // "是否可以删除行";
            cb.Checked = dtl.IsDelete;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsShowIdx";
            cb.Text = "是否显示序号列"; //"是否显示序号列";
            cb.Checked = dtl.IsShowIdx;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsShowSum";
            cb.Text = "是否合计行";// "是否合计行";
            cb.Checked = dtl.IsShowSum;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsEnableAthM";
            cb.Text = "是否启用多附件";
            cb.Checked = dtl.IsEnableAthM;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsEnableM2M;
            cb.Text = "是否启用一对多";
            cb.Checked = dtl.IsEnableM2M;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsEnableM2MM;
            cb.Text = "是否启用一对多多";
            cb.Checked = dtl.IsEnableM2MM;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsRowLock;
            cb.Text = "是否启用锁定行（如果启用就需要增加IsRowLock一个隐藏的列，默认值为0。）？";// "是否合计行";
            cb.Checked = dtl.IsRowLock;
            this.Pub1.AddTD("colspan=3", cb);
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsShowTitle;
            cb.Text = "是否显示标头(如果是多表表头的明细表，就不要显示表头了)?"; ;
            cb.Checked = dtl.IsShowTitle;
            this.Pub1.AddTD("colspan=3", cb);
            this.Pub1.AddTREnd();
            #endregion 权限控制.


            //this.Pub1.AddTR();
            //this.Pub1.AddTDIdx(idx++);
            //cb = new CheckBox();
            //cb.ID = "CB_IsShowTitle";
            //cb.Text = "是否显示标头";// "是否显示标头";
            //cb.Checked = dtl.IsShowTitle;
            //this.Pub1.AddTD(cb);
            //this.Pub1.AddTREnd();


            //cb = new CheckBox();
            //cb.ID = "CB_IsEnableAth";
            //cb.Text = "是否启用单附件"; 
            //cb.Checked = dtl.IsEnablePass;
            //this.Pub1.AddTD(cb);
            //this.Pub1.AddTREnd();

        
            //为精英修改.
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("增加记录方式");
            ddl = new DDL();
            ddl.ID = "DDL_DtlAddRecModel";
            ddl.BindSysEnum(MapDtlAttr.DtlAddRecModel, (int)dtl.DtlAddRecModel);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("满足不同的用户习惯");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("初始化行数");
            tb = new TB();
            tb.ID = "TB_RowsOfList";
            tb.Attributes["class"] = "TBNum";
            tb.TextExtInt = dtl.RowsOfList;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("高度");
            tb = new TB();
            tb.ID = "TB_H";
            tb.Attributes["class"] = "TBNum";
            tb.Text = dtl.H.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("对傻瓜表单有效,宽度有tabele控制.");
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("显示格式");
            ddl = new DDL();
            ddl.ID = "DDL_DtlShowModel";
            ddl.BindSysEnum(MapDtlAttr.DtlShowModel, (int)dtl.HisDtlShowModel);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("越出处理");
            ddl = new DDL();
            ddl.ID = "DDL_WhenOverSize";
            ddl.BindSysEnum(MapDtlAttr.WhenOverSize, (int)dtl.HisWhenOverSize);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("存盘方式");
            ddl = new DDL();
            ddl.ID = "DDL_DtlSaveModel";
            ddl.BindSysEnum(MapDtlAttr.DtlSaveModel, (int)dtl.DtlSaveModel);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("用于设置在明细表自动存盘还是，手动存盘.");
            this.Pub1.AddTREnd();

            #region 与工作流相关设置.
            if (this.FK_Node != 0)
            {
                this.Pub1.AddTRSum();
                this.Pub1.AddTDTitle("colspan=4", "与工作流相关设置");
                this.Pub1.AddTREnd();

                this.Pub1.AddTR1();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("操作权限");
                ddl = new DDL();
                ddl.BindSysEnum(MapDtlAttr.DtlOpenType, (int)dtl.DtlOpenType);
                ddl.ID = "DDL_DtlOpenType";
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTD();
                this.Pub1.AddTREnd();


                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                cb = new CheckBox();
                cb.ID = "CB_IsEnablePass";
                cb.Text = "是否起用审核字段？";// "是否合计行";
                cb.Checked = dtl.IsEnablePass;
                this.Pub1.AddTD(cb);

                //   string sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE FK_MapData='" + dtl.No + "' AND  ((MyDataType in (1,4,6) and UIVisible=1 ) or (UIContralType=1))";

                string sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE FK_MapData='" + dtl.No + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                {
                    this.Pub1.AddTD();
                    this.Pub1.AddTD();
                }
                else
                {
                    this.Pub1.AddTDBegin("colspan=2");
                    cb = new CheckBox();
                    cb.ID = "CB_IsEnableGroupField";
                    cb.Text = "是否起用分组字段？";// "是否合计行";
                    cb.Checked = dtl.IsEnableGroupField;
                    this.Pub1.Add(cb);

                    ddl = new DDL();
                    ddl.ID = "DDL_GroupField";
                    ddl.BindSQL(sql, "No", "Name", dtl.GroupField);
                    this.Pub1.Add(ddl);
                    this.Pub1.AddTDEnd();
                }
                this.Pub1.AddTREnd();


                this.Pub1.AddTR1();
                this.Pub1.AddTDIdx(idx++);
                cb = new CheckBox();
                cb.ID = "CB_IsCopyNDData";
                cb.Text = "是允许从上一个节点Copy数据";
                cb.Checked = dtl.IsCopyNDData;
                this.Pub1.AddTD("colspan=3", cb);
                this.Pub1.AddTREnd();


                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                cb = new CheckBox();
                cb.ID = "CB_IsHLDtl";
                cb.Text = "是否是合流汇总从表(当前节点是合流节点有效)";
                cb.Checked = dtl.IsHLDtl;
                this.Pub1.AddTD("colspan=3", cb);
                this.Pub1.AddTREnd();


                #region 与分流点发送到子线程，处理子线程处理人相关的设置. for yangtai .
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                cb = new CheckBox();
                cb.ID = "CB_IsFLDtl";
                cb.Text = "是否是分流点从表(当前节点是分流节点有效)";
                cb.Checked = dtl.IsFLDtl;
                this.Pub1.AddTD("colspan=3", cb);
                this.Pub1.AddTREnd();

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("子线程处理人字段");
                sql = "SELECT KeyOfEn as No, Name FROM Sys_MapAttr WHERE FK_MapData='" + dtl.No + "' AND  ( (MyDataType =1 and UIVisible=1 ) or (UIContralType=1))";
                dt = DBAccess.RunSQLReturnTable(sql);

                ddl = new DDL();
                ddl.ID = "DDL_SubThreadWorker";
                ddl.BindSQL(sql, "No", "Name", dtl.SubThreadWorker);
                this.Pub1.AddTD("colspan=2", ddl);
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("子线程批次号字段");
                DataRow dr = dt.NewRow();
                dr[0] = "";
                dr[1] = "无";
                dt.Rows.Add(dr);

                ddl = new DDL();
                ddl.ID = "DDL_SubThreadGroupMark";
                ddl.BindSQL(sql, "No", "Name", dtl.SubThreadGroupMark);
                this.Pub1.AddTD("colspan=2", ddl);
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            #endregion 与分流点发送到子线程，处理子线程处理人相关的设置. for yangtai .


            #endregion 与工作流相关设置.

            #region 明细表的数据导入导出.
            this.Pub1.AddTRSum();
            this.Pub1.AddTDTitle("colspan=4", "数据导入导出");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsExp";
            cb.Text = "是否可以导出？";// "是否可以导出";
            cb.Checked = dtl.IsExp;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsImp";
            cb.Text = "是否可以导入？";// "是否可以导出";
            cb.Checked = dtl.IsImp;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsEnableSelectImp;
            cb.Text = "是否启用选择性导入(如果true就要配置数据源呈现的sql)？";
            cb.Checked = dtl.IsEnableSelectImp;
            this.Pub1.AddTD("colspan=3", cb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("初始化SQL");
            tb = new TB();
            tb.ID = "TB_" + MapDtlAttr.ImpSQLInit;
            tb.Text = dtl.ImpSQLInit;
            tb.Columns = 80;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("查询SQL");
            tb = new TB();
            tb.ID = "TB_" + MapDtlAttr.ImpSQLSearch;
            tb.Text = dtl.ImpSQLSearch;
            tb.Columns = 80;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("填充SQL");
            tb = new TB();
            tb.ID = "TB_" + MapDtlAttr.ImpSQLFull;
            tb.Text = dtl.ImpSQLFull;
            tb.Columns = 80;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            #endregion 明细表的数据导入导出.

            #region 超连接.
            this.Pub1.AddTRSum();
            this.Pub1.AddTDTitle("colspan=4", "表格右边列超连接配置");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsEnableLink;
            cb.Text = "是否启用超连接？";
            cb.Checked = dtl.IsEnableLink;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("超连接标签");

            tb = new TB();
            tb.ID = "TB_" + MapDtlAttr.LinkLabel;
            tb.Text = dtl.LinkLabel;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("colspan=3", "连接URL");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            tb = new TB();
            tb.ID = "TB_" + MapDtlAttr.LinkUrl;
            tb.Text = dtl.LinkUrl;
            tb.Columns = 90;
            this.Pub1.AddTD("colspan=3", tb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("连接目标");
            tb = new TB();
            tb.ID = "TB_" + MapDtlAttr.LinkTarget;
            tb.Text = dtl.LinkTarget;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();
            #endregion 超连接.

            //GroupFields gfs = new GroupFields(md.No);
            //if (gfs.Count > 1)
            //{
            //    this.Pub1.AddTR1();
            //    this.Pub1.AddTDIdx(idx++);
            //    this.Pub1.AddTD("显示在分组");
            //    ddl = new DDL();
            //    ddl.ID = "DDL_GroupID";
            //    ddl.BindEntities(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab, false, AddAllLocation.None);
            //    ddl.SetSelectItem(dtl.GroupID);
            //    this.Pub1.AddTD("colspan=2", ddl);
            //    this.Pub1.AddTREnd();
            //}
            //if (gfs.Count > 1)
            //    this.Pub1.AddTR();
            //else
            //    this.Pub1.AddTR1();

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("");
            this.Pub1.AddTDBegin("colspan=3 align=center");

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = " 保存并关闭 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            if (this.FK_MapDtl != null)
            {
                //btn = new Button();
                //btn.ID = "Btn_D";
                //btn.Text = this.ToE("DesignSheet", "设计表单"); // "设计表单";
                //btn.Click += new EventHandler(btn_Go_Click);
                //this.Pub1.Add(btn);

                btn = new Button();
                btn.ID = "Btn_Del";
                btn.CssClass = "Btn";
                btn.Text = "删除"; // "删除";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Del_Click);
                this.Pub1.Add(btn);


                btn = new Button();
                btn.ID = "Btn_New";
                btn.CssClass = "Btn";
                btn.Text = "新建"; // "删除";
                btn.Click += new EventHandler(btn_New_Click);
                this.Pub1.Add(btn);

                //btn = new Button();
                //btn.ID = "Btn_MapExt";
                //btn.CssClass = "Btn";
                //btn.Text = "扩展设置"; // "删除";
                //btn.Click += new EventHandler(btn_MapExt_Click);
                //this.Pub1.Add(btn);

                if (dtl.IsEnableAthM)
                {
                    btn = new Button();
                    btn.CssClass = "Btn";
                    btn.ID = "Btn_IsEnableAthM";
                    btn.Text = "附件属性"; // "删除";
                    btn.Click += new EventHandler(btn_MapAth_Click);
                    this.Pub1.Add(btn);
                }

                // btn = new Button();
                // btn.ID = "Btn_DtlTR";
                // btn.Text = "多表头";
                // btn.Attributes["onclick"] = "javascript:WinOpen('')";
                //// btn.Click += new EventHandler(btn_DtlTR_Click);
                // this.Pub1.Add(btn);
            }
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
    }

}