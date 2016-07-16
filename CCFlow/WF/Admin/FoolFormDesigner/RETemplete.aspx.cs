using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys.XML;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Template.XML;
using BP.WF;

namespace CCFlow.WF.MapDef
{
    public partial class RegularExpressionTemplete : BP.Web.WebPage
    {
        #region 属性
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string KeyOfEn
        {
            get
            {
                return this.Request.QueryString["KeyOfEn"];
            }
        }
        public string ForCtrl
        {
            get
            {
                return this.Request.QueryString["ForCtrl"];
            }
        }
        #endregion

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["REID"] != null)
            {
                RegularExpressionDtls reDtls = new RegularExpressionDtls();
                reDtls.RetrieveAll();

                //删除现有的逻辑.
                BP.Sys.MapExts exts = new BP.Sys.MapExts();
                exts.Delete(MapExtAttr.AttrOfOper, this.KeyOfEn, 
                    MapExtAttr.ExtType, BP.Sys.MapExtXmlList.RegularExpression);

                // 开始装载.
                foreach (RegularExpressionDtl dtl in reDtls)
                {
                    if (dtl.ItemNo != this.Request.QueryString["REID"])
                        continue;

                    BP.Sys.MapExt ext = new BP.Sys.MapExt();
                    ext.MyPK = this.FK_MapData + "_" + this.KeyOfEn + "_" + MapExtXmlList.RegularExpression + "_" + dtl.ForEvent;
                    ext.FK_MapData = this.FK_MapData;
                    ext.AttrOfOper = this.KeyOfEn;
                    ext.Doc = dtl.Exp; //表达公式.
                    ext.Tag = dtl.ForEvent; //时间.
                    ext.Tag1 = dtl.Msg;  //消息
                    ext.ExtType = MapExtXmlList.RegularExpression; // 表达公式 .
                    ext.Insert();
                }
                //this.WinClose("1"); //关闭并返回一个值。
                return;
            }

            RegularExpressions res = new RegularExpressions();
            res.RetrieveAll();

            this.Pub1.AddH3("事件模版-点击名称选用它.");
            this.Pub1.AddHR();

            this.Pub1.AddUL();
            foreach (RegularExpression item in res)
            {
                this.Pub1.AddLi("<a href=\"javascript:DoIt('" + this.FK_MapData + "','" + this.KeyOfEn 
                    + "','" + this.ForCtrl + "','" + item.No + "','" + item.Name
                    + "');\" >" + item.Name + "</a> - " + item.Note);
            }
            this.Pub1.AddULEnd();
        }
    }
}