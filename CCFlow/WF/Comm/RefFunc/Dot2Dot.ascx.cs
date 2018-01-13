using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.Sys;
using BP;

namespace CCFlow.WF.Comm.RefFunc
{
    public partial class WFDot2Dot_UC : BP.Web.UC.UCBase3
    {
        #region 属性.
        public AttrOfOneVSM AttrOfOneVSM
        {
            get
            {
                Entity en = ClassFactory.GetEn(this.EnName);
                foreach (AttrOfOneVSM attr in en.EnMap.AttrsOfOneVSM)
                {
                    if (attr.EnsOfMM.ToString() == this.AttrKey)
                    {
                        return attr;
                    }
                }
                throw new Exception("错误没有找到属性． ");
            }
        }
        /// <summary>
        /// 一的工作类
        /// </summary>
        public new string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string AttrKey
        {
            get
            {
                return this.Request.QueryString["AttrKey"];
            }
        }
        public string PK
        {
            get
            {
                if (ViewState["PK"] == null)
                {
                    string pk = this.Request.QueryString["PK"];
                    if (pk == null)
                        pk = this.Request.QueryString["No"];

                    if (pk == null)
                        pk = this.Request.QueryString["RefNo"];

                    if (pk == null)
                        pk = this.Request.QueryString["OID"];

                    if (pk == null)
                        pk = this.Request.QueryString["MyPK"];


                    if (pk != null)
                    {
                        ViewState["PK"] = pk;
                    }
                    else
                    {
                        Entity mainEn = BP.En.ClassFactory.GetEn(this.EnName);
                        ViewState["PK"] = this.Request.QueryString[mainEn.PK];
                    }
                }

                return ViewState["PK"].ToString();
            }
        }
        public DropDownList DDL_Group
        {
            get
            {
                return this.ToolBar1.GetDropDownListByID("DDL_Group");
            }
        }

