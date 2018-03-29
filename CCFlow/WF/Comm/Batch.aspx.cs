using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;   
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Port;
using BP.Web.Controls;
using BP.Sys;
using BP;

namespace CCFlow.WF.Comm
{
    public partial class Comm_Batch : BP.Web.WebPage
    {
        #region 属性。
        public bool IsS
        {
            get
            {
                string str = this.Request.QueryString["IsS"];
                if (str == null || str == "0")
                    return false;
                return true;
            }
        }
        private new Entities _HisEns = null;
        public new Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                    _HisEns = ClassFactory.GetEns(this.EnsName);
                return _HisEns;
            }
        }
        public new string EnsName
        {
            get
            {
                string s = this.Request.QueryString["EnsName"];
                if (s == null)
                    throw new Exception("@参数错误 EnsName 为空.");
                return s;
            }
        }
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = en.EnMap;
            UIConfig cfg = new UIConfig(en);

            this.Title = this.HisEn.EnMap.EnDesc;

            // this.ToolBar1.AddLab("sd", this.GenerCaption(this.HisEn.EnMap.EnDesc + "" + this.HisEn.EnMap.TitleExt));
            this.ToolBar1.AddLab("sd", "<b>" + this.HisEn.EnMap.EnDesc + "</b>:");

            this.ToolBar1.InitByMapV2(map, 1);

            #region 设置选择的 默认值
            //判断是否有传进来的设置的查询条件，如果有就设置为空。
            bool isHave = false;
            AttrSearchs searchs = map.SearchAttrs;
            foreach (AttrSearch attr in searchs)
            {
                string mykey = this.Request.QueryString[attr.Key];
                if (mykey == "" || mykey == null)
                    continue;
                isHave = true;
            }

            if (isHave == true)
            {
                this.ToolBar1.GetTBByID("TB_Key").Text = "";
                /*清空现有的查询条件设置 */
                foreach (AttrSearch attr in searchs)
                {
                    string mykey = this.Request.QueryString[attr.Key];
                    if (mykey == "" || mykey == null)
                    {
                        if (attr.Key == "FK_Dept")
                            this.ToolBar1.GetDDLByKey("DDL_" + attr.Key).SetSelectItem(WebUser.FK_Dept, attr.HisAttr);
                        else
                            this.ToolBar1.GetDDLByKey("DDL_" + attr.Key).SetSelectItem("all", attr.HisAttr);
                        continue;
                    }
                    this.ToolBar1.GetDDLByKey("DDL_" + attr.Key).SetSelectItem(mykey, attr.HisAttr);
                }
            }
            #endregion

            //   this.BPToolBar1.ButtonClick += new System.EventHandler(this.ButtonClick);
            if (en.HisUAC.IsInsert)
                this.ToolBar1.AddLab("inse",
                    "<input type=button class=Btn id='ToolBar1$Btn_New' name='ToolBar1$Btn_New' onclick=\"javascript:ShowEn('./RefFunc/En.htm?EnName=" + this.EnName + "','cd','" + cfg.WinCardH + "' , '" + cfg.WinCardW + "');\"  value='新建(N)' />");

            if (WebUser.No == "admin")
                this.ToolBar1.AddLab("sw", "<input type=button class=Btn  id='ToolBar1$Btn_P' class=Btn name='ToolBar1$Btn_P'  onclick=\"javascript:OpenAttrs('" + this.EnsName + "');\"  value='设置(P)'  />");

            //this.ToolBar1.AddLab("sw", "<input type=button class=Btn  id='ToolBar1$Btn_P' name='ToolBar1$Btn_P'  onclick=\"javascript:OpenAttrs('" + this.EnsName + "');\"  value='设置(P)'  />");
            // this.ToolBar1.AddLab("s", "<input type=button onclick=\"javascript:OpenAttrs('" + this.EnsName + "');\"  value='设置(S)'  />");

            this.SetDGData();

            //输出执行信息。
            if (this.Session["Info"] != null)
            {
                if (this.Session["Info"].ToString().Contains("正在中止线程")
                    || this.Session["Info"].ToString().Contains("Thread"))
                {
                    this.Session["Info"] = null;
                }
                else
                {
                    this.ResponseWriteBlueMsg(this.Session["Info"].ToString());
                    this.Session["Info"] = null;
                }
            }

            this.ToolBar1.GetLinkBtnByID("Btn_Search").Click += new System.EventHandler(this.ButtonClick);
            //this.ToolBar1.Btn_Click();
            //   this.GenerLabel(this.Lab1, this.HisEn);
            //this.GenerLabel(this.Lab1, "<b>" + map.EnDesc + "</b>" + map.TitleExt);
            this.UCSys2.Add("<a href='./Sys/EnsDataIO.aspx?EnsName=" + this.EnsName + "' target=_blank><img src='../Img/Btn/Excel.gif' border=0 />导入/导出</a>");
        }
        private void ButtonClick(object sender, System.EventArgs e)
        {
            this.UCSys1.Clear();
            this.UCSys2.Clear();
            this.UCSys3.Clear();
            this.SetDGData(1, true);

            //Entities ens = this.HisEns;
            //Entity en = ens.GetNewEntity;
            //QueryObject qo = new QueryObject(ens);
            //qo = this.ToolBar1.GetnQueryObject(ens, en);
            this.Response.Redirect("Batch.aspx?EnsName=" + this.EnsName, true);
        }

        #region 处理
        public Entities SetDGData()
        {
            return this.SetDGData(this.PageIdx, false);
        }
        public Entities SetDGData(int pageIdx, bool isSearch)
        {
            //  this.BPToolBar1.SaveSearchState(this.EnsName, this.Key);
            this.ToolBar1.SaveSearchState(this.EnsName, this.Key);

            Entities ens = this.HisEns;
            Entity en = ens.GetNewEntity;
            QueryObject qo = new QueryObject(ens);
            qo = this.ToolBar1.GetnQueryObject(ens, en);
            string url = this.Request.RawUrl;
            if (url.IndexOf("PageIdx") != -1)
                url = url.Substring(0, url.IndexOf("PageIdx") - 1);

            BP.Sys.EnCfg encfg = new EnCfg(this.EnsName);

            this.UCSys2.Clear();
            int maxPageNum = 0;
            try
            {
                maxPageNum = this.UCSys2.BindPageIdx(qo.GetCount(), encfg.PageSizeOfBatch, pageIdx, "Batch.aspx?EnsName=" + this.EnsName);
            }
            catch (Exception ex)
            {
                en.CheckPhysicsTable();
                throw ex;
            }

            if (isSearch)
                return null;


            if (maxPageNum > 1)
                this.UCSys2.Add( "翻页键:← → PageUp PageDown");

            qo.DoQuery(en.PK, encfg.PageSizeOfBatch, pageIdx);

            this.UCSys1.DataPanelDtlCheckBox(ens);

            //if (this.IsS == false)
            //    this.UCSys3.Add("[<a href='Batch.aspx?EnsName=" + this.EnsName + "&PageIdx=" + this.PageIdx + "&IsS=1'>选择全部</a>]&nbsp;&nbsp;");
            //else
            //    this.UCSys3.Add("[<a href='Batch.aspx?EnsName=" + this.EnsName + "&PageIdx=" + this.PageIdx + "&IsS=0'>全不选</a>]&nbsp;&nbsp;");

            RefMethods rms = en.EnMap.HisRefMethods;
            foreach (RefMethod rm in rms)
            {
                if (rm.IsCanBatch == false)
                    continue;

                Button btn = new Button();
                btn.ID = "Btn_" + rm.Index;
                btn.Text = rm.Title;
                btn.CssClass = "Btn";
                if (rm.Warning == null && rm.HisAttrs.Count == 0)
                {
                    btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
                }
                else
                {
                    if (rm.HisAttrs.Count > 0)
                        btn.Attributes["onclick"] = "";
                    else
                        btn.Attributes["onclick"] = " return confirm('" + rm.Warning + "');";
                }

                this.UCSys3.Add(btn);
                btn.Click += new EventHandler(btn_Click);
            }

            UAC uac = en.HisUAC;
            if (uac.IsDelete)
            {
                Button btn = new Button();
                btn.ID = "Btn_Del";
                btn.CssClass = "Btn";

                btn.Text = "删除";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Attributes["class"] = "Button";
                this.UCSys3.Add(btn);
                btn.Click += new EventHandler(btn_Click);
            }

            UIConfig cfg=new UIConfig(en);
            MoveToShowWay showWay = cfg.MoveToShowWay; 

            // 执行移动.
            if (showWay != MoveToShowWay.None)
            {

                string MoveTo = cfg.MoveTo; 
                if (en.EnMap.Attrs.Contains(MoveTo) == false)
                {
                    this.Alert("Moveto 字段设置错误，实体不包含字段：" + MoveTo);
                    return null;
                }


                Attr attr = en.EnMap.GetAttrByKey(MoveTo);
                if (showWay == MoveToShowWay.DDL)
                {
                    Button btnM = new Button();
                    btnM.ID = "Btn_Move";
                    btnM.CssClass = "Btn";

                    btnM.Text = "移动到";
                    btnM.Attributes["onclick"] = "return confirm('您确实要移动吗？');";
                    this.UCSys3.Add("&nbsp;&nbsp;");
                    this.UCSys3.Add(btnM);

                    btnM.Click += new EventHandler(btn_Move_Click);

                    DDL ddl = new DDL();
                    ddl.ID = "DDL_MoveTo1";
                    if (attr.IsEnum)
                    {
                        ddl.BindSysEnum(attr.Key);
                        ddl.Items.Insert(0, new ListItem("选择" + "=>" + attr.Desc, "all"));
                    }
                    else
                    {
                        EntitiesNoName ens1 = attr.HisFKEns as EntitiesNoName;
                        ens1.RetrieveAll();
                        ddl.BindEntities(ens1);
                        ddl.Items.Insert(0, new ListItem("选择" + "=>" + attr.Desc, "all"));
                    }
                    this.UCSys3.Add(ddl);
                }

                if (showWay == MoveToShowWay.Panel)
                {
                    if (attr.IsEnum)
                    {
                        SysEnums ses = new SysEnums(attr.Key);
                        foreach (SysEnum se in ses)
                        {
                            Button btn = new Button();
                            btn.CssClass = "Btn";

                            btn.ID = "Btn_Move_" + se.IntKey;
                            btn.Text = "设置" + ":" + se.Lab;
                            btn.Attributes["onclick"] = "return confirm('您确实要执行设置[" + se.Lab + "]吗？');";
                            btn.Click += new EventHandler(btn_Move_Click);
                            this.UCSys3.Add(btn);
                            this.UCSys3.Add("&nbsp;&nbsp;");
                        }
                    }
                    else
                    {
                        EntitiesNoName ens1 = attr.HisFKEns as EntitiesNoName;
                        ens1.RetrieveAll();
                        foreach (EntityNoName en1 in ens1)
                        {
                            Button btn = new Button();
                            btn.CssClass = "Btn";
                            btn.ID = "Btn_Move_" + en1.No;
                            btn.Text = "设置:" + en1.Name;
                            btn.Attributes["onclick"] = "return confirm('您确实要设置[" + en1.Name + "]吗？');";
                            btn.Click += new EventHandler(btn_Move_Click);
                            this.UCSys3.Add(btn);
                            this.UCSys3.Add("&nbsp;&nbsp;");
                        }
                    }
                }
            }

            int ToPageIdx = this.PageIdx + 1;
            int PPageIdx = this.PageIdx - 1;

            this.UCSys3.Add("<SCRIPT language=javascript>");
            this.UCSys3.Add("\t\n document.onkeydown = chang_page;");
            this.UCSys3.Add("\t\n function chang_page() {");
            //  this.UCSys3.Add("\t\n  alert(event.keyCode); ");
            if (this.PageIdx == 1)
            {
                this.UCSys3.Add("\t\n if (event.keyCode == 37 || event.keyCode == 33) alert('已经是第一页');");
            }
            else
            {
                this.UCSys3.Add("\t\n if (event.keyCode == 37  || event.keyCode == 38 || event.keyCode == 33) ");
                this.UCSys3.Add("\t\n     location='Batch.aspx?EnsName=" + this.EnsName + "&PageIdx=" + PPageIdx + "';");
            }
            if (this.PageIdx == maxPageNum)
            {
                this.UCSys3.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) alert('已经是最后一页');");
            }
            else
            {
                this.UCSys3.Add("\t\n if (event.keyCode == 39 || event.keyCode == 40 || event.keyCode == 34) ");
                this.UCSys3.Add("\t\n     location='Batch.aspx?EnsName=" + this.EnsName + "&PageIdx=" + ToPageIdx + "';");
            }

            this.UCSys3.Add("\t\n } ");
            this.UCSys3.Add("</SCRIPT>");
            return ens;
        }
        void btn_Move_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string val = "";
            if (btn.ID == "Btn_Move")
                val = this.UCSys3.GetDDLByID("DDL_MoveTo1").SelectedValue;
            else
                val = btn.ID.Substring("Btn_Move".Length + 1);

            Entity en = this.HisEns.GetNewEntity;
            Map map = en.EnMap;
            string msg = "";
            if (val == "all")
                return;

            string title = null;
            if (map.Attrs.Contains("Name"))
                title = "Name";

            if (map.Attrs.Contains("Title"))
                title = "Title";

            UIConfig cfg = new UIConfig(en);
            string moveTo = cfg.MoveTo;

            foreach (Control ctl in this.UCSys1.Controls)
            {
                if (ctl == null || ctl.ID == null || ctl.ID == "")
                    continue;
                if (ctl.ID.Contains("CB_") == false)
                    continue;
                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;
                if (cb.Checked == false)
                    continue;
                string id = ctl.ID.Substring(3);
                try
                {
                    en.PKVal = id;
                    en.Retrieve();
                    en.Update(moveTo, val);
                    if (title == null)
                        msg += "<hr>移动成功：<font color=green>" + en.PKVal + "</font>";
                    else
                        msg += "<hr>移动成功：<font color=green>" + en.PKVal + " : " + en.GetValStrByKey(title) + "</font>";
                }
                catch (Exception ex)
                {
                    msg += "<hr>移动失败：<font color=red>" + en.PKVal + ", 异常信息:" + ex.Message + "</font>";
                }
            }
            if (msg == "")
                msg = "您没有选择行...";

            this.Session["Info"] = msg;
            this.Response.Redirect("Batch.aspx?EnsName=" + this.EnsName, true);
        }
        void btn_Click(object sender, EventArgs e)
        {
            string msg = "";
            Button btn = sender as Button;
            Entity en = this.HisEns.GetNewEntity;
            if (btn.ID == "Btn_Del")
            {
                foreach (Control ctl in this.UCSys1.Controls)
                {
                    if (ctl == null || ctl.ID == null || ctl.ID == "")
                        continue;
                    if (ctl.ID.Contains("CB_") == false)
                        continue;
                    CheckBox cb = ctl as CheckBox;
                    if (cb == null)
                        continue;
                    if (cb.Checked == false)
                        continue;
                    string id = ctl.ID.Substring(3);
                    try
                    {
                        en.PKVal = id;
                        en.Delete();
                        msg += "<hr>删除成功：<font color=green>" + en.PKVal + "</font>";
                    }
                    catch (Exception ex)
                    {
                        msg += "<hr>删除错误：<font color=red>" + en.PKVal + ", 异常信息:" + ex.Message + "</font>";
                    }
                }
                if (msg == "")
                    msg = "您没有选择行...";

                this.Session["Info"] = msg;
                this.Response.Redirect("Batch.aspx?EnsName=" + this.EnsName, true);

            }

            int idx = int.Parse(btn.ID.Replace("Btn_", ""));

            string pks = null;
            BP.En.RefMethod myrm = en.EnMap.HisRefMethods[idx];
            if (myrm.HisAttrs.Count != 0)
                pks = "";

            foreach (Control ctl in this.UCSys1.Controls)
            {
                if (ctl == null || ctl.ID == null || ctl.ID == "")
                    continue;
                if (ctl.ID.Contains("CB_") == false)
                    continue;

                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;
                if (cb.Checked == false)
                    continue;

                string id = ctl.ID.Substring(3);

                if (pks == null)
                {
                    try
                    {

                        en.PKVal = id;
                        en.Retrieve();
                        BP.En.RefMethod rm = en.EnMap.HisRefMethods[idx];
                        rm.HisEn = en;
                        msg += "<hr>执行：" + en.PKVal + " 信息:<br>" + rm.Do(null);
                    }
                    catch (Exception ex)
                    {
                        msg += "<hr>执行错误：<font color=red>主键值：" + en.PKVal + "<br>" + ex.Message + "</font>";
                    }
                }
                else
                {
                    pks += id + ",";
                }
            }
            if (pks != null)
            {
                if (pks == "")
                {
                    msg = "您没有选择行...";
                    this.Session["Info"] = msg;
                    this.Response.Redirect("Batch.aspx?EnsName=" + this.EnsName, true);
                    return;
                }

             //   BP.Sys.PubClass.WinOpen("RefMethod.aspx?Index="+idx+"&EnsName="+this.EnsName+"&PK="+pks+"&r="+DateTime.Now.ToString());
                BP.Sys.PubClass.WinOpen("RefMethod.htm?Index=" + idx + "&EnsName=" + this.EnsName + "&PK=" + pks + "&r=" + DateTime.Now.ToString());
                return;
            }
          

            if (msg == "")
            {
                msg = "您没有选择行...";
                this.Session["Info"] = msg;
                this.Response.Redirect("Batch.aspx?EnsName=" + this.EnsName, true);
                return;
            }



            this.Session["Info"] = msg;
            this.Response.Redirect("Batch.aspx?EnsName=" + this.EnsName, true);

            // this.Response.Redirect(this.Request.RawUrl, true);
        }
        #endregion
    }
}