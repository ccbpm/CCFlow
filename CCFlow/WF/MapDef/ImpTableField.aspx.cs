using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;
using BP.Web.Controls;

namespace CCFlow.WF.MapDef
{
    public partial class ImpTableField : System.Web.UI.Page
    {
        #region 参数.
        public int Step
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["Step"]);
                }
                catch
                {
                    return 1;
                }
            }
        }
        public string FK_MapData
        {
            get
            {
                string str = this.Request.QueryString["FK_MapData"];
                if (string.IsNullOrEmpty(str))
                    return "abc";
                return str;
            }
        }
        public string FK_SFDBSrc
        {
            get
            {
                return this.Request.QueryString["FK_SFDBSrc"];
            }
        }
        private string _STable = null;
        public string STable
        {
            get
            {
                if (_STable == null)
                {
                    _STable = this.Request.QueryString["STable"];
                    if (_STable == null)
                    {
                        BP.En.Entity en = BP.En.ClassFactory.GetEn(this.FK_MapData);
                        if (en != null)
                            _STable = en.EnMap.PhysicsTable;
                        else
                        {
                            MapData md = new MapData(this.FK_MapData);
                            _STable = md.PTable;
                        }
                    }
                }

                if (_STable == null)
                    _STable = "";
                return _STable;
            }
        }
        public string SColumns
        {
            get { return this.Request.QueryString["SColumns"]; }
        }
        #endregion 参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Response.Redirect("ImpTableField1504.aspx?EnsName=" + this.FK_MapData,
            //    true);
            //return;

            #region 第1步.
            if (this.Step == 1)
            {
                BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
                ens.RetrieveAll();

                Pub1.AddTable("class='Table' cellSpacing='0' cellPadding='0'  border='0' style='width:100%'");
                Pub1.AddTR();
                Pub1.AddTDGroupTitle("", "第1步：请选择数据源");
                Pub1.AddTREnd();

                Pub1.AddTR();
                Pub1.AddTDBegin();
                Pub1.AddUL("class='navlist'");

                foreach (BP.Sys.SFDBSrc item in ens)
                {

                    Pub1.AddLi("<div><a href='ImpTableField.aspx?Step=2&FK_MapData=" + this.FK_MapData + "&FK_SFDBSrc=" + item.No + "'><span class='nav'>" + item.No + "  -  " + item.Name + "</span></a></div>");
                }

                Pub1.AddLi("<div><a href=\"javascript:WinOpen('../Comm/Sys/SFDBSrcNewGuide.aspx')\" ><img src='../Img/New.gif' align='middle' /><span class='nav'>新建数据源</span></a></div>");

                Pub1.AddULEnd();
                Pub1.AddTDEnd();
                Pub1.AddTREnd();
                Pub1.AddTableEnd();
            }
            #endregion 第1步.

            #region 第2步.
            if (this.Step == 2)
            {
                SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

                this.Pub1.Add("<div class='easyui-layout' data-options=\"fit:true\">");
                this.Pub1.Add(string.Format("<div data-options=\"region:'west',split:true,title:'选择 {0} 数据表/视图'\" style='width:200px;'>",
                                       src.DBName));

                DataTable dt = src.GetTables();
                this.Pub1.AddUL();
                foreach (DataRow dr in dt.Rows)
                {
                    string url = "?Step=" + this.Step + "&FK_MapData=" + this.FK_MapData + "&FK_SFDBSrc=" + this.FK_SFDBSrc + "&STable=" + dr["No"].ToString();
                    if (dr["No"].ToString() == this.STable)
                        this.Pub1.Add("<li><font color=red><b>" + dr["Name"] + "</font></b></li>");
                    else
                        this.Pub1.Add("<li><a href='" + url + "' >" + dr["Name"] + "</a></li>");
                }
                this.Pub1.AddULEnd();

                Pub1.AddDivEnd();

                this.Pub1.Add("<div data-options=\"region:'center',title:'第2步：请选择要导入的数据列（" + this.STable + "）'\" style='padding:5px;'>");

                //加载选中数据表/视图的列信息
                this.Pub1.AddTable(); // ("id='maintable' class='Table' cellSpacing='0' cellPadding='0'  border='0' style='width:100%'");
                this.Pub1.AddTR();

                var cb = new CheckBox();
                cb.ID = "CB_CheckAll";
                cb.Attributes["onclick"] = "CheckAll(this.checked)";

                this.Pub1.AddTDGroupTitle("序");
                this.Pub1.AddTDGroupTitle("style='width:40px;text-align:center'", cb);
            //    this.Pub1.AddTDGroupTitle("字段名");
                this.Pub1.AddTDGroupTitle("中文描述");
                this.Pub1.AddTDGroupTitle("style='width:80px;text-align:center'", "类型");
                this.Pub1.AddTDGroupTitle("style='width:50px;text-align:center'", "最大长度");
                this.Pub1.AddTREnd();

                MapAttrs attrs = new MapAttrs(this.FK_MapData);

                var tableColumns = src.GetColumns(this.STable);
                foreach (DataRow dr in tableColumns.Rows)
                {
                    cb = new CheckBox();
                    cb.ID = "CB_Col_" + dr["no"];
                    cb.Text = dr["no"].ToString();
                    cb.Checked = this.SColumns == null ? false : this.SColumns.Contains(dr["no"] + ",");

                    //如果已经有该字段，就放弃.
                    if (attrs.Contains(MapAttrAttr.KeyOfEn, dr["no"].ToString()))
                    {
                        continue;
                    }

                    this.Pub1.AddTR();
                    this.Pub1.AddTD(dr["Colid"].ToString());
                    this.Pub1.AddTD(cb);
                    this.Pub1.AddTD(dr["Name"].ToString());
                    this.Pub1.AddTD(dr["DBType"].ToString());
                    this.Pub1.AddTD(Convert.ToInt32(dr["DBLength"]));
                    this.Pub1.AddTREnd();
                }


                this.Pub1.AddTableEnd();
                this.Pub1.AddBR();
                this.Pub1.AddBR();
                this.Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Next, "下一步");
                btn.Click += new EventHandler(btn_Click);
                this.Pub1.Add(btn);
                this.Pub1.AddSpace(1);

                this.Pub1.Add(string.Format("<a href='{0}?Step=1&FK_MapData={1}' class='easyui-linkbutton'>上一步</a>", Request.Url.AbsolutePath, this.FK_MapData));
                this.Pub1.AddBR();
                this.Pub1.AddBR();
                this.Pub1.AddDivEnd();
                this.Pub1.AddDivEnd();
            }
            #endregion 第2步.

            #region 第3步.
            if (this.Step == 3)
            {
                SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);
                this.Pub1.AddTable(); // ("id='maintable' class='Table' cellSpacing='0' cellPadding='0' border='0' style='width:100%'");
                this.Pub1.AddTDGroupTitle("序");
                this.Pub1.AddTDGroupTitle("字段名");
                this.Pub1.AddTDGroupTitle("中文描述");
                this.Pub1.AddTDGroupTitle("数据类型");
                this.Pub1.AddTDGroupTitle("逻辑类型");
                this.Pub1.AddTDGroupTitle("绑定值(双击选择)");
                this.Pub1.AddTDGroupTitle("最大长度");
                this.Pub1.AddTDGroupTitle("顺序");
                this.Pub1.AddTREnd();

                var tableColumns = src.GetColumns(this.STable);
                var idx = 0;

                foreach (DataRow dr in tableColumns.Rows)
                {
                    if (this.SColumns.Contains(dr["no"] + ",") == false)
                        continue;

                    string typeString = dr["DBType"].ToString().ToLower();
                    int mydatatype = 1;
                    if (typeString.Contains("int"))
                        mydatatype = BP.DA.DataType.AppInt;
                    if (typeString.Contains("float"))
                        mydatatype = BP.DA.DataType.AppFloat;
                    if (typeString.Contains("double"))
                        mydatatype = BP.DA.DataType.AppDouble;

                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD(dr["no"].ToString());

                    //中文描述.
                    var tb = new TB();
                    tb.ID = "TB_Desc_" + dr["No"];
                    tb.Columns = 20;
                    tb.Text = dr["Name"].ToString();
                    if (tb.Text.Length == 0)
                        tb.Text = dr["No"].ToString();
                    this.Pub1.AddTD(tb);

                    //数据类型.
                    var ddl = new DDL();
                    ddl.ID = "DDL_DBType_" + dr["No"];
                    ddl.SelfBindSysEnum(MapAttrAttr.MyDataType);
                    ddl.SetSelectItem(mydatatype); //设置选择的项目.
                    this.Pub1.AddTD(ddl);

                    //逻辑类型.
                    ddl = new DDL();
                    ddl.ID = "DDL_LogicType_" + dr["No"];
                    ddl.SelfBindSysEnum(MapAttrAttr.LGType);
                    this.Pub1.AddTD(ddl);


                    //绑定的逻辑类型.
                    tb = new TB();
                    tb.ID = "TB_BindKey_" + dr["No"];
                    tb.Columns = 30;
                    tb.Text = dr["name"].ToString();
                    tb.Attributes["ondblclick"] = "OpenSelectBindKey(this)";
                    this.Pub1.AddTD(tb);

                    //最大长度.
                    //绑定的逻辑类型.
                    tb = new TB();
                    tb.ID = "TB_Len_" + dr["No"];
                    tb.Columns = 5;
                    tb.Text = dr["DBLength"].ToString();
                    this.Pub1.AddTD(tb);

                    this.Pub1.AddTDBegin("style='text-align:center'");
                    var hiddenIdx = new HiddenField();
                    hiddenIdx.ID = "HID_Idx_" + dr["No"];
                    hiddenIdx.Value = idx.ToString();
                    this.Pub1.Add(hiddenIdx);

                    this.Pub1.Add("<a href='javascript:void(0)' onclick='up(this, 6)' class='easyui-linkbutton' data-options=\"iconCls:'icon-up'\"></a>&nbsp;");
                    this.Pub1.Add("<a href='javascript:void(0)' onclick='down(this, 6)' class='easyui-linkbutton' data-options=\"iconCls:'icon-down'\"></a>");

                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }

                this.Pub1.AddTableEnd();
                this.Pub1.AddBR();
                this.Pub1.AddBR();
                this.Pub1.AddSpace(1);

                var btn = new LinkBtn(false, NamesOfBtn.Save, "导入字段，生成表单");
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);
                this.Pub1.AddSpace(1);

                this.Pub1.Add(string.Format("<a href='{0}' class='easyui-linkbutton'>上一步</a>", Request.Url.ToString().Replace("Step=3", "Step=2")));
                this.Pub1.AddBR();
                this.Pub1.AddBR();
            }
            #endregion 第3步.
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            var ts = new List<Tuple<string, string, int, int, int, int>>();
            var colname = string.Empty;

            HiddenField hid = null;

            MapData md=new MapData();
            md.No =this.FK_MapData;
            md.RetrieveFromDBSources();
            
            string msg="导入字段信息:";
            bool isLeft = true;
            float maxEnd = md.MaxEnd; //底部.
            foreach (Control ctrl in Pub1.Controls)
            {
                if (ctrl.ID == null || !ctrl.ID.StartsWith("HID_Idx_")) 
                    continue;

                hid = ctrl as HiddenField;
                colname = hid.ID.Substring("HID_Idx_".Length);

                MapAttr ma = new MapAttr();
                ma.KeyOfEn = colname;
                ma.Name = this.Pub1.GetTBByID("TB_Desc_" + colname).Text;
                ma.FK_MapData = this.FK_MapData;
                ma.MyDataType = this.Pub1.GetDDLByID("DDL_DBType_" + colname).SelectedItemIntVal;
                ma.MaxLen = int.Parse(this.Pub1.GetTBByID("TB_Len_" + colname).Text);
                //ma.LGType = (BP.En.FieldTypeS)this.Pub1.GetDDLByID("DDL_LogicType_" + colname).SelectedItemIntVal;
                ma.UIBindKey = this.Pub1.GetTBByID("TB_BindKey_" + colname).Text;
                ma.MyPK = this.FK_MapData + "_" + ma.KeyOfEn;
                ma.LGType = BP.En.FieldTypeS.Normal;

                if (ma.UIBindKey != "")
                {
                    SysEnums se = new SysEnums();
                    se.Retrieve(SysEnumAttr.EnumKey, ma.UIBindKey);
                    if (se.Count > 0)
                    {
                        ma.MyDataType = BP.DA.DataType.AppInt;
                        ma.LGType = BP.En.FieldTypeS.Enum;
                        ma.UIContralType = BP.En.UIContralType.DDL;
                    }

                    SFTable tb = new SFTable();
                    tb.No = ma.UIBindKey;
                    if (tb.IsExits == true)
                    {
                        ma.MyDataType = BP.DA.DataType.AppString;
                        ma.LGType = BP.En.FieldTypeS.FK;
                        ma.UIContralType = BP.En.UIContralType.DDL;
                    }
                }

                if (ma.MyDataType== BP.DA.DataType.AppBoolean)
                    ma.UIContralType = BP.En.UIContralType.CheckBok;
                if (ma.IsExits)
                    continue;
                ma.Insert();

                msg += "\t\n字段:" + ma.KeyOfEn + "" + ma.Name + "加入成功.";
                FrmLab lab = null;
                if (isLeft == true)
                {
                    maxEnd = maxEnd + 40;
                    /* 是否是左边 */
                    lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = this.FK_MapData;
                    lab.Text = ma.Name;
                    lab.X = 40;
                    lab.Y = maxEnd;
                    lab.Insert();

                    ma.X = lab.X + 80;
                    ma.Y = maxEnd;
                    ma.Update();
                }
                else
                {
                    lab = new FrmLab();
                    lab.MyPK = BP.DA.DBAccess.GenerGUID();
                    lab.FK_MapData = this.FK_MapData;
                    lab.Text = ma.Name;
                    lab.X = 350;
                    lab.Y = maxEnd;
                    lab.Insert();

                    ma.X = lab.X + 80;
                    ma.Y = maxEnd;
                    ma.Update();
                }
                isLeft = !isLeft;
            }

            //重新设置.
            md.ResetMaxMinXY();

            BP.Sys.PubClass.WinClose("OK");
        }

        /// <summary>
        /// 将SQL数据库字段类型转为系统类型
        /// </summary>
        /// <param name="sqlDataType">SQL数据库字段类型</param>
        /// <returns></returns>
        private int GetMyDataType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
            {
                case "tinyint":
                case "smallint":
                case "int":
                    return DataType.AppInt;
                case "money":
                case "smallmoney":
                    return DataType.AppMoney;
                case "float":
                case "decimal":
                case "bigint":
                case "real":
                    return DataType.AppDouble;
                case "bit":
                    return DataType.AppBoolean;
                case "datetime":
                case "smalldatetime":
                    return DataType.AppDateTime;
                case "date":
                    return DataType.AppDate;
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext":
                case "xml":
                default:
                    return DataType.AppString;
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            var selectedColumns = string.Empty;

            foreach (Control ctrl in Pub1.Controls)
            {
                if (ctrl.GetType().Name != "CheckBox" || ctrl.ID == "CB_CheckAll" || !(ctrl as CheckBox).Checked)
                    continue;

                selectedColumns += ctrl.ID.Substring("CB_Col_".Length) + ",";
            }

            Response.Redirect(string.Format(
                        "{0}?Step=3&FK_MapData={1}&FK_SFDBSrc={2}&STable={3}&SColumns={4}",
                        Request.Url.AbsolutePath, this.FK_MapData, this.FK_SFDBSrc, this.STable ?? (Pub1.GetLBByID("LB_Table").SelectedItem.Value), selectedColumns));
        }

        /// <summary>
        /// 数据源
        /// </summary>
        /// <param name="attrs">数据字符串，规则如下:
        /// <para>1.每个字段间用 ^ 来分隔</para>
        /// <para>2.字段的信息间用 ~ 来分隔</para>
        /// <para>3.字段的信息分别为：英文名称，中文名称，数据类型，最大长度，逻辑类型，序号</para>
        /// </param>
        /// <param name="tableName">数据表名称</param>
        public void InitMapAttr(string tableName, string attrs)
        {
            Pub1.AddEasyUiPanelInfo("发送信息", attrs);
            return;
            //删除有可能存在的临时数据.
            //string tempStr = tableName + "Tmp";

            //MapAttr ma = new MapAttr();
            //ma.Delete(MapAttrAttr.FK_MapData, tempStr);

            //string[] strs = attrs.Split('^');
            //foreach (string str in strs)
            //{
            //    if (string.IsNullOrEmpty(str))
            //        continue;

            //    string[] mystrs = str.Split('~');
            //    ma = new MapAttr();
            //    ma.KeyOfEn = mystrs[0];
            //    ma.Name = mystrs[1];
            //    ma.FK_MapData = tempStr;
            //    ma.MyDataType = int.Parse(mystrs[2]);
            //    ma.MaxLen = int.Parse(mystrs[3]);
            //    ma.LGType = (BP.En.FieldTypeS)int.Parse(mystrs[4]);
            //    ma.Idx = int.Parse(mystrs[5]);
            //    ma.MyPK = tempStr + "_" + ma.KeyOfEn;
            //    ma.Insert();
            //}
        }
        /// <summary>
        /// 绑定集合.
        /// </summary>
        public void BindAttrs()
        {

        }
    }
}