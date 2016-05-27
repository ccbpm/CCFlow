using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP;
using BP.Sys;
using BP.Web.Controls;
using System.Collections;

namespace CCFlow.WF.MapDef
{
    public partial class TBFullCtrlNew : BP.Web.WebPage
    {

        #region 属性。
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string OperAttrKey
        {
            get
            {
                return this.Request.QueryString["OperAttrKey"];
            }
        }
        public string ExtType
        {
            get
            {
                return MapExtXmlList.TBFullCtrl;
            }
        }

        public string Lab = null;
        #endregion 属性。
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                MapExt me = new MapExt();

                me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, this.ExtType, MapExtAttr.AttrOfOper, this.RefNo);
                this.TB_SQL.Text = me.Doc;

                BP.Sys.SFDBSrcs srcs = new SFDBSrcs();
                srcs.RetrieveAll();
                BP.Web.Controls.Glo.DDL_BindEns(this.DDL_DBSrc, srcs, me.FK_DBSrc);
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper;
            me.RetrieveFromDBSources();
            me.ExtType = this.ExtType;
            me.Doc = this.TB_SQL.Text;
            me.AttrOfOper = this.RefNo;
            me.FK_MapData = this.FK_MapData;
            me.FK_DBSrc = this.DDL_DBSrc.SelectedValue;
            me.Save();
        }
        /// <summary>
        /// 保存并关闭.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            Btn_Save_Click(null, null);
            BP.Sys.PubClass.WinClose();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Delete_Click(object sender, EventArgs e)
        {
            MapExt me = new MapExt();
            me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, this.ExtType, MapExtAttr.AttrOfOper, this.RefNo);
            me.Doc = "";
            me.Delete();

            BP.Sys.PubClass.WinClose();
        }
        protected void Btn_FullDtl_Click(object sender, EventArgs e)
        {
            Response.Redirect("TBFullCtrl_Dtl.aspx?FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + "");
        }

        protected void Btn_FullDDL_Click(object sender, EventArgs e)
        {
            Response.Redirect("TBFullCtrl_ListNew.aspx?FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + ""); 
        }
    }
}