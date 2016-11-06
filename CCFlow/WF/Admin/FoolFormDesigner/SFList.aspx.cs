using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class SFList : BP.Web.WebPage
    {
        #region 属性.
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string Idx
        {
            get
            {
                return this.Request.QueryString["Idx"];
            }
        }
        public string GroupField
        {
            get
            {
                return this.Request.QueryString["GroupField"];
            }
        }
        int pageSize = 12;
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

            BP.Sys.SFTables ens = new SFTables();
            QueryObject qo = new QueryObject(ens);

            this.Pub2.BindPageIdx(qo.GetCount(), pageSize, this.PageIdx,
                "SFList.aspx?FK_MapData=" + this.FK_MapData + "&Idx=&GroupField=" + this.GroupField);
            qo.DoQuery("No", pageSize, this.PageIdx);

            this.Pub1.AddTable();
           // this.Pub1.AddCaption("<a href='FieldTypeList.aspx?DoType=AddF&FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "'><img src='/WF/Img/Btn/Back.gif' />&nbsp;返回</a> - 字典表列表 - <a href='/WF/Comm/Sys/SFGuide.aspx?DoType=New&FromApp=SL' >新建字典表</a> - <a href='SFEdit.aspx?DoType=New&FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "' >新建表New</a>");
            this.Pub1.AddCaption("<a href='FieldTypeList.aspx?DoType=AddF&FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "'><img src='/WF/Img/Btn/Back.gif' />&nbsp;返回</a> - 字典表列表 - <a href='/WF/Comm/Sys/SFGuide.aspx?DoType=New&FromApp=SL' >新建字典表</a>");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("表名编号");
            this.Pub1.AddTDTitle("表名称");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTDTitle("编码表类型");
            this.Pub1.AddTDTitle("编辑数据");
            this.Pub1.AddTDTitle("属性");
            this.Pub1.AddTDTitle("数据源");
            this.Pub1.AddTDTitle("数据类型");
            this.Pub1.AddTREnd();

            if (ens.Count == 0)
            {
                //string html = "<a href='Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&Idx=" + this.Idx + "'><img src='/WF/Img/Btn/Back.gif' />&nbsp;返回</a> - 增加外键字段 - <a href='SFTable.aspx?DoType=New&MyPK=" + this.MyPK + "&Idx=" + this.Idx + "' >新建表</a>";
                this.Pub1.AddTR();
                this.Pub1.AddTDBigDoc("colspan=5", "注册到ccform的表为空，点击上面的新建表，进入创建向导。");
                this.Pub1.AddTREnd();
                this.Pub1.AddTableEnd();
                return;
            }


            bool is1 = false;
            int Idx = 0;
            foreach (BP.Sys.SFTable sem in ens)
            {
                Idx++;
                //is1 = this.Pub1.AddTR(is1);
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(Idx);

                this.Pub1.AddTD("<a  href=\"javascript:AddSFTable('" + this.FK_MapData + "','" + this.Idx + "','" + sem.No + "','" + this.GroupField + "')\" >" + sem.No + "</a>");

                this.Pub1.AddTD(sem.Name); //描述.


                this.Pub1.AddTD(sem.TableDesc); //描述.

                //编码表类型.
                this.Pub1.AddTD(sem.CodeStructT);

                if (sem.SrcType == SrcType.TableOrView)
                {
                    if (sem.No.Contains("."))
                        this.Pub1.AddTD("<a href=\"javascript:WinOpen('/WF/Comm/Search.aspx?EnsName=" + sem.No + "');\" >编辑</a>");
                    else
                        this.Pub1.AddTD("<a href=\"javascript:WinOpen('SFTableEditData.aspx?FK_SFTable=" + sem.No + "');\" >编辑</a>");
                }
                else 
                {
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('SFSQLDataView.aspx?FK_SFTable=" + sem.No + "');\" >查看</a>");
                }

                this.Pub1.AddTD("<a  href=\"javascript:WinOpen('SFTable.aspx?FK_MapData=" + this.FK_MapData + "&Idx=" + this.Idx + "&FK_SFTable=" + sem.No + "&GroupField=" + this.GroupField + "','sg')\"  ><img src='../../Img/Btn/Edit.gif' border=0/>属性</a>");

                this.Pub1.AddTD(sem.FK_SFDBSrc);  //描述.
                this.Pub1.AddTD(sem.SrcTypeText); //数据类型.
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}