using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_CCForm_ViewFrm : BP.Web.WebPage
    {
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Application.Clear();
            //this.Session.Clear();
            //this.Application.Clear();
            //BP.DA.Cash.Map_Cash.Clear();

            this.Page.RegisterClientScriptBlock("s",
              "<script language='JavaScript' src='../Scripts/jquery-1.4.1.min.js' ></script>");

            this.Page.RegisterClientScriptBlock("b",
         "<script language='JavaScript' src='../CCForm/MapExt.js' ></script>");

            this.UCEn1.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");

            //if (this.FK_MapData.Contains("ND"))
            //{
            //    int nodeid = int.Parse(this.FK_MapData.Replace("ND", ""));
            //    BP.WF.Node nd = new BP.WF.Node(nodeid);
            //    BP.WF.Work work = new BP.WF.GEStartWork(nd.NodeID);
            //    this.UCEn1.BindCCForm(work, this.FK_MapData,false);
            //    return;
            //}

            MapData md = new MapData(this.FK_MapData);

            switch (this.DoType)
            {
                case "CCForm":
                    this.UCEn1.BindCCForm(md.HisGEEn, this.FK_MapData, false, 0,false);
                    break;
                case "FoolForm":
                    this.UCEn1.BindColumn4(md.HisEn, this.FK_MapData);
                    break;
                default:
                    this.UCEn1.BindColumn2(md.HisEn, this.FK_MapData);
                    break;
            }
        }
    }
}