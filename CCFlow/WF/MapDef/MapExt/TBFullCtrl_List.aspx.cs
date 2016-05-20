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
    public partial class TBFullCtrl_List : BP.Web.WebPage
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
                MapAttrs attrs = new MapAttrs(myme.FK_MapData);
                string[] strs = myme.Tag.Split('$');
                foreach (MapAttr attr in attrs)
                {
                    if (attr.LGType == FieldTypeS.Normal)
                        continue;
                    if (attr.UIIsEnable == false)
                        continue;
                    foreach (string s in strs)
                    {
                        if (s == null)
                            continue;
                        if (s.Contains(attr.KeyOfEn + ":") == false)
                            continue;
                        string[] ss = s.Split(':');
                        this.TB_SQL.Text = ss[1];//填充文本
                    }
                    this.LabJLZD.Text = attr.Name + " " + attr.KeyOfEn + " 字段";//填充lab控件
                }
            }
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            MapExt myme = new MapExt(this.MyPK);
            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            string info = "";
            foreach (MapAttr attr in attrs)
            {
                if (attr.LGType == FieldTypeS.Normal)
                    continue;
                if (attr.UIIsEnable == false)
                    continue;
                if (this.TB_SQL.Text.Trim() == "")
                    continue;
                try
                {
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(this.TB_SQL.Text);
                    if (this.TB_SQL.Text.Contains("@Key") == false)
                        throw new Exception("缺少@Key参数。");
                    if (dt.Columns.Contains("No") == false || dt.Columns.Contains("Name") == false)
                        throw new Exception("在您的sql表单公式中必须有No,Name两个列，来绑定下拉框。"); 

                }

                catch (Exception ex)
                {
                    this.Alert("SQL ERROR: " + ex.Message);
                    return;
                }
                info += "$" + attr.KeyOfEn + ":" + this.TB_SQL.Text;  
            }

            myme.Tag = info;
            myme.Update();
            this.Alert("保存成功.");
        }


        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            Btn_Save_Click(null, null);
            BP.Sys.PubClass.WinClose();
        }
    }
}