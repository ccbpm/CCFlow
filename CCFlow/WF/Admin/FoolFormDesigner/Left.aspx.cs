using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.MapDef
{
    public partial class Left : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                string key = this.Request.QueryString["MyPK"];
                if (key == null)
                    key = this.Request.QueryString["PK"];
                if (key == null)
                    key = this.Request.QueryString["FK_MapData"];
                if (key == null)
                    key = "ND1601";
                return key;
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.MyPK;
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// IsEditMapData
        /// </summary>
        public bool IsEditMapData
        {
            get
            {
                string s = this.Request.QueryString["IsEditMapData"];
                if (s == null || s == "1")
                    return true;
                return false;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            
            BP.WF.XML.MapMenus xmls = new BP.WF.XML.MapMenus();
            xmls.RetrieveAll();

            #region bindleft
            //this.Left.Add("<a href='http://ccflow.org' target=_blank ><img src='../../DataUser/ICON/" + SystemConfig.CustomerNo + "/LogBiger.png' border=0/></a>");
            //this.Left.AddHR();
            this.Pub1.AddUL();
            foreach (BP.WF.XML.MapMenu item in xmls)
            {
                this.Pub1.AddLi("<a href=\"" + item.JS.Replace("@MyPK", "'" + this.FK_MapData + "'").Replace("@FK_Flow", "'" + this.FK_Flow + "'") + "\" ><img src='" + item.Img + "' width='16px' /><b>" + item.Name + "</b></a><br><font color=green>" + item.Note + "</font>");
            }
            this.Pub1.AddULEnd();
            #endregion bindleft
        }
    }
}