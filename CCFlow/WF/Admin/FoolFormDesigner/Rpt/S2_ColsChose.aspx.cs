using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Data;
using BP.Sys;

namespace CCFlow.WF.MapDef.Rpt
{

    public partial class ColsChose : BP.Web.PageBase
    {
        #region 属性.
        public string RptNo
        {
            get
            {
                return this.Request.QueryString["RptNo"];

            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];

            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];

            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            MapAttrs mattrs = new MapAttrs(this.FK_MapData);
            MapAttrs mattrsOfRpt = new MapAttrs(this.RptNo);

            var dictAttrs = new Dictionary<string, List<MapAttr>>();
            dictAttrs.Add("系统字段", new List<MapAttr>());
            dictAttrs.Add("枚举字段", new List<MapAttr>());
            dictAttrs.Add("外键字段", new List<MapAttr>());
            dictAttrs.Add("普通字段", new List<MapAttr>());
            var sysFields = BP.WF.Glo.FlowFields;

            //将属性分组：系统、枚举、外键、普通
            foreach (MapAttr attr in mattrs)
            {
                if (sysFields.Contains(attr.KeyOfEn))
                {
                    dictAttrs["系统字段"].Add(attr);
                }
                else if (attr.HisAttr.IsEnum)
                {
                    dictAttrs["枚举字段"].Add(attr);
                }
                else if (attr.HisAttr.IsFK)
                {
                    dictAttrs["外键字段"].Add(attr);
                }
                else
                {
                    dictAttrs["普通字段"].Add(attr);
                }
            }
            this.Pub2.AddTable();
            foreach (var de in dictAttrs)
            {
                if (de.Value.Count == 0)
                    continue;
                this.Pub2.AddTR();
                this.Pub2.AddTDGroupTitle("colspan=3", de.Key);
                this.Pub2.AddTREnd();

                int isBr = 0;
                foreach (var attr in de.Value)
                {
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn;
                    cb.Text = attr.Name + "(" + attr.KeyOfEn + ")";
                    cb.Checked = mattrsOfRpt.Contains(MapAttrAttr.KeyOfEn, attr.KeyOfEn);

                    switch (attr.KeyOfEn)
                    {
                        case NDXRptBaseAttr.Title:
                        case NDXRptBaseAttr.MyNum:
                        case NDXRptBaseAttr.OID:
                        case NDXRptBaseAttr.WFSta:
                            cb.Checked = true;
                            cb.Enabled = false;
                            break;
                        case NDXRptBaseAttr.WFState:
                            continue;
                        default:
                            break;
                    }

                    if (isBr == 0)
                        this.Pub2.AddTR();

                    this.Pub2.AddTD("style='width:33%'", cb);

                    if (isBr == 2)
                        this.Pub2.AddTREnd();
                    isBr++;
                    if (isBr == 3)
                        isBr = 0;
                }

                if (isBr == 1)
                {
                    Pub2.AddTD();
                    Pub2.AddTD();
                    Pub2.AddTREnd();
                }
                if (isBr == 2)
                {
                    Pub2.AddTD();
                    Pub2.AddTREnd();
                }
            }
            this.Pub2.AddTableEnd();
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Save();
            Response.Redirect("S2_ColsChose.aspx?FK_MapData=" + this.FK_MapData + "&RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow + "&s=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }

        protected void Btn_SaveAndNext1_Click(object sender, EventArgs e)
        {
            Save();
            Response.Redirect("S3_ColsLabel.aspx?FK_MapData=" + this.FK_MapData + "&RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow + "&s=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }

        private void Save()
        {
            MapAttrs mattrs = new MapAttrs(this.FK_MapData);
            MapAttrs mrattrs = new MapAttrs(this.RptNo);
            MapAttr tattr = null;

            foreach (MapAttr attr in mattrs)
            {
                CheckBox cb = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn);
                tattr = mrattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, attr.KeyOfEn) as MapAttr;

                if (cb == null || cb.Checked == false)
                {
                    if(tattr != null)
                    {
                        tattr.Delete();
                        mrattrs.RemoveEn(tattr);
                    }

                    continue;
                }

                attr.FK_MapData = this.RptNo;
                if (attr.KeyOfEn == "FK_NY")
                {
                    attr.LGType = BP.En.FieldTypeS.FK;
                    attr.UIBindKey = "BP.Pub.NYs";
                    attr.UIContralType = BP.En.UIContralType.DDL;
                }

                if (attr.KeyOfEn == "FK_Dept")
                {
                    attr.LGType = BP.En.FieldTypeS.FK;
                    attr.UIBindKey = "BP.Port.Depts";
                    attr.UIContralType = BP.En.UIContralType.DDL;
                }

                if (tattr != null)
                {
                    attr.Idx = tattr.Idx;
                    attr.Name = tattr.Name;
                    tattr.Delete();
                    mrattrs.RemoveEn(tattr);
                }

                attr.Insert();
            }
        }
    }
}