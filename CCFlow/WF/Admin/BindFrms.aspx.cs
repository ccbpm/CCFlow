using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web.Controls;
using BP.WF;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Sys;
using BP;

namespace CCFlow.WF.Admin
{
    public partial class BindFrms : BP.Web.WebPage
    {
        #region 属性.

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
        /// <summary>
        /// 节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return int.Parse(this.Request.QueryString["FK_Flow"]);
                }
            }
        }
        private BP.WF.Node _currND = null;

        public BP.WF.Node currND
        {
            get
            {
                if (_currND == null)
                    _currND = new BP.WF.Node(this.FK_Node);
                return _currND;
            }
            set
            {
                _currND = value;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

            //注册这个枚举，防止第一次运行出错.
            BP.Sys.SysEnums ses = new SysEnums("FrmEnableRole");

            switch (this.DoType)
            {
                case "Up":
                    FrmNode fnU = new FrmNode(this.MyPK);
                    fnU.DoUp();
                    this.BindList();
                    break;
                case "Down":
                    FrmNode fnD = new FrmNode(this.MyPK);
                    fnD.DoDown();
                    this.BindList();
                    break;
                case "SelectedFrm":
                    this.SelectedFrm();
                    break;
                default:
                    this.BindList();
                    break;
            }
        }

        #region 绑定表单.
        public void SelectedFrm()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            this.Pub1.AddTable("align=left");
            this.Pub1.AddCaption("设置节点:(" + nd.Name + ")绑定的表单");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("表单编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("表/视图");
            this.Pub1.AddTREnd();

            MapDatas mds = new MapDatas();
            QueryObject obj_mds = new QueryObject(mds);
            obj_mds.AddWhere(MapDataAttr.AppType, (int)AppType.Application);
            obj_mds.addOrderBy(MapDataAttr.Name);
            obj_mds.DoQuery();

            SysFormTrees formTrees = new SysFormTrees();
            QueryObject objInfo = new QueryObject(formTrees);
            objInfo.AddWhere(SysFormTreeAttr.ParentNo, "0");
            objInfo.addOrderBy(SysFormTreeAttr.Name);
            objInfo.DoQuery();

            int idx = 0;
            foreach (SysFormTree fs in formTrees)
            {
                idx++;
                this.Pub1.AddTRSum();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD("colspan=4", fs.Name);
                this.Pub1.AddTREnd();
                foreach (MapData md in mds)
                {
                    if (md.FK_FormTree != fs.No)
                        continue;
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + md.No;
                    cb.Text = md.No;
                    cb.Checked = fns.Contains(FrmNodeAttr.FK_Frm, md.No);

                    this.Pub1.AddTD(cb);
                    if (cb.Checked)
                    {
                        this.Pub1.AddTDB("<a href=\"javascript:WinOpen('../MapDef/CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "');\" ><b>" + md.Name + "</b></a>");
                        this.Pub1.AddTDB(md.PTable);
                    }
                    else
                    {
                        this.Pub1.AddTD("<a href=\"javascript:WinOpen('../MapDef/CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "');\" >" + md.Name + "</a>");
                        this.Pub1.AddTD(md.PTable);
                    }
                    this.Pub1.AddTREnd();
                }
                AddChildNode(fs.No, mds, fns);
            }
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存并设置绑定方案属性";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SaveFlowFrms_Click);
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=4", btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        private void AddChildNode(string parentNo, MapDatas mds, FrmNodes fns)
        {
            SysFormTrees formTrees = new SysFormTrees();
            QueryObject objInfo = new QueryObject(formTrees);
            objInfo.AddWhere(SysFormTreeAttr.ParentNo, parentNo);
            objInfo.addOrderBy(SysFormTreeAttr.Name);
            objInfo.DoQuery();

            int idx = 0;
            foreach (SysFormTree fs in formTrees)
            {
                idx++;
                foreach (MapData md in mds)
                {
                    if (md.FK_FormTree != fs.No)
                        continue;
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + md.No;
                    cb.Text = md.No;
                    cb.Checked = fns.Contains(FrmNodeAttr.FK_Frm, md.No);

                    this.Pub1.AddTD(cb);
                    this.Pub1.AddTD(md.Name);
                    this.Pub1.AddTD(md.PTable);
                    this.Pub1.AddTREnd();
                }
                AddChildNode(fs.No, mds, fns);
            }
        }
        void btn_SaveFlowFrms_Click(object sender, EventArgs e)
        {
            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            MapDatas mds = new MapDatas();
            mds.Retrieve(MapDataAttr.AppType, (int)AppType.Application);
            //BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string ids = ",";
            foreach (MapData md in mds)
            {
                CheckBox cb = this.Pub1.GetCBByID("CB_" + md.No);
                if (cb == null || cb.Checked == false)
                    continue;
                ids += md.No + ",";
            }

            //删除已经删除的。
            foreach (FrmNode fn in fns)
            {
                if (ids.Contains("," + fn.FK_Frm + ",") == false)
                {
                    fn.Delete();
                    continue;
                }
            }

            // 增加集合中没有的。
            string[] strs = ids.Split(',');
            foreach (string s in strs)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                if (fns.Contains(FrmNodeAttr.FK_Frm, s))
                    continue;

                FrmNode fn = new FrmNode();
                fn.FK_Frm = s;
                fn.FK_Flow = this.FK_Flow;
                fn.FK_Node = this.FK_Node;
                fn.Save();
            }
            this.Response.Redirect("BindFrms.aspx?FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow, true);
        }
        #endregion 绑定表单.

        #region 设置方案.
        public void BindList()
        {
            string text = "";
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);

            #region 如果没有ndFrm 就增加上.
            bool isHaveNDFrm = false;
            foreach (FrmNode fn in fns)
            {
                if (fn.FK_Frm == "ND" + this.FK_Node)
                {
                    isHaveNDFrm = true;
                    break;
                }
            }
            if (isHaveNDFrm ==false)
            {
                FrmNode fn = new FrmNode();
                fn.FK_Flow = this.FK_Flow;
                fn.FK_Frm = "ND" + this.FK_Node;
                fn.FK_Node = this.FK_Node;

                fn.FrmEnableRole = FrmEnableRole.Disable; //就是默认不启用.
                fn.FrmSln = 0;
              //  fn.IsEdit = true;
                fn.IsEnableLoadData = true;
                fn.Insert();
                fns.AddEntity(fn);
            }
            #endregion 如果没有ndFrm 就增加上.

            string tfModel = SystemConfig.AppSettings["TreeFrmModel"];
            this.Pub1.AddTable("width=100%");
            this.Pub1.AddCaption("设置节点:(" + nd.Name + ")绑定表单");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("表单编号");
            this.Pub1.AddTDTitle("名称");
            if (tfModel == "1")
            {
                this.Pub1.AddTDTitle("关键字段");
            }

            this.Pub1.AddTDTitle("启用规则");
           // this.Pub1.AddTDTitle("可编辑否？");
            this.Pub1.AddTDTitle("可打印否？");
            this.Pub1.AddTDTitle("是否启用<br>装载填充事件");
            this.Pub1.AddTDTitle("权限控制<br>方案");
            this.Pub1.AddTDTitle("表单元素<br>自定义设置");
            this.Pub1.AddTDTitle("谁是主键？");
            this.Pub1.AddTDTitle("文件模版");

            if (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL)
               this.Pub1.AddTDTitle("是否1变N"); //add by zhoupeng 2016.03.25 for hainan.

            if (nd.HisRunModel == RunModel.SubThread)
                this.Pub1.AddTDTitle("数据汇总"); //add by zhoupeng 2016.03.25 for hainan.

            this.Pub1.AddTDTitle("顺序");
            this.Pub1.AddTDTitle("");
            this.Pub1.AddTDTitle("");
            this.Pub1.AddTREnd();

            int idx = 1;
            foreach (FrmNode fn in fns)
            {
                MapData md = new MapData();
                md.No = fn.FK_Frm;
                if (md.RetrieveFromDBSources() == 0)
                {
                    fn.Delete();  //说明该表单不存在了，就需要把这个删除掉.
                    continue;
                }

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD(fn.FK_Frm); 

                if (fn.FK_Frm=="ND"+this.FK_Node)
                     this.Pub1.AddTDB("<a href=\"javascript:WinOpen('../MapDef/CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "');\" >" + md.Name + "</a>");
                else
                     this.Pub1.AddTD("<a href=\"javascript:WinOpen('../MapDef/CCForm/Frm.aspx?FK_MapData=" + md.No + "&FK_Flow=" + this.FK_Flow + "');\" >" + md.Name + "</a>");

                DDL ddl = new DDL();
                //获取当前独立表单中的所有字段  add by 海南  zqp
                if (tfModel == "1")
                {
                    //获取它的字段集合
                    MapAttrs attrs = new MapAttrs();
                    attrs.Retrieve(MapAttrAttr.FK_MapData, md.No);

                    MapAttrs attrNs = new MapAttrs();
                    //去掉一些基础字段
                    foreach (MapAttr attr in attrs)
                    {
                        switch (attr.KeyOfEn)
                        {
                            case "Title":
                            case "FK_Emp":
                            case "MyNum":
                            case "FK_NY":
                            case WorkAttr.Emps:
                            case WorkAttr.OID:
                            case StartWorkAttr.Rec:
                            case StartWorkAttr.FID:
                                continue;
                            default:
                                break;
                        }
                        attrNs.AddEntity(attr);
                    }
                    //添加到页面中
                    DDL myddl = new DDL();
                    myddl.ID = "DDL_Attr_" + md.No;
                    myddl.BindEntities(attrNs, MapAttrAttr.KeyOfEn, MapAttrAttr.Name);
                    myddl.SetSelectItem(fn.GuanJianZiDuan);
                    this.Pub1.AddTD(myddl);
                }

                //为了扩充的需要，把下拉框的模式去掉了.
                //ddl = new DDL();
                //ddl.ID = "DDL_FrmEnableRole_" + md.No;
                //ddl.Items.Add(new ListItem("始终启用", "0"));
                //ddl.Items.Add(new ListItem("有数据时启用", "1"));
                //ddl.Items.Add(new ListItem("有参数时启用", "2"));
                //ddl.Items.Add(new ListItem("按表单字段条件", "3"));
                //ddl.Items.Add(new ListItem("按SQL表达式", "4"));
                //ddl.SetSelectItem(fn.FrmEnableRoleInt); //设置权限控制方案.
                //this.Pub1.AddTD(ddl);

                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./FlowFrm/FrmEnableRole.aspx?FK_Node="+fn.FK_Node+"&FK_MapData="+fn.FK_Frm+"')\">设置(" + fn.FrmEnableRoleText + ")</a>");

                CheckBox cb = new CheckBox();
                //cb.ID = "CB_IsEdit_" + md.No;
                //cb.Text = "可编辑否？";
                //cb.Checked = fn.IsEdit;
                //this.Pub1.AddTD(cb);

                cb = new CheckBox();
                cb.ID = "CB_IsPrint_" + md.No;
                cb.Text = "打印否？";
                cb.Checked = fn.IsPrint;
                this.Pub1.AddTD(cb);

                cb = new CheckBox();
                cb.ID = "CB_IsEnableLoadData_" + md.No;
                cb.Text = "启用否？";
                cb.Checked = fn.IsEnableLoadData;
                this.Pub1.AddTD(cb);

                ddl = new DDL();
                ddl.ID = "DDL_Sln_" + md.No;
                ddl.Items.Add(new ListItem("默认方案", "0"));
                ddl.Items.Add(new ListItem("自定义", this.FK_Node.ToString()));
                ddl.Items.Add(new ListItem("不可编辑", "1")); //让其不可编辑.

                ddl.SetSelectItem(fn.FrmSln); //设置权限控制方案.
                this.Pub1.AddTD(ddl);

                this.Pub1.AddTDBegin();
                this.Pub1.Add("<a href=\"javascript:WinField('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >字段</a>");
                this.Pub1.Add("-<a href=\"javascript:WinFJ('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >附件</a>");
                this.Pub1.Add("-<a href=\"javascript:WinDtl('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >从表</a>");

                if (md.HisFrmType == FrmType.ExcelFrm)
                    this.Pub1.Add("-<a href=\"javascript:ToolbarExcel('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >ToolbarExcel</a>");

                if (md.HisFrmType == FrmType.WordFrm)
                    this.Pub1.Add("-<a href=\"javascript:ToolbarWord('" + md.No + "','" + this.FK_Node + "','" + this.FK_Flow + "')\" >ToolbarWord</a>");

                this.Pub1.AddTDEnd();

                ddl = new DDL();
                ddl.ID = "DDL_WhoIsPK_" + md.No;
                ddl.BindSysEnum("WhoIsPK");
                ddl.SetSelectItem((int)fn.WhoIsPK); //谁是主键？.
                this.Pub1.AddTD(ddl);

                if (md.HisFrmType == FrmType.ExcelFrm || md.HisFrmType == FrmType.WordFrm)
                {
                    ddl = new DDL();
                    ddl.ID = "DDL_File_" + md.No;
                    string[] files = System.IO.Directory.GetFiles(BP.Sys.SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\", md.No + "*.xls");
                    foreach (string str in files)
                    {
                       //System.IO.FileInfo info=new System.IO.FileInfo(
                        ddl.Items.Add(new ListItem(str.Substring(str.LastIndexOf(md.No)), str ));
                    }
                    this.Pub1.AddTD(ddl);
                }
                else
                {
                    this.Pub1.AddTD("无效");
                }

                if (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL)
                {
                    cb = new CheckBox();
                    cb.ID = "CB_Is1ToN_" + md.No;
                    cb.Text = "是否1变N？";
                    cb.Checked = fn.Is1ToN;
                    this.Pub1.AddTD(cb);//add by zhoupeng 2016.03.25 for hainan.
                }

                if (nd.HisRunModel == RunModel.SubThread)
                {
                    ddl = new DDL();
                    ddl.ID = "DDL_HuiZong_" + md.No;
                    ddl.Items.Add(new ListItem("不汇总数据", "0"));

                    BP.WF.Template.FrmNodes myfns = new FrmNodes();
                    myfns.Retrieve(FrmNodeAttr.FK_Flow, nd.FK_Flow);

                    //组合这个字符串.
                    string strs = ";"+fn.FK_Frm+";";
                    foreach (BP.WF.Template.FrmNode myfrn in myfns)
                    {
                        if (strs.Contains(";" + myfrn.FK_Frm + ";") == true)
                            continue;
                        
                        strs += ";" + myfrn.FK_Frm + ";";

                        //检查该frm 是否有dtl.
                        MapDtls dtls = new MapDtls(myfrn.FK_Frm);
                        if (dtls.Count == 0)
                            continue;

                        foreach (MapDtl dtl in dtls)
                        {
                            ddl.Items.Add(new ListItem("汇总到:"+myfrn.HisFrm.Name+"-"+dtl.Name,  myfrn.HisFrm.No+"@"+dtl.No));
                        }
                    }
                    ddl.SetSelectItem(fn.HuiZong); //设置汇总..
                    this.Pub1.AddTD(ddl);
                }

                TextBox tb = new TextBox();
                tb.ID = "TB_Idx_" + md.No;
                tb.Text = fn.Idx.ToString();
                tb.Columns = 5;
                this.Pub1.AddTD(tb);

                this.Pub1.AddTDA("BindFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&MyPK=" + fn.MyPK + "&DoType=Up", "上移");
                this.Pub1.AddTDA("BindFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&MyPK=" + fn.MyPK + "&DoType=Down", "下移");

                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTableEnd();

              text = "<input type=button onclick=\"javascript:BindFrms('" + this.FK_Node + "','" + this.FK_Flow + "');\" value='修改表单绑定'  class=Btn />";
            this.Pub1.Add(text);

            Button btn = new Button();
            btn.ID = "Save";
            btn.Text = "保存";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_SavePowerOrders_Click);
            this.Pub1.Add(btn);

            text = "<input type=button onclick=\"javascript:window.close();\" value='关闭'  class=Btn />";
            this.Pub1.Add(text);
        }
        void btn_EditBindFrms_Click(object sender, EventArgs e)
        {

        }

        void btn_SavePowerOrders_Click(object sender, EventArgs e)
        {
            string tfModel = SystemConfig.AppSettings["TreeFrmModel"];
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            FrmNodes fns = new FrmNodes(this.FK_Flow, this.FK_Node);
            foreach (FrmNode fn in fns)
            {
              //  fn.IsEdit = this.Pub1.GetCBByID("CB_IsEdit_" + fn.FK_Frm).Checked;
                fn.IsPrint = this.Pub1.GetCBByID("CB_IsPrint_" + fn.FK_Frm).Checked;

                //是否启
                fn.IsEnableLoadData = this.Pub1.GetCBByID("CB_IsEnableLoadData_" + fn.FK_Frm).Checked;

                fn.Idx = int.Parse(this.Pub1.GetTextBoxByID("TB_Idx_" + fn.FK_Frm).Text);
                //fn.HisFrmType = (BP.Sys.FrmType)this.Pub1.GetDDLByID("DDL_FrmType_" + fn.FK_Frm).SelectedItemIntVal;
                if (tfModel == "1")
                {
                    if (currND.IsStartNode == false)
                    {
                         fn.GuanJianZiDuan = this.Pub1.GetDDLByID("DDL_Attr_" + fn.FK_Frm).SelectedValue;
                    }
                }

                //表单启用规则. 去掉了 2016-05-11 使用连接替代.
               // fn.FrmEnableRoleInt = this.Pub1.GetDDLByID("DDL_FrmEnableRole_" + fn.FK_Frm).SelectedItemIntVal;

                //权限控制方案.
                fn.FrmSln = this.Pub1.GetDDLByID("DDL_Sln_" + fn.FK_Frm).SelectedItemIntVal;
                fn.WhoIsPK = (WhoIsPK)this.Pub1.GetDDLByID("DDL_WhoIsPK_" + fn.FK_Frm).SelectedItemIntVal;


                fn.FK_Flow = this.FK_Flow;
                fn.FK_Node = this.FK_Node;
                 
                fn.MyPK = fn.FK_Frm + "_" + fn.FK_Node + "_" + fn.FK_Flow;

                if (fn.HisFrmType == FrmType.WordFrm || fn.HisFrmType == FrmType.ExcelFrm)
                {
                    fn.TempleteFile = this.Pub1.GetDDLByID("DDL_File_" + fn.FK_Frm).SelectedItemStringVal;
                }

                // add  for hainan 2016.3.25 增加1对N.
                if (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL)
                {
                    fn.Is1ToN = this.Pub1.GetCBByID("CB_Is1ToN_" + fn.FK_Frm).Checked;
                    if (fn.Is1ToN == true)
                    {
                        /*检查该表单里是否具有FID的隐藏字段，如果没有系统自动给他增加上.*/
                        MapAttrs attrs = new MapAttrs(fn.FK_Frm);
                        if (attrs.Contains("KeyOfEn", "FID") == false)
                        {
                            MapAttr attr = new MapAttr();
                            attr.MyPK = fn.FK_Frm + "_FID";
                            attr.FK_MapData = fn.FK_Frm;
                            attr.KeyOfEn = "FID";
                            attr.LGType = FieldTypeS.Normal;
                            attr.UIVisible = false;
                            attr.MyDataType = BP.DA.DataType.AppInt;
                            attr.Insert();
                        }
                    }
                }

                if (nd.HisRunModel == RunModel.SubThread )
                {
                    fn.HuiZong = this.Pub1.GetDDLByID("DDL_HuiZong_" + fn.FK_Frm).SelectedItemStringVal;
                }

                fn.Update();
            }
            this.Response.Redirect("BindFrms.aspx?ShowType=EditPowerOrder&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow, true);
        }
        #endregion 设置方案.


    }
}