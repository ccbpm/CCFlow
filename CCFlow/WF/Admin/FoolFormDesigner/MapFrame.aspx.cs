using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_MapFrame : BP.Web.WebPage
    {
        #region 属性
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
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_MapFrame
        {
            get
            {
                return this.Request.QueryString["FK_MapFrame"];
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            this.Title = md.Name + " - " + "设计框架";
            switch (this.DoType)
            {
                case "DtlList":
                    BindList(md);
                    break;
                case "New":
                    int num = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(MyPK) FROM Sys_MapFrame WHERE FK_MapData='" + this.FK_MapData + "'") + 1;
                    MapFrame dtl1 = new MapFrame();
                    dtl1.Name =   "框架"  + num;
                    dtl1.NoOfObj = "F" + num;
                    BindEdit(md, dtl1);
                    break;
                case "Edit":
                    MapFrame dtl = new MapFrame();
                    if (this.FK_MapFrame == null)
                    {
                        dtl.NoOfObj = "Frm";
                    }
                    else
                    {
                        dtl.MyPK = this.FK_MapFrame;
                        dtl.Retrieve();
                    }
                    BindEdit(md, dtl);
                    break;
                default:
                    throw new Exception("er" + this.DoType);
            }
        }
        public void BindList(MapData md)
        {
            MapFrames dtls = new MapFrames(md.No);
            if (dtls.Count == 0)
            {
                this.Response.Redirect("MapFrame.aspx?DoType=New&FK_MapData=" + this.FK_MapData + "&sd=sd", true);
                return;
            }

            if (dtls.Count == 1)
            {
                MapFrame d = (MapFrame)dtls[0];
                this.Response.Redirect("MapFrame.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&FK_MapFrame=" + d.MyPK, true);
                return;
            }

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("<a href='Designer.aspx?MyPK=" + this.MyPK + "'>返回:" + md.Name + "</a> - <a href='MapFrame.aspx?DoType=New&FK_MapData=" + this.FK_MapData + "&sd=sd'><img src='../../Img/Btn/New.gif' border=0/>新建</a>");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            TB tb = new TB();
            int i = 0;
            foreach (MapFrame dtl in dtls)
            {
                i++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(i);
                this.Pub1.AddTD(dtl.MyPK);
                this.Pub1.AddTD(dtl.Name);
                this.Pub1.AddTD("<a href='MapFrame.aspx?FK_MapData=" + this.FK_MapData + "&DoType=Edit&FK_MapFrame=" + dtl.MyPK + "'>编辑</a>");
                this.Pub1.AddTREnd();
                //continue;
                //tb = new TB();
                //tb.ID = "TB_No_" + dtl.MyPK;
                //tb.Text = dtl.MyPK;
                //this.Pub1.AddTD(tb);

                //tb = new TB();
                //tb.ID = "TB_Name_" + dtl.MyPK;
                //tb.Text = dtl.Name;
                //this.Pub1.AddTD(tb);

                //this.Pub1.AddTD("<a href='MapFrame.aspx?FK_MapData=" + this.FK_MapData + "&DoType=Edit&FK_MapFrame=" + dtl.MyPK + "'>编辑</a>");
                //this.Pub1.AddTREnd();
            }

            //this.Pub1.AddTRSum();
            //Button btn = new Button();
            //btn.ID = "Btn_Save";
            //btn.Text = "保存";
            //btn.Click += new EventHandler(btn_Click);
            //this.Pub1.AddTD("colspan=5", btn);
            //this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            try
            {
                switch (this.DoType)
                {
                    case "New":
                        MapFrame dtlN = new MapFrame();
                        dtlN = (MapFrame)this.Pub1.Copy(dtlN);
                        if (this.DoType == "New")
                        {
                            if (dtlN.IsExits)
                            {
                                this.Alert("已存在编号：" + dtlN.MyPK);
                                return;
                            }
                        }
                        dtlN.FK_MapData = this.FK_MapData;
                        dtlN.GroupID = 0;
                        dtlN.RowIdx = 0;
                        dtlN.Insert();

                        if (btn.ID.Contains("AndClose"))
                        {
                            this.WinClose();
                            return;
                        }
                        this.Response.Redirect("MapFrame.aspx?DoType=Edit&FK_MapFrame=" + dtlN.MyPK + "&FK_MapData=" + this.FK_MapData, true);
                        break;
                    case "Edit":
                        MapFrame dtl = new MapFrame(this.FK_MapFrame);
                        dtl = (MapFrame)this.Pub1.Copy(dtl);
                        if (this.DoType == "New")
                        {
                            if (dtl.IsExits)
                            {
                                this.Alert("已存在编号："  + dtl.NoOfObj);
                                return;
                            }
                        }
                        dtl.FK_MapData = this.FK_MapData;
                        dtl.IsAutoSize = this.Pub1.GetRadioBtnByID("RB_IsAutoSize_1").Checked;

                        if (this.DoType == "New")
                        {
                           
                            
                            dtl.Insert();
                        }
                        else
                            dtl.Update();

                        if (btn.ID.Contains("AndC"))
                        {
                            this.WinClose();
                            return;
                        }

                        this.Response.Redirect("MapFrame.aspx?DoType=Edit&FK_MapFrame=" + dtl.MyPK + "&FK_MapData=" + this.FK_MapData, true);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                MapFrame dtl = new MapFrame();
                dtl.MyPK = this.FK_MapFrame;
                dtl.Delete();
                this.WinClose();
                //this.Response.Redirect("MapFrame.aspx?DoType=DtlList&FK_MapData=" + this.FK_MapData, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        void btn_New_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("MapFrame.aspx?DoType=New&FK_MapData=" + this.FK_MapData, true);

            //MapData md = new MapData(this.FK_MapData);
            //MapFrames dtls = new MapFrames(md.No);
            //MapFrame d = (MapFrame)dtls[0];
            //try
            //{
            //    MapFrame dtl = new MapFrame();
            //    dtl.No = this.FK_MapFrame;
            //    dtl.Delete();
            //    this.WinClose();
            //}
            //catch (Exception ex)
            //{
            //    this.Alert(ex.Message);
            //}
        }
        void btn_Go_Click(object sender, EventArgs e)
        {
            MapFrame dtl = new MapFrame(this.FK_MapFrame);
            //  dtl.IntMapAttrs();
            this.Response.Redirect("MapFrameDe.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&FK_MapFrame=" + this.FK_MapFrame, true);
        }

        public void BindEdit(MapData md, MapFrame dtl)
        {
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTREnd();

            int idx = 1;
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("编号");
            TB tb = new TB();
            tb.ID = "TB_" + MapFrameAttr.NoOfObj;
            tb.Text = dtl.NoOfObj;
            if (this.DoType == "Edit")
                tb.Enabled = false;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();


            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("描述");
            tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = dtl.Name;
            tb.Columns = 50;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("框架连接");
            tb = new TB();
            tb.ID = "TB_URL";
            tb.Text = dtl.URL;
            tb.Columns = 50;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("框架宽度");
            tb = new TB();
            tb.ID = "TB_W";
            tb.Text = dtl.W;
            tb.ShowType = TBType.TB;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("比如: 400px , 100%");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("框架高度");
            tb = new TB();
            tb.ID = "TB_H";
            tb.ShowType = TBType.TB;
            tb.Text = dtl.H;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("比如: 600px , 800px");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTDBegin("colspan=3");

            RadioBtn rb = new RadioBtn();
            rb.Text = "指定框架宽度高度";
            rb.ID = "RB_IsAutoSize_0";
            rb.GroupName = "s";
            if (dtl.IsAutoSize)
                rb.Checked = false;
            else
                rb.Checked = true;
            this.Pub1.Add(rb);

            rb = new RadioBtn();
            rb.Text = "让框架自适应大小";
            rb.ID = "RB_IsAutoSize_1";
            rb.GroupName = "s";

            if (dtl.IsAutoSize)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();


            this.Pub1.AddTRSum();
            this.Pub1.AddTDBegin("colspan=4 align=center");

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = " 保存并关闭 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            if (this.FK_MapFrame != null)
            {
                btn = new Button();
                btn.ID = "Btn_Del";
                btn.CssClass = "Btn";
                btn.Text = "删除"; // "删除";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Del_Click);
                this.Pub1.Add(btn);

                btn = new Button();
                btn.ID = "Btn_New";
                btn.CssClass = "Btn";
                btn.Text = "新建"; // "删除";
                btn.Click += new EventHandler(btn_New_Click);
                this.Pub1.Add(btn);
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
    }
}