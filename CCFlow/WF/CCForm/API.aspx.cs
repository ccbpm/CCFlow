using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.CCForm
{
    public partial class API : BP.Web.WebPage
    {
        /// <summary>
        /// ccform 的api.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (WebUser.NoOfRel == null)
            {
                this.Pub1.AddFieldSet("ERR","用户登录信息丢失.");
                return;
            }

            string fk_mapdata = this.Request.QueryString["FK_MapData"];
            string oid = this.Request.QueryString["OID"];

            MapData md = new MapData();
            md.No = fk_mapdata;
            if (md.RetrieveFromDBSources() == 0)
                this.Pub1.AddFieldSetRed("错误","表单编号错误:"+md.No);
            if (md.HisFrmType != FrmType.Url)
            {
                /*处理表单装载前事件.*/
               GEEntity entity=md.HisGEEn;
               entity.PKVal = oid;
               entity.RetrieveFromDBSources();
               entity.ResetDefaultVal();

               //执行一次装载前填充.
               string msg = md.FrmEvents.DoEventNode(FrmEventList.FrmLoadBefore, entity);
               if (string.IsNullOrEmpty(msg) == false)
               {
                   this.Pub1.AddFieldSetRed("错误","错误"+msg);
                   return;
               }
            }

            switch(md.HisFrmType)
            {
                case FrmType.FreeFrm:
                    this.Response.Redirect("Frm.aspx?FK_MapData=" + fk_mapdata + "&OID=" + oid, true); 
                    break;
                case FrmType.Column4Frm:
                    this.Response.Redirect("FrmFix.aspx?FK_MapData=" + fk_mapdata + "&OID=" + oid, true);
                    break;
                case FrmType.SLFrm: // 
                    this.Response.Redirect("SLFrm.aspx?FK_MapData=" + fk_mapdata + "&OID=" + oid, true);
                    break;
                case FrmType.Url: // 如果是一个超链接
                    string url = md.PTable;
                    url = url.Replace("@OID", oid);
                    url = url.Replace("@PK", oid);
                    this.Response.Redirect(url, true);
                    break;
                default:
                    break;
            }

        }
    }
}