        public bool IsLine
        {
            get
            {
                try
                {
                    return (bool)ViewState["IsLine"];
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                ViewState["IsLine"] = value;
            }
        }
        public string MainEnName
        {
            get
            {
                return ViewState["MainEnName"] as string;
            }
            set
            {
                this.ViewState["MainEnName"] = value;
            }
        }
        public string MainEnPKVal
        {
            get
            {
                return ViewState["MainEnPKVal"] as string;
            }
            set
            {
                this.ViewState["MainEnPKVal"] = value;
            }
        }
        public bool IsTreeShowWay
        {
            get
            {
                if (this.Request.QueryString["IsTreeShowWay"] != null)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 显示方式
        /// </summary>
        public string ShowWay
        {
            get
            {
                string str = this.Request.QueryString["ShowWay"];
                if (str == null)
                    str = this.DDL_Group.SelectedValue;
                return str;
            }
        }
        public string MainEnPK
        {
            get
            {
                return ViewState["MainEnPK"] as string;
            }
            set
            {
                this.ViewState["MainEnPK"] = value;
            }
        }
        private Entity _MainEn = null;
        public Entity MainEn
        {
            get
            {
                if (_MainEn == null)
                    _MainEn = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
                return _MainEn;
            }
            set
            {
                _MainEn = value;
            }
        }

        public int ErrMyNum = 0;
        public LinkBtn Btn_Save
        {
            get
            {
                return this.ToolBar1.GetLinkBtnByID("Btn_Save");
            }
        }
        public LinkBtn Btn_SaveAndClose
        {
            get
            {
                return this.ToolBar1.GetLinkBtnByID("Btn_SaveAndClose");
            }
        }
        /// <summary>
        /// 检索关键字
        /// </summary>
        public string Key
        {
            get
            {
                TB tb = this.ToolBar1.GetTBByID("TB_Key");

                if (tb == null)
                    return string.Empty;

                return tb.Text;
            }
        }
        #endregion 属性.

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region 处理可能来自于 父实体 的业务逻辑。
                Entity enP = ClassFactory.GetEn(this.EnName);
                this.Page.Title = enP.EnDesc;
                this.MainEnName = enP.EnDesc;
                this.MainEnPKVal = this.PK;
                this.MainEnPK = enP.PK;
                if (enP.EnMap.EnType != EnType.View)
                {
                    try
                    {
                        enP.SetValByKey(enP.PK, this.PK);// =this.PK;
                        enP.Retrieve(); //查询。
                        enP.Update(); // 执行更新，处理写在 父实体 的业务逻辑。
                    }
                    catch
                    {
                    }
                }
                MainEn = enP;
                #endregion
            }
            catch (Exception ex)
            {
                this.ToErrorPage(ex.Message);
                return;
            }

            AttrOfOneVSM ensattr = this.AttrOfOneVSM;
            this.ToolBar1.AddLab("lab_desc", "分组:");
            DropDownList ddl = new DropDownList();
            ddl.ID = "DDL_Group";
            ddl.AutoPostBack = true;
            this.ToolBar1.Add(ddl);
            ddl.Items.Clear();
            ddl.SelectedIndexChanged += new EventHandler(DDL_Group_SelectedIndexChanged);
            Entity open = ensattr.EnsOfM.GetNewEntity;

            ////如果被选择是一个树的实体.
            //if (open.IsTreeEntity)
            //{
            //    this.Response.Redirect("Dot2DotTree.htm?1=1" + this.RequestParas, true);
            //    return;
            //}
       

            Map map = open.EnMap;
            int len = 19;

            // 如果最长的 标题  〉 15 长度。就用一行显示。
            if (len > 20)
                this.IsLine = true;
            else
                this.IsLine = false;

            // 先加入enum 类型。
            foreach (Attr attr in map.Attrs)
            {
                /* map */
                if (attr.IsFKorEnum == false)
                    continue;
                this.DDL_Group.Items.Add(new ListItem(attr.Desc, attr.Key));
            }

            this.DDL_Group.Items.Add(new ListItem("无", "None"));
            foreach (ListItem li in ddl.Items)
            {
                if (li.Value == this.ShowWay)
                    li.Selected = true;
            }
            this.ToolBar1.AddSpt("spt");

            CheckBox cb = new CheckBox();
            cb.ID = "checkedAll";
            cb.Attributes["onclick"] = "SelectAll(this);";
            cb.Text = "选择全部" ;
            this.ToolBar1.Add(cb);
            cb.Dispose();
            this.DDL_Group.SelectedIndexChanged += new EventHandler(DDL_Group_SelectedIndexChanged);

            #region 处理保存权限.
            UAC uac = ensattr.EnsOfMM.GetNewEntity.HisUAC;
            if (uac.IsInsert == true || uac.IsUpdate == true)
            {
                this.ToolBar1.AddSpace(4);
                this.ToolBar1.AddLinkBtn("Btn_Save", "保存");
                //this.ToolBar1.AddBtn("Btn_Save", "保存");
                //this.Btn_Save.UseSubmitBehavior = false;
                //this.Btn_Save.OnClientClick = "this.disabled=true;";
                try
                {
                    this.ToolBar1.GetLinkBtnByID("Btn_Save").Click += new EventHandler(BPToolBar1_ButtonClick);
                    //this.ToolBar1.GetLinkBtnByID("Btn_SaveAndClose").Click += new EventHandler(BPToolBar1_ButtonClick);
                }
                catch
                {
                }
            }
            #endregion 处理保存权限.

            #region 增加关键字过滤

            TB tbKey = new TB();
            tbKey.ID = "TB_Key";

            this.ToolBar1.AddSpace(4);
            this.ToolBar1.Add("关键字：");
            this.ToolBar1.AddTB(tbKey);
            this.ToolBar1.AddSpace(2);
            this.ToolBar1.AddLinkBtn("Btn_Search", "检索");
            this.ToolBar1.GetLinkBtnByID("Btn_Search").Click += new EventHandler(BPToolBar1_ButtonClick);
            #endregion

            this.SetDataV2();
        }
        #endregion Page_Load

        #region 方法
        public void SetDataV2()
        {
            this.UCSys1.ClearViewState();
            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            //if (ensOfM.Count == 0)
                ensOfM.RetrieveAll();

            try
            {
                Entities ensOfMM = attrOM.EnsOfMM;
                QueryObject qo = new QueryObject(ensOfMM);
                qo.AddWhere(attrOM.AttrOfOneInMM, this.PK);
                qo.DoQuery();

                if (this.DDL_Group.SelectedValue == "None")
                {
                    if (this.IsLine)
                        this.UCSys1.UIEn1ToM_OneLine(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM, this.Key);
                    else
                        this.UCSys1.UIEn1ToM(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM, this.Key);
                }
                else
                {
                    if (this.IsLine)
                        this.UCSys1.UIEn1ToMGroupKey_Line(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM, this.DDL_Group.SelectedValue, this.Key);
                    else
                        this.UCSys1.UIEn1ToMGroupKey(ensOfM, attrOM.AttrOfMValue, attrOM.AttrOfMText, ensOfMM, attrOM.AttrOfMInMM, this.DDL_Group.SelectedValue, this.Key);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    ensOfM.GetNewEntity.CheckPhysicsTable();
                }
                catch (Exception ex1)
                {
                    BP.DA.Log.DefaultLogWriteLineError(ex1.Message);
                }

                this.UCSys1.ClearViewState();
                ErrMyNum++;
                if (ErrMyNum > 3)
                {
                    this.UCSys1.AddMsgOfWarning("error", ex.Message);
                    return;
                }
                this.SetDataV2();
            }
        }
        private void BPToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            var btn = (LinkBtn) sender;
            switch (btn.ID)
            {
                case NamesOfBtn.SelectNone:
                    //this.CBL1.SelectNone();
                    break;
                case NamesOfBtn.SelectAll:
                    //this.CBL1.SelectAll();
                    break;
                case NamesOfBtn.Save:
                    if (this.IsTreeShowWay)
                        SaveTree();
                    else
                        Save();

                    string str = this.Request.RawUrl;
                    if (str.Contains("ShowWay="))
                        str = str.Replace("&ShowWay=", "&1=");
                    this.Response.Redirect(str + "&ShowWay=" + this.DDL_Group.SelectedItem.Value, true);
                    return;
                case "Btn_SaveAndClose":
                    if (this.IsTreeShowWay)
                        SaveTree();
                    else
                        Save();
                    this.WinClose();
                    break;
                case "Btn_Close":
                    this.WinClose();
                    break;
                case "Btn_EditMEns":
                    this.EditMEns();
                    break;
                case "Btn_Search":
                    SetDataV2();
                    break;
                default:
                    throw new Exception("@没有找到" + btn.ID);
            }
        }
        #endregion 方法

