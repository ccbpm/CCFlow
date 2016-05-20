using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class DTSBTableExt : System.Web.UI.Page
    {

        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string FK_DBSrc
        {
            get
            {
                return this.Request.QueryString["FK_DBSrc"];
            }
        }
        public string TableName
        {
            get
            {
                return this.Request.QueryString["TableName"];
            }
        }
        #endregion 属性.


        protected void Page_Load(object sender, EventArgs e)
        {

            string rpt = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            MapAttrs attrs = new MapAttrs(rpt);

            Flow fl = new Flow(this.FK_Flow);
            fl.RetrieveFromDBSources();

            fl.No = this.FK_Flow;
            if (string.IsNullOrEmpty(this.TableName) == true)
            {
                this.Pub1.AddFieldSet("配置错误", "请关闭该窗口，在流程属性里配置业务表名，然后点保存按钮，之后打开该功能界面。");
                return;
            }

            //获得数据表列.
            SFDBSrc src = new SFDBSrc(this.FK_DBSrc);
            DataTable dt = src.GetColumns(this.TableName);

            foreach (DataRow dr in dt.Rows)
            {
                string fType = dr["DBType"].ToString();
                string desc = dr["Name"].ToString();

                dr["Name"] = dr["No"] + " " + dr["DBType"] + "(" + dr["DBLength"] + ") " + dr["Name"];
            }

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("请设置流程字段与业务表字段的同步映射");
            this.Pub1.AddTR();

            string textCenter = " style='text-align:center;'";

            this.Pub1.AddTDTitle(textCenter, "序");
            this.Pub1.AddTDTitle(textCenter, "是否同步");
            this.Pub1.AddTDTitle(textCenter, "类型");
            this.Pub1.AddTDTitle(textCenter, "业务表(" + fl.DTSBTable + ")");
            this.Pub1.AddTREnd();

            int idx = 0;
            Hashtable ht = new Hashtable();
            if (string.IsNullOrEmpty(fl.DTSFields))
                fl.DTSFields = "@";

            string[] fieldArr = fl.DTSFields.Split('@');
            string[] lcArr = fieldArr[0].Split(',');
            string[] ywArr = fieldArr[1].Split(',');

            for (int i = 0; i < lcArr.Length; i++)
                ht.Add(lcArr[i], ywArr[i]);


            #region 锁定workid 在第一行上.

            //guid workid必须选择一项
            BP.Web.Controls.RadioBtn rb_workId = new BP.Web.Controls.RadioBtn();
            rb_workId.ID = "rb_workId";
            rb_workId.GroupName = "RB_KEY";
            rb_workId.Text = "OID - WorkID";

            BP.Web.Controls.RadioBtn rb_guid = new BP.Web.Controls.RadioBtn();
            rb_guid.ID = "rb_guid";
            rb_guid.GroupName = "RB_KEY";
            rb_guid.Text = "GUID";

            foreach (DictionaryEntry de in ht)
            {
                if (de.Key.ToString().ToUpper() == "OID")
                    rb_workId.Checked = true;
                else
                    rb_guid.Checked = true;
            }

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx("1");

            this.Pub1.AddTDBegin();
            this.Pub1.Add(rb_workId);
            this.Pub1.Add(rb_guid);
            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("主键<img src='../../Img/PRI/2.png' border=0/>");

            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.Width = 250;
            ddl.ID = "DDL_OID";

            ddl.Bind(dt, dt.Columns["No"].ToString(), dt.Columns["Name"].ToString());
            ddl.SetSelectItem(fl.DTSBTablePK);

            this.Pub1.AddTD(textCenter, ddl);
            this.Pub1.AddTREnd();
            #endregion 锁定workid 在第一行上.


            bool is1 = false;
            foreach (MapAttr attr in attrs)
            {
                if (attr.KeyOfEn.ToUpper() == "OID" || attr.KeyOfEn.ToUpper() == "GUID")
                    continue;

                idx++;

                is1 = this.Pub1.AddTR(is1);

                this.Pub1.AddTDIdx(idx);

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + attr.KeyOfEn;
                cb.Text = attr.KeyOfEn + " - " + attr.Name;

                foreach (DictionaryEntry de in ht)
                {
                    if (attr.KeyOfEn == de.Key.ToString())
                        cb.Checked = true;
                }

                this.Pub1.AddTD(cb);
                this.Pub1.AddTD(attr.MyDataTypeStr);

                ddl = new BP.Web.Controls.DDL();
                ddl.ID = "DDL_" + attr.KeyOfEn;
                ddl.Width = 250;

                ddl.Bind(dt, dt.Columns["No"].ToString(), dt.Columns["Name"].ToString());

                if (cb.Checked == true)
                {
                    try
                    {
                        ddl.SetSelectItem(ht[attr.KeyOfEn].ToString());
                    }
                    catch
                    {
                    }
                }

                //类似的默认选中  区分大小写  方法if(ddl.SetSelectItem(attr.KeyOfEn)){cb.Checked = true;}不适用
                foreach (DataRow dr in dt.Rows)
                {
                    if (attr.KeyOfEn.ToUpper() == dr[0].ToString().ToUpper())
                    {
                        ddl.SetSelectItem(dr[0].ToString());
                        cb.Checked = true;
                        break;
                    }
                }

                this.Pub1.AddTD(textCenter, ddl);
                this.Pub1.AddTREnd();
            }


            this.Pub1.AddTableEnd();

            this.Pub1.AddBR();
            this.Pub1.AddBR();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";

            btn.Click += new EventHandler(btn_Save_Click);

            Button btnClose = new Button();
            btnClose.ID = "Btn_Close";
            btnClose.Text = "取消";
            btnClose.Click += new EventHandler(btnClose_Click);

            this.Pub1.Add(btn);
            this.Pub1.Add(btnClose);

            this.Pub1.AddBR();
            this.Pub1.AddBR();
            this.Pub1.AddBR();
        }
        /// <summary>
        /// 执行数据的保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                string rpt = "ND" + int.Parse(this.FK_Flow) + "Rpt";
                Flow fl = new Flow(this.FK_Flow);
                MapAttrs attrs = new MapAttrs(rpt);

                #region 求业务表的主键。
                string pk = this.Pub1.GetDDLByID("DDL_OID").SelectedItemStringVal;
                if (string.IsNullOrEmpty(pk) == true)
                {
                    BP.Sys.PubClass.Alert("@必须设置业务表的主键，否则无法同步。");
                    return;
                }
                #endregion 求业务表的主键。

                string lcStr = "";//要同步的流程字段
                string ywStr = "";//第三方字段
                string err = "";
                foreach (MapAttr attr in attrs)
                {
                    CheckBox cb = this.Pub1.GetCBByID("CB_" + attr.KeyOfEn);
                    if (cb == null || cb.Checked == false)
                        continue;

                    BP.Web.Controls.DDL ddl = this.Pub1.GetDDLByID("DDL_" + attr.KeyOfEn);


                    //如果选中的业务字段重复，抛出异常
                    if (ywStr.Contains("@" + ddl.SelectedItemStringVal + "@"))
                    {
                        err += "@配置【" + attr.KeyOfEn + " - " + attr.Name +
                            "】错误, 请确保选中业务字段的唯一性，该业务字段已经被其他字段所使用。";
                    }
                    lcStr += "@" + attr.KeyOfEn + "@,";
                    ywStr += "@" + ddl.SelectedItemStringVal + "@,";
                }

                BP.Web.Controls.RadioBtn rb = this.Pub1.GetRadioBtnByID("rb_workId");

                BP.Web.Controls.DDL ddl_key = this.Pub1.GetDDLByID("DDL_OID");
                if (rb.Checked)
                {
                    if (ywStr.Contains("@" + ddl_key.SelectedItemStringVal + "@"))
                    {
                        err += "@请确保选中业务字段的唯一性，该业务字段【" + ddl_key.SelectedItemStringVal +
                            "】已经被其他字段所使用。";
                    }
                    lcStr = "@OID@," + lcStr;
                    ywStr = "@" + ddl_key.SelectedItemStringVal + "@," + ywStr;
                }
                else
                {
                    if (ywStr.Contains("@" + ddl_key.SelectedItemStringVal + "@"))
                    {
                        err += "@请确保选中业务字段的唯一性，该业务字段【" + ddl_key.SelectedItemStringVal +
                            "】已经被其他字段所使用。";
                    }
                    lcStr = "@GUID@," + lcStr;
                    ywStr = "@" + ddl_key.SelectedItemStringVal + "@," + ywStr;
                }

                if (err != "")
                {
                    BP.Sys.PubClass.Alert(err);
                    return;
                }

                lcStr = lcStr.Replace("@", "");
                ywStr = ywStr.Replace("@", "");


                //去除最后一个字符的操作
                if (string.IsNullOrEmpty(lcStr) || string.IsNullOrEmpty(ywStr))
                {
                    BP.Sys.PubClass.Alert("要配置的内容为空...");
                    return;
                }
                lcStr = lcStr.Substring(0, lcStr.Length - 1);
                ywStr = ywStr.Substring(0, ywStr.Length - 1);


                //数据存储格式   a,b,c@a_1,b_1,c_1
                fl.DTSFields = lcStr + "@" + ywStr;
                fl.DTSBTablePK = pk;
                fl.Update();

                // System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>if (confirm('操作成功,是否关闭配置页面?'))" +
                //"{window.parent.closeTab('设置字段匹配');}</script> ");

                System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>if (confirm('操作成功,是否关闭配置页面?'))" +
                "{try{window.parent.closeTab('设置字段匹配');}catch{}}</script> ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClose_Click(object sender, EventArgs e)
        {
            //System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>window.parent.closeTab('设置字段匹配');</script> ");
            System.Web.HttpContext.Current.Response.Write("<script language='JavaScript'>"+
                "try{window.parent.closeTab('设置字段匹配');}catch{}</script> ");
        }
    }
}