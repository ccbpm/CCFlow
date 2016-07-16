using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;
namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_NewEnum : BP.Web.WebPage
    {
        /// <summary>
        /// 执行类型
        /// </summary>
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            SysEnumMain main = new SysEnumMain();
            if (this.RefNo != null)
            {
                main.No = this.RefNo;
                main = new SysEnumMain(this.RefNo);
                // main.Retrieve();
            }
            this.BindSysEnum(main);
        }
        public void BindSysEnum(SysEnumMain en)
        {
            SysEnums ses = new SysEnums();
            if (en.No.Length > 0)
            {
                //ses = new SysEnums(en.No);
                ses.Retrieve(SysEnumAttr.EnumKey, en.No);
            }

            this.Pub1.AddTable();
            if (this.RefNo == null)
                this.Pub1.AddCaptionLeft("<a href='Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>&nbsp;返回</a> - <a href='Do.aspx?DoType=AddSysEnum&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'>枚举字段</a> - 新建");
            else
                this.Pub1.AddCaptionLeft("<a href='Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>&nbsp;返回</a> - <a href='Do.aspx?DoType=AddSysEnum&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'>枚举字段</a> - 编辑");

            if (this.RefNo == null)
                this.Title = "新建枚举";
            else
                this.Title = "编辑枚举类型";
          

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("&nbsp;");
            this.Pub1.AddTDTitle("&nbsp;");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("编号");
            BP.Web.Controls.TB tb = new BP.Web.Controls.TB();
            tb.ID = "TB_No";
            tb.Text = en.No;
            if (this.RefNo == null)
                tb.Enabled = true;
            else
                tb.Enabled = false;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD( "枚举英文名称");
            this.Pub1.AddTREnd();


            this.Pub1.AddTRSum();
            this.Pub1.AddTD("名称");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_Name";
            tb.Text = en.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD(  "枚举中文名称");
            this.Pub1.AddTREnd();

            int idx = 0;
            while (idx < 20)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                tb = new BP.Web.Controls.TB();
                tb.ID = "TB_" + idx;
                SysEnum se = ses.GetEntityByKey(SysEnumAttr.IntKey, idx) as SysEnum;
                if (se != null)
                    tb.Text = se.Lab;
                //   tb.Text = en.Name;
                this.Pub1.AddTD(tb);
                this.Pub1.AddTD("");
                this.Pub1.AddTREnd();
                idx++;
            }

            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD colspan=3 align=center>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_Add";
            btn.Text =  "添加到表单"; // "添加到表单";
            btn.Attributes["onclick"] = " return confirm('您确认吗？');";
            btn.Click += new EventHandler(btn_Add_Click);
            if (this.RefNo == null)
                btn.Enabled = false;
            this.Pub1.Add(btn);

            btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_Del";
            btn.Text = " 删除 ";
            btn.Attributes["onclick"] = " return confirm('您确认吗？');";
            if (this.RefNo == null)
                btn.Enabled = false;

            btn.Click += new EventHandler(btn_Del_Click);
            this.Pub1.Add(btn);

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        void btn_Add_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("Do.aspx?DoType=AddEnum&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&EnumKey=" + this.RefNo, true);
            this.WinClose();
            return;
        }
        void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SysEnumMain main = new SysEnumMain();
                if (this.RefNo == null)
                {
                    main.No = this.Pub1.GetTBByID("TB_No").Text;
                    if (main.IsExits)
                    {
                        //this.Alert("编号（枚举英文名称）[" + main.No + "]已经存在。");
                        this.Alert("编号（枚举英文名称）[" + main.No + "]已经存在。");
                        return;
                    }

                    SysEnum se = new SysEnum();
                    if (se.IsExit(SysEnumAttr.EnumKey, main.No) == true)
                    {
                        this.Alert("编号（枚举英文名称）[" + main.No + "]已经存在。");
                        return;
                    }

                    main = (SysEnumMain)this.Pub1.Copy(main);
                    if (main.No.Length == 0 || main.Name.Length == 0)
                        throw new Exception("编号与名称不能为空");
                }
                else
                {
                    main.No = this.RefNo;
                    main.Retrieve();
                    main = (SysEnumMain)this.Pub1.Copy(main);
                    if (main.No.Length == 0 || main.Name.Length == 0)
                        throw new Exception( "编号与名称不能为空");
                }

                string cfgVal = "";
                int idx = -1;
                while (idx < 19)
                {
                    idx++;
                    string t = this.Pub1.GetTBByID("TB_" + idx).Text.Trim();
                    if (t.Length == 0)
                        continue;

                    cfgVal += "@" + idx + "=" + t;
                }

                main.CfgVal = cfgVal;
                if (main.CfgVal == "")
                    throw new Exception( "错误：您必须输入枚举值，请参考帮助。"); //错误：您必须输入枚举值，请参考帮助。

                main.Save();

                //重新生成
                SysEnums se1s = new SysEnums();
                se1s.Delete(SysEnumAttr.EnumKey, main.No);
                SysEnums ses = new SysEnums();
                ses.RegIt(main.No, cfgVal);

                string keyApp = "EnumOf" + main.No + WebUser.SysLang;
                BP.DA.Cash.DelObjFormApplication(keyApp);

                if (this.MyPK != null)
                    this.Response.Redirect("SysEnum.aspx?RefNo=" + main.No + "&MyPK=" + this.MyPK + "&IDX=" + this.IDX, true);
                return;
            }
            catch (Exception ex)
            {
                this.ResponseWriteBlueMsg(ex.Message);
                //this.ToErrorPage(ex.Message);
                //this.Alert(ex.Message);
            }

        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                // 检查这个类型是否被使用？
                MapAttrs attrs = new MapAttrs();
                QueryObject qo = new QueryObject(attrs);
                qo.AddWhere(MapAttrAttr.MyDataType, (int)FieldTypeS.Enum);
                qo.addAnd();
                qo.AddWhere(MapAttrAttr.KeyOfEn, this.RefNo);
                int i = qo.DoQuery();
                if (i == 0)
                {
                    BP.Sys.SysEnums ses = new SysEnums();
                    ses.Delete(BP.Sys.SysEnumAttr.EnumKey, this.RefNo);

                    BP.Sys.SysEnumMain m = new SysEnumMain();
                    m.No = this.RefNo;
                    m.Delete();
                    this.ToWFMsgPage("删除成功");
                    return;
                }

                string msg =   "错误:下列数据已经引用了枚举您不能删除它。"; // "错误:下列数据已经引用了枚举您不能删除它。";
                foreach (MapAttr attr in attrs)
                {
                    msg += "\t\n" + attr.Field + "" + attr.Name + " Table = " + attr.FK_MapData;
                }
                return;
            }
            catch (Exception ex)
            {
                this.ToErrorPage(ex.Message);
            }

        }
    }

}