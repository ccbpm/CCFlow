using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    public partial class UISysEnum : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 分组
        /// </summary>
        public string GroupField
        {
            get
            {
                return this.Request.QueryString["GroupField"];
            }
        }
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
        /// <summary>
        /// 顺序号
        /// </summary>
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }

        public string EnumKey
        {
            get
            {
                return this.Request.QueryString["EnumKey"];
            }
        }
        #endregion 

        protected void Page_Load(object sender, EventArgs e)
        {

            SysEnumMain main = new SysEnumMain();
            if (this.EnumKey != null)
            {
                main.No = this.EnumKey;
                main = new SysEnumMain(this.EnumKey);
            }

            SysEnums ses = new SysEnums();
            if (main.No.Length > 0)
            {
                ses.Retrieve(SysEnumAttr.EnumKey, main.No);
            }

            if (this.IsPostBack == false)
            {
                this.Btn_SaveAndAddFrm.Enabled = false;
                this.Btn_Del.Enabled = false;
                this.Btn_SaveAndAddFrm.Enabled = false;

                if (this.EnumKey != null)
                {
               
                    this.TB_No.Text = this.EnumKey;
                    this.TB_No.Enabled = false;

                    this.Btn_Del.Enabled = true;
                    if (this.FK_MapData != null)
                    {
                        this.Btn_SaveAndAddFrm.Enabled = true;
                    }
                }
                this.TB_Name.Text = main.Name;
            }

            this.Pub1.AddTable("width=100%");
            if (this.EnumKey == null)
                this.Pub1.AddCaptionLeft("<a href='FieldTypeList.htm?DoType=AddF&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>&nbsp;返回</a> - <a href='Do.aspx?DoType=AddSysEnum&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'>枚举字段</a> - 新建");
            else
                this.Pub1.AddCaptionLeft("<a href='FieldTypeList.htm?DoType=AddF&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>&nbsp;返回</a> - <a href='Do.aspx?DoType=AddSysEnum&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'>枚举字段</a> - 编辑");

            if (this.EnumKey == null)
                this.Title = "新建枚举";
            else
                this.Title = "编辑枚举类型";

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("枚举值;");
            this.Pub1.AddTDTitle("标签");
            this.Pub1.AddTREnd();

            int idx = 0;
            bool istr = false;
            while (idx < 100)
            {
                istr = this.Pub1.AddTR(istr);
                this.Pub1.AddTDIdx(idx);
                BP.Web.Controls.TB tb = new BP.Web.Controls.TB();
                tb.ID = "TB_" + idx;
                tb.Columns = 50;
                SysEnum se = ses.GetEntityByKey(SysEnumAttr.IntKey, idx) as SysEnum;
                if (se != null)
                    tb.Text = se.Lab;

                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();
                idx++;
            }
            this.Pub1.AddTableEnd();

        }
        void btn_Add_Click(object sender, EventArgs e)
        {
            //SysEnumMain sem1 = new SysEnumMain(this.EnumKey);
            //MapAttr attrAdd = new MapAttr();
            //attrAdd.KeyOfEn = sem1.No;
            //if (attrAdd.IsExit(MapAttrAttr.FK_MapData, this.MyPK, MapAttrAttr.KeyOfEn, sem1.No))
            //{
            //    BP.Sys.PubClass.Alert("字段已经存在 [" + sem1.No + "]。");
            //    return;
            //}
            //attrAdd.FK_MapData = this.MyPK;
            //attrAdd.Name = sem1.Name;
            //attrAdd.UIContralType = UIContralType.DDL;
            //attrAdd.UIBindKey = sem1.No;
            //attrAdd.MyDataType = BP.DA.DataType.AppInt;
            //attrAdd.LGType = FieldTypeS.Enum;
            //attrAdd.DefVal = "0";
            //attrAdd.UIIsEnable = true;
            //attrAdd.Insert();
            //// http://localhost:41466/WF/Admin/FoolFormDesigner/EditEnum.aspx?DoType=Edit&FK_MapData=ND17501&EnumKey=CeShiMoShi&IDX=
            
            this.Response.Redirect("EditEnum.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&EnumKey=" + this.EnumKey, true);
            // this.WinClose();
            return;
        }
        bool isSaveOK = false;
        public void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SysEnumMain main = new SysEnumMain();
                main.Name = this.TB_Name.Text;
                main.No = this.TB_No.Text;

                if (this.EnumKey == null)
                {
                    main.No = this.TB_No.Text;
                    if (main.IsExits)
                    {
                        this.Alert("编号（枚举英文名称）[" + main.No + "]已经存在。");
                        isSaveOK = false;
                        return;
                    }

                    SysEnum se = new SysEnum();
                    if (se.IsExit(SysEnumAttr.EnumKey, main.No) == true)
                    {
                        this.Alert("编号（枚举英文名称）[" + main.No + "]已经存在。");
                        isSaveOK = false;
                        return;
                    }
                    main.Name = this.TB_Name.Text;
                }

                if (main.No.Length == 0 || main.Name.Length == 0)
                    throw new Exception("编号与名称不能为空");

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
                    throw new Exception("错误：您必须输入枚举值，请参考帮助。"); //错误：您必须输入枚举值，请参考帮助。

                main.Save();

                //重新生成
                SysEnums se1s = new SysEnums();
                se1s.Delete(SysEnumAttr.EnumKey, main.No);
                SysEnums ses = new SysEnums();
                ses.RegIt(main.No, cfgVal);

                string keyApp = "EnumOf" + main.No + WebUser.SysLang;
                BP.DA.Cash.DelObjFormApplication(keyApp);

                if (this.EnumKey == null)
                    this.Response.Redirect("SysEnum.aspx?EnumKey=" + main.No + "&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "&GroupField=" + this.GroupField, true);
                isSaveOK = true;
                return;
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
                isSaveOK = false;
                return;
            }
        }
        public void Btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                SysEnumMain sem = new SysEnumMain(this.EnumKey);
                sem.Delete();
                this.ToWFMsgPage("删除成功");
                return;
            }
            catch (Exception ex)
            {
                this.ToErrorPage(ex.Message);
            }
        }

        /// <summary>
        /// 保存并保存到表单里.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_SaveAndAddFrm_Click(object sender, EventArgs e)
        {
            this.Btn_Save_Click(null, null);

            if (this.isSaveOK == false)
                return;
             
            //SysEnumMain sem1 = new SysEnumMain(this.EnumKey);
            //MapAttr attrAdd = new MapAttr();
            //attrAdd.KeyOfEn = sem1.No;
            //if (attrAdd.IsExit(MapAttrAttr.FK_MapData, this.MyPK, MapAttrAttr.KeyOfEn, sem1.No))
            //{
            //    BP.Sys.PubClass.Alert("字段已经存在 [" + sem1.No + "]。");
            //    return;
            //}
            //attrAdd.FK_MapData = this.MyPK;
            //attrAdd.Name = sem1.Name;
            //attrAdd.UIContralType = UIContralType.DDL;
            //attrAdd.UIBindKey = sem1.No;
            //attrAdd.MyDataType = BP.DA.DataType.AppInt;
            //attrAdd.LGType = FieldTypeS.Enum;
            //attrAdd.DefVal = "0";
            //attrAdd.UIIsEnable = true;
            //attrAdd.Insert();


            this.Response.Redirect("EditEnum.aspx?DoType=New&FK_MapData="+this.FK_MapData+"&EnumKey="+this.EnumKey+"&GroupField="+this.GroupField, true);

        }

        /// <summary>
        /// 保存并关闭.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            this.Btn_Save_Click(null, null);

            if (this.isSaveOK == false)
                return;
            
            this.WinClose();
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.TB_No.Enabled == false)
                return;

            if (this.TB_Name.Text == "" || this.TB_Name.Text == null)
                return;

            this.TB_No.Text = BP.DA.DataType.ParseStringToPinyin(this.TB_Name.Text);
        }

     
    }
}