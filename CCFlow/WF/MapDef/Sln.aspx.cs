using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
using BP;
using BP.WF.Template;

namespace CCFlow.WF.MapDef
{
    public partial class Sln : WebPage
    {
        #region 属性.
        public string FK_MapDtl
        {
            get
            {
                return this.Request.QueryString["FK_MapDtl"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string _fk_flow = null;
        public string FK_Flow
        {
            get
            {
                if (_fk_flow == null)
                {
                    string flowNo = this.Request.QueryString["FK_Flow"];
                    if (string.IsNullOrEmpty(flowNo)==false)
                    {
                        _fk_flow = flowNo;
                    }
                    else
                    {
                        BP.WF.Node nd = new BP.WF.Node(int.Parse(this.FK_Node));
                        _fk_flow = nd.FK_Flow;
                    }
                }
                return _fk_flow;
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string KeyOfEn
        {
            get
            {
                return this.Request.QueryString["KeyOfEn"];
            }
        }
        public string Ath
        {
            get
            {
                return this.Request.QueryString["Ath"];
            }
        }
        public DataTable _dtNodes = null;
        public DataTable dtNodes
        {
            get
            {
                if (_dtNodes == null)
                {
                    string sql = "SELECT NodeID,Name,Step FROM WF_Node WHERE NodeID IN (SELECT FK_Node FROM Sys_FrmSln WHERE FK_MapData='" + this.FK_MapData + "' )";
                    _dtNodes = BP.DA.DBAccess.RunSQLReturnTable(sql);
                }
                return _dtNodes;
            }
        }
         
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 功能执行.
            if (this.DoType == "DeleteFJ")
            {
                FrmAttachment ath1 = new FrmAttachment();
                ath1.MyPK = this.FK_MapData + "_" + this.Ath + "_" + this.FK_Node;
                ath1.Delete();
                this.WinClose();
                return;
            }
            if (this.DoType == "DeleteDtl")
            {
                MapDtl dtl = new MapDtl();
                dtl.No = this.FK_MapDtl + "_" + this.FK_Node;
                dtl.Delete();
                this.WinClose();
                return;
            }
            #endregion 功能执行.


            MapData md = new MapData(this.FK_MapData);
            FrmField sln = new FrmField();
            sln.CheckPhysicsTable();

            switch (this.DoType)
            {
                case "Dtl": //附件方案.
                    this.Title = "从表权限";
                    BindDtl();
                    break;
                case "FJ": //附件方案.
                    this.Title = "表单附件权限";
                    BindFJ();
                    break;
                case "Field": //字段方案.
                default:
                    this.Title = "表单字段权限";
                    this.BindSln();
                    break;
            }
        }
        public void BindDtl()
        {
            BP.Sys.MapDtls dtls = new BP.Sys.MapDtls();
            dtls.Retrieve(MapDtlAttr.FK_MapData, this.FK_MapData);

            this.Pub2.AddTable("width='100%'");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("Idx");
            this.Pub2.AddTDTitle("编号");
            this.Pub2.AddTDTitle("名称");
            this.Pub2.AddTDTitle("原始属性");
            this.Pub2.AddTDTitle("编辑");
            this.Pub2.AddTDTitle("删除");
            this.Pub2.AddTREnd();

            int idx = 0;
            foreach (BP.Sys.MapDtl item in dtls)
            {
                if (item.FK_Node != 0)
                    continue;

                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);
                this.Pub2.AddTD(item.No);
                this.Pub2.AddTD(item.Name);
                this.Pub2.AddTD("<a href=\"javascript:EditDtlYuanShi('" + this.FK_MapData + "','" + item.No + "')\">原始属性</a>");
                this.Pub2.AddTD("<a href=\"javascript:EditDtl('" + this.FK_Node + "','" + this.FK_MapData + "','" + item.No + "')\">编辑</a>");

                MapDtl en = new MapDtl();
                en.No = item.No + "_" + this.FK_Node;
                if (en.RetrieveFromDBSources() == 0)
                    this.Pub2.AddTD();
                else
                    this.Pub2.AddTD("<a href=\"javascript:DeleteDtl('" + this.FK_Node + "','" + this.FK_MapData + "','" + item.No + "')\">删除</a>");

                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();

        }

        public void BindFJ()
        {
            BP.Sys.FrmAttachments fas = new BP.Sys.FrmAttachments();
            fas.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData);

            this.Pub2.AddTable("width='100%'");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("Idx");
            this.Pub2.AddTDTitle("编号");
            this.Pub2.AddTDTitle("名称");
            this.Pub2.AddTDTitle("附件类型");
            this.Pub2.AddTDTitle("原始属性");
            this.Pub2.AddTDTitle("编辑");
            this.Pub2.AddTDTitle("删除");
            this.Pub2.AddTREnd();

            int idx = 0;
            foreach (BP.Sys.FrmAttachment item in fas)
            {
                if (item.FK_Node != 0)
                    continue;

                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);
                this.Pub2.AddTD(item.NoOfObj);
                this.Pub2.AddTD(item.Name);
                this.Pub2.AddTD(item.UploadTypeT);
                this.Pub2.AddTD("<a href=\"javascript:EditFJYuanShi('" + this.FK_MapData + "','" + item.NoOfObj + "')\">原始属性</a>");
                this.Pub2.AddTD("<a href=\"javascript:EditFJ('"+this.FK_Node+"','"+this.FK_MapData+"','"+item.NoOfObj+"')\">编辑</a>");

                FrmAttachment en = new FrmAttachment();
                en.MyPK = this.FK_MapData + "_" + item.NoOfObj + "_" + this.FK_Node;
                if (en.RetrieveFromDBSources()==0)
                    this.Pub2.AddTD();
                else
                    this.Pub2.AddTD("<a href=\"javascript:DeleteFJ('" + this.FK_Node + "','" + this.FK_MapData + "','" + item.NoOfObj + "')\">删除</a>");

                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();
        }
        /// <summary>
        /// 绑定方案
        /// </summary>
        public void BindSln()
        {
            // 查询出来解决方案.
            FrmFields fss = new FrmFields(this.FK_MapData,int.Parse(this.FK_Node) );
           
            // 处理好.
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            this.Pub2.AddTable("80%");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("Idx");
            this.Pub2.AddTDTitle("字段");
            this.Pub2.AddTDTitle("名称");
            this.Pub2.AddTDTitle("类型");

            this.Pub2.AddTDTitle("width='90px'","可见？");
            this.Pub2.AddTDTitle("<input type='checkbox' id='s' onclick=\"CheckAll('UIIsEnable')\" />可用？");

            this.Pub2.AddTDTitle("是否是签名？");
            this.Pub2.AddTDTitle("默认值");

            this.Pub2.AddTDTitle("<input type='checkbox' id='s' onclick=\"CheckAll('IsNotNull')\" />检查必填？");
            this.Pub2.AddTDTitle("正则表达式");

            this.Pub2.AddTDTitle("<input type='checkbox' id='s' onclick=\"CheckAll('IsWriteToFlowTable')\" />写入流程数据表？");
            this.Pub2.AddTDTitle("");
            this.Pub2.AddTREnd();

            CheckBox cb = new CheckBox();
            TextBox tb = new TextBox();

            int idx = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case BP.WF.WorkAttr.RDT:
                    case BP.WF.WorkAttr.FID:
                    case BP.WF.WorkAttr.OID:
                    case BP.WF.WorkAttr.Rec:
                    case BP.WF.WorkAttr.MyNum:
                    case BP.WF.WorkAttr.MD5:
                    case BP.WF.WorkAttr.Emps:
                    case BP.WF.WorkAttr.CDT:
                        continue;
                    default:
                        break;
                }

                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);
                this.Pub2.AddTD(attr.KeyOfEn);
                this.Pub2.AddTD(attr.Name);
                this.Pub2.AddTD(attr.LGTypeT);

                FrmField sln = fss.GetEntityByKey(FrmFieldAttr.KeyOfEn, attr.KeyOfEn) as FrmField;
                if (sln == null)
                {
                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_UIVisible";
                    cb.Checked = attr.UIVisible;
                    cb.Text = "可见？";
                    this.Pub2.AddTD("width=90px", cb);

                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_UIIsEnable";
                    cb.Checked = attr.UIIsEnable;
                    cb.Text = "可用？";
                    this.Pub2.AddTD("width=90px", cb);

                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_IsSigan";
                    cb.Checked = attr.IsSigan;
                    cb.Text = "是否数字签名？";
                    this.Pub2.AddTD("width=150px", cb);

                    tb = new TextBox();
                    tb.ID = "TB_" + attr.KeyOfEn + "_DefVal";
                    tb.Text = attr.DefValReal;
                    this.Pub2.AddTD(tb);


                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn +  "_" + FrmFieldAttr.IsNotNull;
                   // cb.Checked = attr.IsNotNull;
                    cb.Checked = false;
                    cb.Text = "检查必填？";
                    this.Pub2.AddTD(cb);

                    tb = new TextBox();
                    tb.ID = "TB_" + attr.KeyOfEn + "_" + FrmFieldAttr.RegularExp ;
                    //tb.Text = attr.RegularExp;
                  //  tb.Columns = 150;
                    this.Pub2.AddTD(tb);


                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_"+FrmFieldAttr.IsWriteToFlowTable;
                    cb.Checked = false;
                    cb.Text = "是否写入流程表？";
                    this.Pub2.AddTD(cb);

                    this.Pub2.AddTD();
                    //this.Pub2.AddTD("<a href=\"javascript:EditSln('" + this.FK_MapData + "','" + this.SlnString + "','" + attr.KeyOfEn + "')\" >Edit</a>");
                }
                else
                {
                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_UIVisible";
                    cb.Checked = sln.UIVisible;
                    cb.Text = "可见？";
                    this.Pub2.AddTD("width=90px", cb);

                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_UIIsEnable";
                    cb.Checked = sln.UIIsEnable;
                    cb.Text = "可用？";
                    this.Pub2.AddTD("width=90px", cb);

                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_IsSigan";
                    cb.Checked = sln.IsSigan;
                    cb.Text = "是否数字签名？";
                    this.Pub2.AddTD("width=150px", cb);

                    tb = new TextBox();
                    tb.ID = "TB_" + attr.KeyOfEn + "_DefVal";
                    tb.Text = sln.DefVal;
                    this.Pub2.AddTD(tb);

                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_"+FrmFieldAttr.IsNotNull;
                    cb.Checked = sln.IsNotNull;
                    cb.Text = "必填？";
                    this.Pub2.AddTD(cb);

                    tb = new TextBox();
                    tb.ID = "TB_" + attr.KeyOfEn + "_RegularExp";
                    tb.Text = sln.RegularExp;
                    this.Pub2.AddTD(tb);

                    cb = new CheckBox();
                    cb.ID = "CB_" + attr.KeyOfEn + "_" + FrmFieldAttr.IsWriteToFlowTable;
                    cb.Checked = sln.IsWriteToFlowTable;
                    cb.Text = "写入流程数据表？";
                    this.Pub2.AddTD(cb);

                    this.Pub2.AddTD("<a href=\"javascript:DelSln('" + this.FK_MapData + "','"+this.FK_Flow+"','"+this.FK_Node+"','" + this.FK_Node + "','" + attr.KeyOfEn + "')\" ><img src='../Img/Btn/Delete.gif' border=0/>Delete</a>");
                }
                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Click += new EventHandler(btn_Field_Click);
            btn.Text = "保存";
            this.Pub2.Add(btn); //保存.

            if (fss.Count != 0)
            {
                btn = new Button();
                btn.ID = "Btn_Del";
                btn.Click += new EventHandler(btn_Field_Click);
                btn.Text = " Delete All ";
                btn.Attributes["onclick"] = "return confirm('Are you sure?');";
                this.Pub2.Add(btn); //删除定义..
            }

            if (dtNodes.Rows.Count >= 1)
            {
                btn = new Button();
                btn.ID = "Btn_Copy";
                btn.Click += new EventHandler(btn_Field_Click);
                btn.Text = " Copy From Node ";
                btn.Attributes["onclick"] = "CopyIt('" + this.FK_MapData + "','"+this.FK_Flow+"','" + this.FK_Node + "')";
                this.Pub2.Add(btn); //删除定义..
            }
        }

        void btn_Field_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ID == "Btn_Del")
            {
                FrmFields fss1 = new FrmFields();
                fss1.Delete(FrmFieldAttr.FK_MapData, this.FK_MapData,
                    FrmFieldAttr.FK_Node, int.Parse(this.FK_Node));
                this.Response.Redirect("Sln.aspx?FK_Flow="+this.FK_Flow+"&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&IsOk=1", true);
                return;
            }

            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            // 查询出来解决方案.
             FrmFields fss = new  FrmFields();
             fss.Delete(FrmFieldAttr.FK_MapData, this.FK_MapData, FrmFieldAttr.FK_Node, int.Parse(this.FK_Node));

            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case BP.WF.WorkAttr.RDT:
                    case BP.WF.WorkAttr.FID:
                    case BP.WF.WorkAttr.OID:
                    case BP.WF.WorkAttr.Rec:
                    case BP.WF.WorkAttr.MyNum:
                    case BP.WF.WorkAttr.MD5:
                    case BP.WF.WorkAttr.Emps:
                    case BP.WF.WorkAttr.CDT:
                        continue;
                    default:
                        break;
                }

