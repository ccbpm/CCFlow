using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.MapDef.MapExtUI
{
    public partial class AutoFullDLLUI : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (this.IsPostBack == false)
            {
                string fk_mapdata = this.Request.QueryString["FK_MapData"];
                string RefNo = this.Request.QueryString["RefNo"];


                BP.Sys.MapExt ext = new BP.Sys.MapExt();
                ext.MyPK = "AutoFullDLL_" + fk_mapdata + "_" + RefNo;
                ext.RetrieveFromDBSources();
                this.TB_Doc.Value = ext.Doc;

            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            string fk_mapdata = this.Request.QueryString["FK_MapData"];
            string RefNo = this.Request.QueryString["RefNo"];

            BP.Sys.MapExt ext = new BP.Sys.MapExt();
            ext.MyPK = "AutoFullDLL_" + fk_mapdata + "_" + RefNo;
            ext.RetrieveFromDBSources();
            ext.FK_MapData = fk_mapdata;
            ext.Doc = this.TB_Doc.Value;
            ext.AttrOfOper = RefNo;
            ext.ExtType = "AutoFullDLL";
            ext.Save();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void But_Del_Click(object sender, EventArgs e)
        {
            string fk_mapdata = this.Request.QueryString["FK_MapData"];
            string RefNo = this.Request.QueryString["RefNo"];

            BP.Sys.MapExt ext = new BP.Sys.MapExt();
            ext.MyPK = "AutoFullDLL_" + fk_mapdata + "_" + RefNo;
            ext.Delete();

            BP.Sys.PubClass.WinCloseAndAlertMsg("删除成功!");
             
        }
    }
}