using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web.Controls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;
using BP.DA;

namespace CCFlow.WF.MapDef
{
    public partial class AutoFullUI : BP.Web.WebPage
    {
        #region 属性
        /// <summary>
        /// 执行类型
        /// </summary>
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public int FType
        {
            get
            {
                return int.Parse(this.Request.QueryString["FType"]);
            }
        }
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack == false)
            {

                MapAttr mattrNew = new MapAttr(this.RefNo);

                MapExt me = new MapExt();
                me.MyPK = this.RefNo + "_AutoFull";
                me.RetrieveFromDBSources();
                me.FK_MapData = this.FK_MapData;
                me.AttrOfOper = mattrNew.KeyOfEn;
                me.ExtType = MapExtXmlList.AutoFull;
                if (me.Tag == "0")
                {
                    this.RB_0.Checked = true;
                    this.TB_Exp.Text = "";
                }
                else
                {
                    this.RB_0.Checked = false;
                }

                if (me.Tag == "1")
                {
                    this.RB_1.Checked = true;
                }
                else
                {
                    this.RB_1.Checked = false;
                }
                 
                this.TB_Exp.Text = me.Doc;
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            MapAttr mattrNew = new MapAttr(this.RefNo);

            MapExt me = new MapExt();
            me.MyPK = this.RefNo + "_AutoFull";
            me.RetrieveFromDBSources();
            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = mattrNew.KeyOfEn;
            me.ExtType = MapExtXmlList.AutoFull;
            if (this.RB_0.Checked)
            {
                me.Tag = "0";
                this.TB_Exp.Text = "";
                me.Doc = "";

            }

            if (this.RB_1.Checked)
            {
                me.Tag = "1";
                me.Doc = this.TB_Exp.Text;

                /*检查字段是否填写正确.*/
                MapAttrs attrsofCheck = new MapAttrs(this.FK_MapData);
                string docC = me.Doc;
                foreach (MapAttr attrC in attrsofCheck)
                {
                    if (attrC.IsNum == false)
                        continue;
                    docC = docC.Replace("@" + attrC.KeyOfEn, "");
                    docC = docC.Replace("@" + attrC.Name, "");
                }

                if (docC.Contains("@"))
                {
                    this.Alert("您填写的表达公式不正确，导致一些数值类型的字段没有被正确的替换。" + docC);
                    return;
                }


            }
            me.Save();
        }

        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            Btn_Save_Click(null, null);
            BP.Sys.PubClass.WinClose();
        }
    }
}