                bool isChange = false;
                bool UIVisible = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn + "_UIVisible").Checked;
                if (attr.UIVisible != UIVisible)
                    isChange = true;

                bool UIIsEnable = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn + "_UIIsEnable").Checked;
                if (attr.UIIsEnable != UIIsEnable)
                    isChange = true;

                bool IsSigan = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn + "_IsSigan").Checked;
                if (attr.IsSigan != IsSigan)
                    isChange = true;

                string defVal = this.Pub2.GetTextBoxByID("TB_" + attr.KeyOfEn + "_DefVal").Text;
                if (attr.DefValReal != defVal)
                    isChange = true;

                bool IsNotNull = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn + "_IsNotNull").Checked;
                if (  IsNotNull==true)
                    isChange = true;

                bool IsWriteToFlowTable = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn + "_" + FrmFieldAttr.IsWriteToFlowTable).Checked;
                if (IsWriteToFlowTable == true)
                    isChange = true;

                string exp = this.Pub2.GetTextBoxByID("TB_" + attr.KeyOfEn + "_RegularExp").Text;
                if ( string.IsNullOrEmpty(exp) )
                    isChange = true;

                if (isChange == false)
                    continue;

                FrmField sln = new FrmField();
                sln.UIVisible = UIVisible;
                sln.UIIsEnable = UIIsEnable;
                sln.IsSigan = IsSigan;
                sln.DefVal = defVal;

                sln.IsNotNull = IsNotNull;
                sln.RegularExp = exp;
                sln.IsWriteToFlowTable = IsWriteToFlowTable;
                sln.FK_Node = int.Parse(this.FK_Node);
                sln.FK_Flow = this.FK_Flow;

                sln.FK_MapData = this.FK_MapData;
                sln.KeyOfEn = attr.KeyOfEn;
                sln.Name = attr.Name;

                sln.MyPK = this.FK_MapData + "_"+this.FK_Flow+"_" + this.FK_Node + "_" + attr.KeyOfEn;
                sln.Insert();
            }
            this.Response.Redirect("Sln.aspx?FK_Flow="+this.FK_Flow+"&FK_Node=" + this.FK_Node + "&FK_MapData=" + this.FK_MapData + "&IsOk=1", true);
        }
    }
}