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

namespace CCFlow.WF.MapDef
{
    public partial class TBFullCtrl_Dtl : BP.Web.WebPage
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
            if (IsPostBack==false)
            {
                MapExt myme = new MapExt(this.MyPK);
                MapDtls dtls = new MapDtls(myme.FK_MapData);
                string[] strs = myme.Tag1.Split('$');

                foreach (MapDtl dtl in dtls)
                {


                    foreach (string s in strs)
                    {
                        if (s == null)
                            continue;

                        if (s.Contains(dtl.No + ":") == false)
                            continue;

                        string[] ss = s.Split(':');
                       this.TB_SQL.Text = ss[1];
                    }


                    this.LabNo.Text = dtl.No;//编号显示
                    this.LabName.Text = dtl.Name;//名称显示
                    MapAttrs attrs = new MapAttrs(dtl.No);
                    foreach (MapAttr item in attrs)
                    {
                        if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal")
                            continue;
                        this.LabZD.Text += item.KeyOfEn + ",";//可填充的字段显示
                    }


                }


            }

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {

            MapExt myme = new MapExt(this.MyPK);
            MapDtls dtls = new MapDtls(myme.FK_MapData);
           

            string info = "";
            string error = "";
            foreach (MapDtl dtl in dtls)
            {
                
                if (this.TB_SQL.Text.Trim() == "")
                    continue;
               
                info += "$" + dtl.No + ":" + this.TB_SQL.Text;
            }

            if (error != "")
            {

                 throw new Exception("设置错误，请更正:<br/>"+error+"");
            }
            myme.Tag1 = info;
            myme.Update();
           
        }
      

        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {

            Btn_Save_Click(null, null);
            BP.Sys.PubClass.WinClose();
        }
    }
}