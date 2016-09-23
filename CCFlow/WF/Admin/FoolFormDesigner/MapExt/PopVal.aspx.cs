using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
using BP.Tools;

namespace CCFlow.WF.MapDef
{
    public partial class PopVal : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request["FK_MapData"];
            }
        }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string RefNo
        {
            get
            {
                return this.Request["RefNo"];
            }
        }
        #endregion 属性.

        /// <summary>
        /// 2016.4.19 by liuhui.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = this.Request.RawUrl;
            url = url.Replace("PopVal.aspx", "PopVal.htm");
            this.Response.Redirect(url, true);
            return;


            if ( this.IsPostBack==false)
            {
                //实例化控件值.
                BP.Sys.MapExt ext = new BP.Sys.MapExt();
               int i=  ext.Retrieve(BP.Sys.MapExtAttr.FK_MapData, FK_MapData,
                    BP.Sys.MapExtAttr.ExtType, "PopVal",
                    BP.Sys.MapExtAttr.AttrOfOper, RefNo);
               
                if (i == 0)
                   this.Btn_Delete.Visible = false;
               else
                    this.Btn_Delete.Visible = true;

                #region 给控件赋值.  2016.4.19 by liuhui.
                // 工作模式 0 -url .1-内置
                if (ext.PopValWorkModel == PopValWorkModel.SelfUrl)
                    this.RB_Model_Url.Checked = true;


                ////选择数据方式
                //if (ext.PopValSelectModel == PopValSelectModel.One)
                //    this.RB_PopValSelectModel_0.Checked = true;
                //else
                //    this.RB_PopValSelectModel_1.Checked = true;

                if (ext.PopValFormat == 0)
                    this.RB_PopValFormat_0.Checked = true;

                if (ext.PopValFormat == 1)
                    this.RB_PopValFormat_1.Checked = true;

                if (ext.PopValFormat == 2)
                    this.RB_PopValFormat_2.Checked = true;

                //宽度/高度/标题
                this.TB_Width.Text = ext.W == 0 ? "760" : ext.W.ToString();
                this.TB_Height.Text = ext.H == 0 ? "450" : ext.H.ToString();
                this.TB_Title.Text = ext.GetParaString("Title");
                
                //数据源分组sql
                this.TB_Group.Text = ext.Tag1;
                //数据源sql
                this.TB_Entity.Text = ext.Tag2;
                // rul
                this.TB_URL.Text = ext.Doc;
                #endregion 给控件赋值.  
            }
        }
            
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            //查询出实体.
            BP.Sys.MapExt ext = new BP.Sys.MapExt();
            ext.Retrieve(BP.Sys.MapExtAttr.FK_MapData, FK_MapData,
                BP.Sys.MapExtAttr.ExtType, "PopVal",
                BP.Sys.MapExtAttr.AttrOfOper, RefNo);

            // 工作模式 0 -url .1-内置
            if (this.RB_Model_Inntel.Checked)
                ext.PopValWorkModel = PopValWorkModel.SelfUrl;

            //返回值格式
            if (this.RB_PopValFormat_0.Checked)
            {
                ext.PopValFormat = 0;
            }
            if (this.RB_PopValFormat_1.Checked)
            {
                ext.PopValFormat = 1;
            }

            if (this.RB_PopValFormat_2.Checked)
            {
                ext.PopValFormat = 2;
            }
            //数据源分组sql
            if (!string.IsNullOrEmpty(this.TB_Group.Text ))
            {
                ext.Tag1 = this.TB_Group.Text;
            }
            else
            {
                ext.Tag1 = "";
            }
            //数据源sql
            if (!string.IsNullOrEmpty( this.TB_Entity.Text))
            {
                ext.Tag2 = this.TB_Entity.Text;
            }
            else
            {
                ext.Tag2 = "";
            }

            //查询sql
            if (!string.IsNullOrEmpty(this.TB_SearchSQL.Text))
            {
                ext.Tag3 = this.TB_SearchSQL.Text;
            }
            else
            {
                ext.Tag3 = "";
            }


            //URL
            if (string.IsNullOrEmpty(this.TB_URL.Text)==false)
                ext.Doc = this.TB_URL.Text;
            else
                ext.Doc = "";

            //弹窗宽度/高度
            int d;
            if (!string.IsNullOrWhiteSpace(this.TB_Width.Text) && int.TryParse(this.TB_Width.Text, out d))
                ext.W = d;
            else
                ext.W = 760;

            if (!string.IsNullOrWhiteSpace(this.TB_Height.Text) && int.TryParse(this.TB_Height.Text, out d))
                ext.H = d;
            else
                ext.H = 450;

            //弹窗标题
            ext.SetPara("Title", this.TB_Title.Text);

            //操作的文本框
            ext.AttrOfOper = this.RefNo;
            ext.ExtType = MapExtXmlList.PopVal; 
            ext.FK_MapData = this.FK_MapData;
            ext.MyPK = ext.ExtType + "_" + this.FK_MapData + "_" + ext.AttrOfOper;
            ext.Save();
        }

        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            Btn_Save_Click(null, null);
            BP.Sys.PubClass.WinClose();
        }

        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }

        protected void Btn_Delete_Click(object sender, EventArgs e)
        {
            BP.Sys.MapExt ext = new BP.Sys.MapExt();
            ext.MyPK = ext.ExtType + "_" + this.FK_MapData + "_" +this.RefNo;
            ext.Delete();
            BP.Sys.PubClass.WinClose();
        }

    }
}