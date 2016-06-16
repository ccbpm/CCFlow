using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.Admin.UC
{
    public partial class WF_Admin_UC_CondBySQL : BP.Web.UC.UCBase3
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
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
                    return this.FK_MainNode;
                }
            }
        }
        public int FK_MainNode
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_MainNode"]);
            }
        }
        public int ToNodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["ToNodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CondType HisCondType
        {
            get
            {
                return (CondType)int.Parse(this.Request.QueryString["CondType"]);
            }
        }
        public string GetOperVal
        {
            get
            {
                if (this.IsExit("TB_Val"))
                    return this.GetTBByID("TB_Val").Text;
                return this.GetDDLByID("DDL_Val").SelectedItemStringVal;
            }
        }
        public string GetOperValText
        {
            get
            {
                if (this.IsExit("TB_Val"))
                    return this.GetTBByID("TB_Val").Text;
                return this.GetDDLByID("DDL_Val").SelectedItem.Text;
            }
        }
        public string GenerMyPK
        {
            get
            {
                return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.SQL.ToString();
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            Cond cond = new Cond();
            cond.MyPK = this.GenerMyPK;
            cond.RetrieveFromDBSources();

            this.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            this.AddTR();
            this.AddTD("class='GroupTitle'", "设置SQL");
            this.AddTREnd();

            AddTR();

            TextBox tb = new TextBox();
            tb.ID = "TB_SQL";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 10;
            tb.Columns = 80;
            tb.Style.Add("width", "99%");
            tb.Text = cond.OperatorValueStr;
            AddTD("", tb);

            AddTREnd();
            AddTableEnd();

            AddBR();
            AddSpace(1);

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Click);
            this.Add(btn);
            AddSpace(1);

            btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
            btn.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
            btn.Click += new EventHandler(btn_Click);
            this.Add(btn);
            AddBR();
            AddBR();

            string help = "";
            help += "<ul>";
            help += "<li>在文本框里设置一个查询SQL，它返回一行一列。比如: SELECT COUNT(*) AS Num FROM MyTable WHERE NAME='@MyFieldName'。 </li>";
            help += "<li>该SQL参数支持系统的表达式，什么是ccflow的表达式请查看说明书。</li>";
            help += "<li>当前登录信息变量: @WebUser.No,  @WebUser.Name, @WebUser.FK_Dept.</li>";
            help += "<li>系统就会获取该返回的值把它转化为decimal类型</li>";
            help += "<li>如果该值大于零，该条件就是成立的否则不成立。</li>";
            help += "</ul>";
            AddEasyUiPanelInfo("帮助",  help);
        }

        void btn_Click(object sender, EventArgs e)
        {
            var btn = sender as LinkBtn;

            if (btn.ID == NamesOfBtn.Delete)
            {
                #region songhonggang (2014-06-15) 修改点击删除的时候删除条件
                Cond deleteCond = new Cond();
                deleteCond.Delete(CondAttr.NodeID, this.FK_MainNode,
                  CondAttr.ToNodeID, this.ToNodeID,
                  CondAttr.CondType, (int)this.HisCondType);
                #endregion
                this.Response.Redirect(this.Request.RawUrl, true);

                return;
            }

            string sql = this.GetTextBoxByID("TB_SQL").Text;

            if (string.IsNullOrEmpty(sql))
            {
                this.Alert("请填写sql语句.");
                return;
            }

            Cond cond = new Cond();
            cond.Delete(CondAttr.NodeID, this.FK_MainNode,
              CondAttr.ToNodeID, this.ToNodeID,
              CondAttr.CondType, (int)this.HisCondType);

            cond.MyPK = this.GenerMyPK;
            cond.HisDataFrom = ConnDataFrom.SQL;
            cond.NodeID = this.FK_MainNode;
            cond.FK_Node = this.FK_MainNode;
            cond.FK_Flow = this.FK_Flow;
            cond.ToNodeID = this.ToNodeID;
            cond.OperatorValue = sql;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = this.HisCondType;

            cond.Insert();

            EasyUiHelper.AddEasyUiMessager(this, "保存成功！");

            //switch (this.HisCondType)
            //{
            //    case CondType.Flow:
            //    case CondType.Node:
            //        cond.Update();
            //        this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr, true);
            //        return;
            //    case CondType.Dir:
            //        cond.ToNodeID = this.ToNodeID;
            //        cond.Update();
            //        this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
            //        return;
            //    default:
            //        throw new Exception("未设计的情况。");
            //}
        }
    }
}