        #region 操作
        public void EditMEns()
        {
            this.WinOpen(this.Request.ApplicationPath + "/Comm/UIEns.aspx?EnsName=" + this.AttrOfOneVSM.EnsOfM.ToString());
        }
        public void Save()
        {
            AttrOfOneVSM attr = this.AttrOfOneVSM;
            Entities ensOfMM = attr.EnsOfMM;
            //ensOfMM.Delete(attr.AttrOfOneInMM, this.PK);
            ensOfMM.Retrieve(attr.AttrOfOneInMM, this.PK);

            //edited by liuxc,2016-12-24
            //修改保存逻辑，此处不一次性删除原有保存的记录，只是将原有数据中，当前页未选中的删除掉，当前页选中但原有未记录的保存上
            string val = string.Empty;
            var keys = new List<string>();
            var key = string.Empty;

            //原有保存记录中，当前页中含有但未选中的去除掉
            foreach(Entity en in ensOfMM)
            {
                val = en.GetValStringByKey(attr.AttrOfMInMM);

                if (IsChecked(val) == false)
                    en.Delete();
            }

            //执行保存.
            // edited by liuxc,2015.1.6
            // 增加去除相同项的逻辑，比如同一个人员属于多个部门，则保存的时候则可能会选中有多个相同选择项

            foreach (System.Web.UI.Control ctl in this.UCSys1.Controls)
            {
                if (ctl == null || ctl.ID == null)
                    continue;

                if (ctl.ID.Contains("CB_") == false)
                    continue;

                CheckBox cb = (CheckBox)ctl;  
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                  key = ctl.ID.Split('_')[1];

                if (key == "EN" || key == "SE" || keys.Contains(key))
                    continue;

                if (ensOfMM.GetEntityByKey(attr.AttrOfMInMM, key) != null)
                    continue;

                Entity en1 = ensOfMM.GetNewEntity;
                en1.SetValByKey(attr.AttrOfOneInMM, this.PK);
                en1.SetValByKey(attr.AttrOfMInMM, key);
                en1.Insert();

                keys.Add(key);
            }

            //更新entity ,防止有业务逻辑出现.
            Entity enP = ClassFactory.GetEn(this.EnName);
            if (enP.EnMap.EnType != EnType.View)
            {
                enP.SetValByKey(enP.PK, this.PK);// =this.PK;
                enP.Retrieve(); //查询。
                try
                {
                    enP.Update(); // 执行更新，处理写在 父实体 的业务逻辑。
                }
                catch (Exception ex)
                {
                }
            }
        }
        public void SaveTree()
        {
            AttrOfOneVSM attr = this.AttrOfOneVSM;
            Entities ensOfMM = attr.EnsOfMM;
            ensOfMM.Delete(attr.AttrOfOneInMM, this.PK); //删除已经保存的数据。

            AttrOfOneVSM attrOM = this.AttrOfOneVSM;
            Entities ensOfM = attrOM.EnsOfM;
            ensOfM.RetrieveAll();

            Entity enP = ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
            if (enP.EnMap.EnType != EnType.View)
            {
                enP.SetValByKey(enP.PK, this.PK);// =this.PK;
                enP.Retrieve(); //查询。
                enP.Update(); // 执行更新，处理写在 父实体 的业务逻辑。
            }
        }
        #endregion

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        
        private void DDL_Group_SelectedIndexChanged(object sender, EventArgs e)
        {

            string str = this.Request.RawUrl;
            if (str.Contains("ShowWay="))
                str = str.Replace("&ShowWay=", "&1=");
            this.Response.Redirect(str + "&ShowWay=" + this.DDL_Group.SelectedItem.Value, true);
            return;

            //// this.SetDataV2();
            //CheckBox mycb = this.ToolBar1.GetCBByID("RB_Tree");
            //if (mycb == null)
            //    this.SetDataV2();
            ////else
            ////    this.BindTree();
        }
        #endregion

        public bool IsChecked(string val)
        {
            CheckBox cb = null;
            string prefix = "CB_" + val;

            if (this.DDL_Group.SelectedValue != "None")
                prefix += "_";

            foreach (Control ctrl in this.UCSys1.Controls)
            {
                cb = ctrl as CheckBox;

                if (cb == null)
                    continue;

                if (cb.ID.StartsWith(prefix) == false)
                    continue;

                return cb.Checked;
            }

            return false;
        }
    }


}