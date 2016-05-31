using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
using BP;
using BP.Sys;
using BP.WF.Template;

namespace CCFlow.WF.MapDef
{
    public partial class SlnDo : WebPage
    {
        #region 属性.
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string KeyOfEn
        {
            get
            {
                return this.Request.QueryString["KeyOfEn"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "DelSln": //删除sln.
                    FrmField sln = new FrmField();
                    sln.Delete(FrmFieldAttr.FK_MapData, this.FK_MapData,
                        FrmFieldAttr.KeyOfEn, this.KeyOfEn,
                        FrmFieldAttr.FK_Flow, this.FK_Flow,
                        FrmFieldAttr.FK_Node, this.FK_Node);
                    this.WinClose();
                    return;
                case "EditSln": //编辑sln.
                    this.EditSln();
                    return;
                case "Copy": //编辑sln.
                    this.Copy();
                    return;
                case "CopyIt": //编辑sln.
                    BP.WF.Glo.CopyFrmSlnFromNodeToNode(this.FK_Flow, this.FK_MapData,
                        int.Parse(this.FK_Node),
                        int.Parse(this.Request.QueryString["FromSln"]));

                    this.WinClose();
                    return;
                default:
                    break;
            }
        }
        /// <summary>
        /// 执行复制.
        /// </summary>
        public void Copy()
        {
            string sql = "SELECT NodeID, Name, Step FROM WF_Node WHERE NodeID IN (SELECT FK_Node FROM Sys_FrmSln WHERE FK_MapData='" + this.FK_MapData + "' )";
            DataTable dtNodes = BP.DA.DBAccess.RunSQLReturnTable(sql);

            this.Pub1.AddFieldSet("请选择要copy的节点.");

            this.Pub1.AddUL();
            foreach (DataRow dr in dtNodes.Rows)
            {
                string name = "步骤:" + dr[2] + ",节点ID:" + dr[0] + ":" + dr[1].ToString();
                string no = dr[0].ToString();

                if (this.FK_Node == no)
                    continue;
                else
                    this.Pub1.AddLi("<a href='SlnDo.aspx?FK_MapData=" + this.FK_MapData + "&FromSln=" + no +"&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&DoType=CopyIt' >" + name + "</a>");
            }
            this.Pub1.AddULEnd();
             
            this.Pub1.AddFieldSetEnd();

        }
        public void EditSln()
        {
            FrmField sln = new FrmField();
            int num = sln.Retrieve(FrmFieldAttr.FK_MapData, this.FK_MapData,
                         FrmFieldAttr.KeyOfEn, this.KeyOfEn,
                         FrmFieldAttr.FK_Node, this.FK_Node);

            BP.Sys.MapAttr attr = new MapAttr();
            attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData,
                      MapAttrAttr.KeyOfEn, this.KeyOfEn);

            if (num == 0)
            {
                sln.UIIsEnable = attr.UIIsEnable;
                sln.UIVisible = attr.UIVisible;
                sln.IsSigan =attr.IsSigan;
                sln.DefVal =attr.DefValReal;
            }

            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("信息");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("字段");
            this.Pub1.AddTD(attr.KeyOfEn);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("中文名");
            this.Pub1.AddTD(attr.Name);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD();
            CheckBox cb = new CheckBox();
            cb.ID = "CB_Visable";
            cb.Text = "是否可见?";
            cb.Checked = sln.UIVisible;

            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("在该方案中是否可见？");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD();
            cb = new CheckBox();
            cb.ID = "CB_Readonly";
            cb.Text = "是否只读?";
            cb.Checked = sln.UIIsEnable;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("在该方案中是否只读？");
            this.Pub1.AddTREnd();

            if ( attr.MyDataType== DataType.AppString)
            {
                /*只读，并且是String. */
                this.Pub1.AddTR();
                this.Pub1.AddTD();
                cb = new CheckBox();
                cb.ID = "CB_IsSigan";
                cb.Text = "是否是数字签名?";
                cb.Checked = sln.IsSigan;
                this.Pub1.AddTD(cb);
                this.Pub1.AddTD("如果是，并且需要在当前方案显示当前人员的签名：<br>请在默认值里输入@WebUser.No");
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTR();
            this.Pub1.AddTD("默认值");
            TextBox tb=new TextBox();
            tb.ID = "TB_DefVal";
            tb.Text = sln.DefVal;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("支持ccflow的全局变量.");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            this.Pub1.AddFieldSet("独立表单中的数字签名设置方法");
            this.Pub1.AddBR("应用概述:");
            this.Pub1.AddBR("1, 一个独立表单上会有多出数字签名. ");
            this.Pub1.AddBR("2, 这些数字签名有时会读取以前的签名，有时需要当前的数字签名。");
            this.Pub1.AddBR("3, 如果当前方案需要读取以前的数字签名，那就清除默认值信息，否则就设置@WebUser.No 获取当前操作员的数字签名。");
            this.Pub1.AddFieldSetEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            FrmField sln = new FrmField();
            sln.Retrieve(FrmFieldAttr.FK_MapData, this.FK_MapData,
                           FrmFieldAttr.KeyOfEn, this.KeyOfEn,
                           FrmFieldAttr.FK_Node, this.FK_Node);

            sln.UIIsEnable = this.Pub1.GetCBByID("CB_Readonly").Checked;
            sln.UIVisible = this.Pub1.GetCBByID("CB_Visable").Checked;

            if (this.Pub1.IsExit("CB_IsSigan"))
                sln.IsSigan = this.Pub1.GetCBByID("CB_IsSigan").Checked;

            sln.DefVal = this.Pub1.GetTextBoxByID("TB_DefVal").Text;

            sln.FK_MapData = this.FK_MapData;
            sln.KeyOfEn = this.KeyOfEn;
            sln.FK_Node = int.Parse(this.FK_Node);
            sln.FK_Flow = this.FK_Node;

            sln.MyPK = this.FK_MapData +"_"+this.FK_Flow+ "_" + this.FK_Node + "_" + this.KeyOfEn;
            sln.CheckPhysicsTable();
            sln.Save();
            this.WinClose();
        }
    }
}