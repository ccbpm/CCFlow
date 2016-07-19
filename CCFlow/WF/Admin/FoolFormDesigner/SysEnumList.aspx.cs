using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.UC;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    public partial class SysEnumList :  WebPage
    {
        #region 属性.
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string FK_MapData
        {
            get
            {
                string str= this.Request.QueryString["FK_MapData"];
                if (str == null)
                    str = this.MyPK;
                return str;
            }
        }
        public string GroupField
        {
            get
            {
                return this.Request.QueryString["GroupField"];
            }
        }
        public string Idx
        {
            get
            {
                return this.Request.QueryString["Idx"];
            }
        }
        #endregion 属性.

        int pageSize = 36;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            this.Title = "增加新字段向导";
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("<a href='FieldTypeList.aspx?DoType=AddF&FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "'><img src='/WF/Img/Btn/Back.gif'>返回</a></a> - <a href='SysEnum.aspx?DoType=New&FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "' ><img src='../../Img/Btn/New.gif' />新建枚举</a>");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("编号(点击增加到表单)");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTDTitle();
            this.Pub1.AddTREnd();

            BP.Sys.SysEnumMains sems = new SysEnumMains();
            QueryObject qo = new QueryObject(sems);
            this.Pub2.BindPageIdx(qo.GetCount(), pageSize, this.PageIdx, "?DoType=AddSysEnum&FK_MapData=" + this.FK_MapData + "&Idx="+this.Idx+"&GroupField="+this.GroupField);
            qo.DoQuery("No", pageSize, this.PageIdx);

            bool is1 = false;
            int Idx = 0;
            foreach (BP.Sys.SysEnumMain sem in sems)
            {
                BP.Web.Controls.DDL ddl = null;
                try
                {
                    ddl = new BP.Web.Controls.DDL();
                    ddl.BindSysEnum(sem.No);
                }
                catch
                {
                    sem.Delete();
                }
                Idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(Idx);
                this.Pub1.AddTD("<a href=\"javascript:AddEnum('" + this.FK_MapData + "','" + this.GroupField + "','" + sem.No + "')\" >" + sem.No + "</a>");
                this.Pub1.AddTD(sem.Name);
                this.Pub1.AddTD("[<a href='SysEnum.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "&EnumKey=" + sem.No + "' >编辑</a>]");
                this.Pub1.AddTD(ddl);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}