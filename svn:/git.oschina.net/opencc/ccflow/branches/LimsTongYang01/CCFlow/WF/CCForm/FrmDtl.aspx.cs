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
    public partial class WF_FrmDtl : System.Web.UI.Page
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 101;
                }
            }
        }
        public int WorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int OID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["OID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                    return "ND101";
                return s;
            }
        }
        public bool IsReadonly
        {
            get
            {
                if (this.Request.QueryString["IsReadonly"] == "1")
                    return true;
                return false;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.FK_MapData);
            GEDtl dtlEn = dtl.HisGEDtl;
            dtlEn.SetValByKey("OID", this.OID);
            dtlEn.RetrieveFromDBSources();

            MapAttrs mattrs = new MapAttrs(dtl.No);
            foreach (MapAttr mattr in mattrs)
            {
                if (mattr.DefValReal.Contains("@") == false)
                    continue;
                dtlEn.SetValByKey(mattr.KeyOfEn, mattr.DefVal);
            }

            this.Btn_Save.UseSubmitBehavior = false;
            this.Btn_Save.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";

            //是否要重新装载数据.
            bool isLoadData = false;
            if (this.Request.QueryString["IsLoadData"]=="1")
                isLoadData=true;
            if (this.Request.QueryString["IsReadonly"]=="1")
                isLoadData=false;


            this.UCEn1.BindCCForm(dtlEn, this.FK_MapData, this.IsReadonly, 0,isLoadData);


            if (this.IsReadonly)
            {
                this.Btn_Save.Visible = false;
                this.Btn_Save.Enabled = false;
            }
            else
            {
                this.Btn_Save.Visible = true;
                this.Btn_Save.Enabled = true;
            }
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {

                MapDtl dtl = new MapDtl(this.FK_MapData);
                GEDtl dtlEn = dtl.HisGEDtl;
                dtlEn.SetValByKey("OID", this.OID);
                int i = dtlEn.RetrieveFromDBSources();
                dtlEn = this.UCEn1.Copy(dtlEn) as GEDtl;
                dtlEn.SetValByKey(GEDtlAttr.RefPK, this.WorkID);

                if (i == 0)
                {
                    dtlEn.OID = 0;
                    dtlEn.Insert();
                }
                else
                {
                    dtlEn.Update();
                }

                this.Response.Redirect("FrmDtl.aspx?WorkID=" + dtlEn.RefPK + "&FK_MapData=" + this.FK_MapData + "&IsReadonly=" + this.IsReadonly + "&OID=" + dtlEn.OID, true);

                //if (fes.Contains(FrmEventAttr.FK_Event, FrmEventList.SaveAfter) == true
                //    || fes.Contains(FrmEventAttr.FK_Event, FrmEventList.SaveBefore) == true)
                //{
                //    /*如果包含保存*/
                //    // /FrmDtl.aspx?FK_MapData=ND11699Dtl1&WorkID=2078&OID=7365&IsReadonly=False
                //    this.Response.Redirect(this.Request.RawUrl, true);
                //    //this.Response.Redirect("FrmDtl.aspx?WorkID=" + this.WorkID + "&FK_MapData=" + this.FK_MapData + "&IsReadonly="+this.IsReadonly, true);
                //}
            }
            catch (Exception ex)
            {
                this.UCEn1.AddMsgOfWarning("error:", ex.Message);
            }
        }
    }
}