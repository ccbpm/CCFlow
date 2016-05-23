using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Sys;

namespace CCFlow.WF.Admin.AttrNode
{
    public partial class SepcFiledsSepcUsersDtl :BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string Fields
        {
            get
            {
                return this.Request.QueryString["Fields"];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {

            BP.Sys.MapExt me = new MapExt();
            if (this.MyPK != null)
            {
                me.MyPK = this.MyPK;
                me.RetrieveFromDBSources();

                me.FK_MapData = this.FK_MapData;
                me.Doc = this.Fields;
                me.ExtType = "SepcFiledsSepcUsers";
                me.Tag = this.FK_Node;

                me.Tag1 = this.TB_Emps.Text;
                me.Tag2 = this.TB_SQL.Text;

                //if (me.Tag.Length == 0)
                //{
                //    this.Alert("您没有设置人员,无法保存.");
                //    return;
                //}

                me.Update();
            }
            else
            {
                me.MyPK = BP.DA.DBAccess.GenerGUID();

                me.FK_MapData = this.FK_MapData;
                me.Doc = this.Fields;

                me.Tag = this.FK_Node;

                me.Tag1 = this.TB_Emps.Text;
                me.Tag2 = this.TB_SQL.Text;

                //if (me.Tag.Length == 0)
                //{
                //    this.Alert("您没有设置人员,无法保存.");
                //    return;
                //}
                me.ExtType = "SepcFiledsSepcUsers";
                me.Insert();
            }

            this.WinClose("ok");
        }

        protected void Btn_Del_Click(object sender, EventArgs e)
        {
            BP.Sys.MapExt me = new MapExt();
            me.MyPK = this.MyPK;
            me.Delete();
            this.WinClose("ok");
        }
    }
}