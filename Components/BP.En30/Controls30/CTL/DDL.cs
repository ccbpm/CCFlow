using System;
using System.Data;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using System.Drawing;
using BP;
using BP.Sys;
using BP.Sys.XML;
using System.ComponentModel;


namespace BP.Web.Controls
{

    /// <summary>
    /// BPListBox 的摘要说明。
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.DropDownList))]
    public class DDL : System.Web.UI.WebControls.DropDownList
    {
        #region 处理showtype
        /// <summary>
        /// 关联的Key .
        /// </summary>
        public string SelfEnsRefKey
        {
            get
            {
                return (string)ViewState["EnsKey"];
            }
            set
            {
                ViewState["EnsKey"] = value;
            }
        }
        /// <summary>
        /// 关联的Text
        /// </summary>
        public string SelfEnsRefKeyText
        {
            get
            {
                return (string)ViewState["SelfEnsRefKeyText"];
            }
            set
            {
                ViewState["SelfEnsRefKeyText"] = value;
            }
        }
        public void BindThisYearMonth()
        {
            string year = DataType.CurrentYear;
            int i = 0;
            this.Items.Clear();
            while (i < 12)
            {
                i++;
                string val = year + "-" + i.ToString().PadLeft(2, '0');
                this.Items.Add(new ListItem(val, val));
            }

            int myyear = int.Parse(year) + 1;
            year = myyear.ToString();
            i = 0;
            while (i < 12)
            {
                i++;
                string val = year + "-" + i.ToString().PadLeft(2, '0');
                this.Items.Add(new ListItem(val, val));
            }
        }
        public void BindEntitiesDel(Entities ens, string refkey, string reftext, bool isShowKey)
        {
            this.Items.Clear();
            foreach (Entity en in ens)
            {
                if (isShowKey)
                    this.Items.Add(new ListItem(en.GetValStringByKey(refkey) + " " + en.GetValStringByKey(reftext), en.GetValStringByKey(refkey)));
                else
                    this.Items.Add(new ListItem(en.GetValStringByKey(reftext), en.GetValStringByKey(refkey)));
            }
        }
        public void BindEntities(Entities ens, string refkey, string reftext, bool isShowKey, AddAllLocation where)
        {
            this.Items.Clear();
            if (where == AddAllLocation.Top || where == AddAllLocation.TopAndEnd)
            {
                ListItem li = new ListItem("-=全部=-", "all");
                this.Items.Add(li);
            }

            foreach (Entity en in ens)
            {
                if (isShowKey)
                    this.Items.Add(new ListItem(en.GetValStringByKey(refkey) + " " + en.GetValStringByKey(reftext), en.GetValStringByKey(refkey)));
                else
                    this.Items.Add(new ListItem(en.GetValStringByKey(reftext), en.GetValStringByKey(refkey)));
            }

            if (where == AddAllLocation.End || where == AddAllLocation.TopAndEnd)
            {
                ListItem li = new ListItem("-=全部=-", "all");
                this.Items.Add(li);
                //this.Items.
            }
        }
        private DDLShowType _ShowType = DDLShowType.None;
        public DDLShowType SelfShowType
        {
            get
            {
                return _ShowType;
            }
            set
            {
                this._ShowType = value;
            }
        }
        /// <summary>
        /// BindBindSysEnum
        /// </summary>
        /// <param name="enumKey"></param>
        public void BindSysEnum(string enumKey)
        {
            SelfBindSysEnum(enumKey);
        }
        public void BindSysEnum(string enumKey, int selecVal)
        {
            this.SelfDefaultVal = selecVal.ToString();
            SelfBindSysEnum(enumKey);
        }
        public void BindSysEnum(string key, bool isShowKey, AddAllLocation alllocal)
        {
            Sys.SysEnums ens = new BP.Sys.SysEnums(key);
            this.Items.Clear();
            if (alllocal == AddAllLocation.TopAndEnd
                || alllocal == AddAllLocation.Top
                || alllocal == AddAllLocation.TopAndEndWithMVal)
                this.Items.Add(new ListItem(">>", "all"));

            foreach (SysEnum en in ens)
            {
                if (this.SelfIsShowVal)
                {
                    this.Items.Add(new ListItem(en.IntKey + " " + en.Lab, en.IntKey.ToString()));
                }
                else
                {
                    ListItem li = new ListItem(en.Lab, en.IntKey.ToString());
                    //li.Attributes.CssStyle.Add("style", "color:" + en.Style);
                    //li.Attributes.Add("color", en.Style);
                    //li.Attributes.Add("style", "color:" + en.Style);

                    this.Items.Add(li);
                }
            }

            if (alllocal == AddAllLocation.TopAndEndWithMVal && this.Items.Count >= 4)
            {
                ListItem liMvals = new ListItem("*多项组合..", "mvals");
                liMvals.Attributes.CssStyle.Add("style", "color:green");
                liMvals.Attributes.Add("color", "green");
                liMvals.Attributes.Add("style", "color:green");
                this.Items.Add(liMvals); //new ListItem("*指定选项...", "mvals"));
            }

            if (alllocal == AddAllLocation.TopAndEnd
                || alllocal == AddAllLocation.End
                || alllocal == AddAllLocation.TopAndEndWithMVal)
                this.Items.Add(new ListItem("-=全部=-", "all"));
        }
        public void SelfBindSysEnum()
        {
            this.SelfBindSysEnum(this.SelfBindKey.Trim());
        }
        /// <summary>
        /// 按照enum
        /// </summary>
        public void SelfBindSysEnum(string enumKey)
        {
            this.Items.Clear();

            if (this.Enabled)
            {
                SysEnums ens = new SysEnums(enumKey);
                foreach (SysEnum en in ens)
                {
                    if (this.SelfIsShowVal)
                    {
                        this.Items.Add(new ListItem(en.IntKey + " " + en.Lab, en.IntKey.ToString()));
                    }
                    else
                    {
                        ListItem li = new ListItem(en.Lab, en.IntKey.ToString());
                        //li.Attributes.Add("color",en.Style);
                        //li.Attributes.CssStyle.Add("color",en.Style);
                        ////li.Attributes.Add("onclick","alert('hello')");
                        this.Items.Add(li);
                    }
                }
                this.SetSelectItem(this.SelfDefaultVal);
            }
            else
            {
                this.Items.Add(new ListItem(this.SelfDefaultText, this.SelfDefaultVal));
            }
        }
        public void SelfBind(string key)
        {
            this.SelfBindKey = key;
            this.Items.Clear();
            this.SelfBind();
        }
        public void SelfBind()
        {
            switch (this.SelfShowType)
            {
                case DDLShowType.None:
                    return;
                case DDLShowType.Boolean:
                    this.Items.Add(new ListItem("是", "1"));
                    this.Items.Add(new ListItem("否", "0"));
                    break;
                case DDLShowType.Gender:
                    this.Items.Add(new ListItem("男", "1"));
                    this.Items.Add(new ListItem("女", "0"));
                    break;
                case DDLShowType.SysEnum: /// 枚举类型
                    SelfBindSysEnum();
                    break;
                case DDLShowType.Self: /// 枚举类型
                    SelfBindSysEnum();
                    break;
                case DDLShowType.BindSQL: /// 于Table Bind .
                    this.SelfBindSQL();
                    break;
                case DDLShowType.Ens: /// 于实体。
                    this.SelfBindEns();
                    break;
            }

            if (this.SelfDefaultVal != null && this.SelfDefaultVal.Length > 0)
                this.SetSelectItem(this.SelfDefaultVal);

            //			if (this.Enabled==false)		
            //				this.BackColor=Color.FromName("#E4E3E6");
        }
        #endregion

        public void BindSQL(string sql, string val, string text, string selecVal)
        {
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            this.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem();
                li.Value = dr[val].ToString(); // as string;
                li.Text = dr[text].ToString(); // as string;
                //if (li.Value==selecVal)
                //    li.Selected=true;
                this.Items.Add(li);
            }
            this.SetSelectItem(selecVal);
        }
        /// <summary>
        /// 绑定atPara.
        /// </summary>
        /// <param name="AtPara"></param>
        public void BindAtParas(string atParas)
        {
            string[] strs = atParas.Split('@');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;
                string[] mystrs = str.Split('=');
                this.Items.Add(new ListItem(mystrs[1], mystrs[0]));
            }
        }
        public bool Bind(EntitiesTree ens, string selecVal)
        {
            this.Items.Clear();
            bool isHave = false;
            foreach (EntityTree en in ens)
            {
                if (en.No == selecVal)
                {
                    ListItem li = new ListItem(en.Name, en.No);
                    li.Selected = true;
                    isHave = true;
                    this.Items.Add(li);
                }
                else
                {
                    this.Items.Add(new ListItem(en.Name, en.No));
                }
            }
            return isHave;
        }
        /// <summary>
        /// 绑定一个table,并设置选择的值.
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="val">值列</param>
        /// <param name="text">标签列</param>
        /// <param name="selectVal">选中的值</param>
        /// <returns>返回是否选择成功</returns>
        public bool Bind(DataTable dt, string val="No", string text="Name", string selectVal="")
        {
            this.Items.Clear();
            if (dt.Rows.Count == 0)
            {
                ListItem li = new ListItem("无", "无");
                this.Items.Add(li);
                return false;
            }

            bool isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[val].ToString() == selectVal)
                {
                    ListItem li = new ListItem(dr[text].ToString(), dr[val].ToString());
                    li.Selected = true;
                    isHave = true;
                    this.Items.Add(li);
                }
                else
                {
                    this.Items.Add(new ListItem(dr[text].ToString(), dr[val].ToString()));
                }
            }
            return isHave;
        }
        public void SetSelectItemByIndex(int index)
        {
            foreach (ListItem li in this.Items)
            {
                li.Selected = false;
            }
            int i = 0;
            foreach (ListItem li in this.Items)
            {

                if (i == index)
                {
                    li.Selected = true;
                }
                i++;
            }

        }

        #region 处理BindKey
        /// <summary>
        /// -==全部==- 显示位置。
        /// </summary>		
        private AddAllLocation _SelfAddAllLocation = AddAllLocation.None;
        /// <summary>
        /// -==全部==- 显示位置。
        /// </summary>		 
        public AddAllLocation SelfAddAllLocation
        {
            get
            {
                return _SelfAddAllLocation;
            }
            set
            {
                _SelfAddAllLocation = value;
            }
        }
        /// <summary>
        /// 要bind的key.
        /// 这里有3种情况。
        /// 1，枚举类型的。
        /// 2，Table类型的。
        /// 3，实体类型的。
        /// 只有对于2，3两种类型的SelfRefKey, SelfRefText.才有意义。
        /// </summary>
        public string SelfBindKey
        {
            get
            {
                return (string)ViewState["SelfBindKey"];
            }
            set
            {
                ViewState["SelfBindKey"] = value;
            }
        }
        public string AppPath
        {
            get
            {
                return (string)ViewState["AppPath"];
            }
            set
            {
                ViewState["AppPath"] = value;
            }
        }
        /// <summary>
        /// 为attr , 设置的属性
        /// </summary>
        private Entities HisFKEns = null;
        /// <summary>
        /// 默认值
        /// </summary>
        public string SelfDefaultVal
        {
            get
            {
                return (string)ViewState["SelfDefaultVal"];
            }
            set
            {
                ViewState["SelfDefaultVal"] = value;
            }
        }
        /// <summary>
        /// 默认Text
        /// </summary>
        public string SelfDefaultText
        {
            get
            {
                return (string)ViewState["SelfDefaultText"];
            }
            set
            {
                ViewState["SelfDefaultText"] = value;
            }
        }

        /// <summary>
        /// 要不要显示 Bind 的值.
        /// </summary>
        public bool SelfIsShowVal
        {
            get
            {
                try
                {
                    return (bool)ViewState["SelfIsShowVal"];
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                ViewState["SelfIsShowVal"] = value;
            }
        }
        /// <summary>
        /// 用到了DDL 于 Ens 定义的帮定
        /// </summary>
        private void SelfBindEns()
        {
            if (this.SelfBindKey == "")
                throw new Exception("@没有设定它的Key.");

            if (this.SelfAddAllLocation == AddAllLocation.Top
                || this.SelfAddAllLocation == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("-=全部=-", "all"));

            if (this.Enabled == true)
            {
                Entities ens = this.HisFKEns;
                ens.RetrieveAll();

                this.BindEntities(ens, this.SelfEnsRefKey, this.SelfEnsRefKeyText);
            }
            else
            {
                this.Items.Add(new ListItem(this.SelfDefaultText, this.SelfDefaultVal));
            }

            if (this.SelfAddAllLocation == AddAllLocation.End || this.SelfAddAllLocation == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("-=全部=-", "all"));
        }
        /// <summary>
        /// DDLDataHelp 用到了DDL自定义的帮定。
        /// </summary>
        private void SelfBindSQL()
        {
            if (this.SelfBindKey == "")
                return;

            /*

            DDLDataHelp en = new DDLDataHelp(this.SelfBindKey);

            string sql = "SELECT TOP 100  "+en.NoCol+"  , "+en.NameCol+"  FROM  "+en.RefTable ;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            string name, val;
            foreach(DataRow dr in dt.Rows)
            {
                if (this.SelfIsShowVal)
                {
                    name=dr[0].ToString()+" "+dr[1].ToString() ; 
                    val=dr[0].ToString().Trim() ;
                }
                else
                {
                    name=dr[0].ToString().Trim()+" "+dr[1].ToString() ; 
                    val=dr[0].ToString().Trim() ;
                }
                ListItem li = new ListItem(name,val);
                this.Items.Add(li);	
            }			
            ///  判断它是不是要求双击事件。
            if (en.FK_TBDataHelp.Trim().Length==0)
                this.DealRightKey(en.FK_TBDataHelp) ; 
                */

        }
        private void DealRightKey(string TBDataHelpKey)
        {
            /*
            DataHelp en = new DataHelp(TBDataHelpKey) ; 
            string script ,  url ;
            string appPath =this.Page.Request.ApplicationPath;
			
            this.Font.Bold=false;
            this.BorderStyle=System.Web.UI.WebControls.BorderStyle.Groove;
            //this.Width=Unit.Pixel(150);
            this.ForeColor=Color.FromName("#006699");
					 
            url =appPath+"Comm/RefFunc/DataHelp.htm?"+appPath+en.FileUrl;
            script=" if ( event.button != 2)  return;  val=window.showModalDialog('"+url+"','','dialogHeight: "+en.Height+"px; dialogWidth: "+en.Width+"px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( val==undefined) return ; "+this.ClientID+".value=val ; ";
            this.Attributes["onmousedown"]=script;
            //this.ToolTip=en.ToolTip;
            */

        }


        private void TBInit(object sender, System.EventArgs e)
        {
            this.SelfBind();
            return;
            /* 可以保留如下代码。 */
            //if (this.Enabled)
            //{
            //    //url =appPath+"Comm/DataHelp.htm?"+appPath+"Comm/HelperOfTB.aspx?EnsName="+this.EnsName+"&AttrKey="+this.AttrKey ;
            //    //script=" if ( event.button != 2)  return; str="+this.ClientID+".value;str= window.showModalDialog('"+url+"&Key=\'+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; "+this.ClientID+".value=str ; ";
            //    //	this.Attributes["onmousedown"]=script;
            //    //string appPath =this.Page.Request.ApplicationPath;
            //    //string url=appPath+"Comm/DataHelp.htm?"+appPath+"Comm/HelperOfDDL.aspx?EnsName="+attr.UIBindKey+"&RefKey="+attr.UIRefKeyValue+"&RefText="+attr.UIRefKeyText;
            //    //string script=" if ( event.button != 2 )  return; str="+this.ClientID+".DataTextField; str= window.showModalDialog('"+url+"&Key=\'+str , '','dialogHeight: 500px; dialogWidth:800px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ;  "+this.ClientID+".DataTextField=str ; ";
            //    //string script=" if ( event.button != 2 )  return; window.showModalDialog('"+url+"' , '','dialogHeight: 500px; dialogWidth:800px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); SetDDLVal('"+this.ClientID+"',str ) ; ";

            //    if (this.SelfBindKey != null)
            //    {
            //        this.Attributes["onmousedown"] = " if ( event.button != 2 ) return; HalperOfDDL('" + this.AppPath + "','" + this.SelfBindKey + "','" + this.SelfEnsRefKey + "','" + this.SelfEnsRefKeyText + "','" + this.ClientID.ToString() + "' )";
            //    }
            //}
            //this.SelfBind();
        }
        #endregion

        /// <summary>
        /// new ddl
        /// </summary>
        /// <param name="attr">属性</param>
        /// <param name="DefaultVal">DefaultVal</param>
        /// <param name="DefaultText">DefaultText</param>
        /// <param name="enable">enable</param>
        public DDL(Attr attr, string DefaultVal, string DefaultText, bool enable, string appPath)
        {
            this.AppPath = appPath;
            this.ID = "DDL_" + attr.Key;
            this.BorderStyle = BorderStyle.None;
            this.CssClass = "DDL" + WebUser.Style;

            this.SelfDefaultText = DefaultText;
            this.SelfDefaultVal = DefaultVal;

            this.Enabled = enable;
            this.SelfShowType = attr.UIDDLShowType;

            this.SelfBindKey = attr.UIBindKey;
            this.SelfEnsRefKey = attr.UIRefKeyValue;
            this.SelfEnsRefKeyText = attr.UIRefKeyText;
            this.HisFKEns = attr.HisFKEns;
            this.SelfIsShowVal = false; ///不让显示编号

            //this.PreRender +=new System.EventHandler(this.DDLPreRender);
            this.Init += new System.EventHandler(this.TBInit);
        }
        public void LoadMapAttr(Attr attr)
        {
            this.ID = "DDL_" + attr.Key;
            // this.AppPath = BP.Sys.SystemConfig.AppName;
            this.BorderStyle = BorderStyle.None;

            this.SelfDefaultText = attr.UIRefKeyText;
            this.SelfDefaultVal = attr.UIRefKeyValue;

            this.Enabled = attr.UIIsReadonly;
            if (attr.MyDataType == DataType.AppInt)
                this.SelfShowType = DDLShowType.SysEnum;
            else
                this.SelfShowType = DDLShowType.Ens;

            this.HisFKEns = attr.HisFKEns;

            this.SelfBindKey = attr.UIBindKey;
            this.SelfEnsRefKey = attr.UIRefKeyValue;
            this.SelfEnsRefKeyText = attr.UIRefKeyText;
            this.SelfIsShowVal = false; ///不让显示编号
            this.Init += new System.EventHandler(this.TBInit);
        }



        public DDL(Attr attr, string DefaultVal, string DefaultText, bool enable)
        {
            this.ID = "DDL_" + attr.Key;
            this.AppPath = this.Page.Request.ApplicationPath;
            this.BorderStyle = BorderStyle.None;
            this.CssClass = "DDL" + WebUser.Style;

            this.SelfDefaultText = DefaultText;
            this.SelfDefaultVal = DefaultVal;

            this.Enabled = enable;
            this.SelfShowType = attr.UIDDLShowType;

            this.SelfBindKey = attr.UIBindKey;
            this.SelfEnsRefKey = attr.UIRefKeyValue;
            this.SelfEnsRefKeyText = attr.UIRefKeyText;
            this.HisFKEns = attr.HisFKEns;

            this.SelfIsShowVal = false; ///不让显示编号
            //this.PreRender +=new System.EventHandler(this.DDLPreRender);
            this.Init += new System.EventHandler(this.TBInit);
        }
        public void LoadMapAttr(BP.Sys.MapAttr attr)
        {
            this.ID = "DDL_" + attr.KeyOfEn;
            // this.AppPath = BP.Sys.SystemConfig.AppName;
            this.BorderStyle = BorderStyle.None;

            this.SelfDefaultText = attr.UIRefKeyText;
            this.SelfDefaultVal = attr.UIRefKey;

            this.Enabled = attr.UIIsEnable;
            if (attr.MyDataType == DataType.AppInt)
            {
                this.SelfShowType = DDLShowType.SysEnum;
                //SysEnums ses = new SysEnums(attr.UIBindKey);
                // this.BindSysEnum(attr.UIBindKey);
            }
            else
            {
                this.SelfShowType = DDLShowType.Ens;
                this.HisFKEns = BP.En.ClassFactory.GetEns(attr.UIBindKey);

                //this.HisFKEns.RetrieveAll();
                //this.BindEntities(this.HisFKEns, "No", "Name", false);
            }


            //  SelfEnsRefKey
            this.SelfBindKey = attr.UIBindKey;
            this.SelfEnsRefKey = attr.UIRefKey;
            this.SelfEnsRefKeyText = attr.UIRefKeyText;
            this.SelfIsShowVal = false; ///不让显示编号
            ///
            this.Init += new System.EventHandler(this.TBInit);
        }


        public DDL(Attr attr, string defSelectVal)
        {
            this.ID = "DDL_" + attr.Key;
            this.SelfShowType = attr.UIDDLShowType;
            this.SelfBindKey = attr.UIBindKey;
            this.SelfEnsRefKey = attr.UIRefKeyValue;
            this.SelfEnsRefKeyText = attr.UIRefKeyText;
            this.SelfDefaultVal = defSelectVal;
            this.HisFKEns = attr.HisFKEns;


            // this.SelfAddAllLocation = AddAllLocation.Top;

            if (attr.UIBindKey == "BP.Port.SJs")
            {

            }
            else
            {
                this.SelfAddAllLocation = AddAllLocation.Top;
            }
            this.SelfIsShowVal = false; ///不让显示编号
        }

        public DDL()
        {
            //this.PreRender +=new System.EventHandler(this.DDLPreRender);
            this.Init += new System.EventHandler(this.TBInit);
            //this.Load +=new System.EventHandler(this.TBPreRender);			 
        }
        public bool Bind(EntitiesNoName ens, string seleVal)
        {
            return this.BindEntitiesNoName(ens, seleVal);
        }
        public void Bind(XmlEns xmls, string key, string name)
        {
            foreach (XmlEn xml in xmls)
            {
                this.Items.Add(new ListItem(xml.GetValStringByKey(name), xml.GetValStringByKey(key)));
            }
        }
        public void Bind(Entities ens, string key, string name)
        {
            this.Items.Clear();
            foreach (Entity en in ens)
            {
                this.Items.Add(new ListItem(en.GetValStringByKey(name), en.GetValStringByKey(key)));
            }
        }

        protected override void OnPreRender(System.EventArgs e)
        {

            if (this.SelfAddAllLocation == AddAllLocation.None)
                return;

            if (this.Items.FindByValue("all") == null)
                return;

            if (this.SelfAddAllLocation == AddAllLocation.Top || this.SelfAddAllLocation == AddAllLocation.TopAndEnd)
            {
                ListItem li = new ListItem("-=全部=-", "all");
                this.Items.Add(li);
            }

            if (this.SelfAddAllLocation == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("-=全部=-", "all"));
            base.OnPreRender(e);
        }


        #region 处理用户风格

        public void Style3()
        {
            this.CssClass = "DDL3";
            /*
            //this.BorderColor=Color.Transparent;
            this.BackColor=Color.FromName("#006699");
            this.ForeColor=Color.White;		
            */
        }
        public void Style2()
        {
            this.CssClass = "DDL2";
            /*
            this.BorderColor=System.Drawing.Color.Transparent;
            this.BackColor=Color.FromName("#DEBA84");
            this.ForeColor=Color.Black;
            */
        }
        public void Style1()
        {
            this.CssClass = "DDL1";
            //this.BorderColor=System.Drawing.Color.FromName("#DEBA84");
            //this.BackColor=Color.FromName("#DEBA84");
            //this.ForeColor=Color.Black;			 			 
        }
        #endregion



        /// <summary>
        /// OID, No, Name 
        /// </summary>
        /// <param name="dt"></param>
        public void BindWithOIDNoNameCol(DataTable dt)
        {
            this.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr["No"].ToString() + " " + dr["Name"].ToString(), dr["OID"].ToString());
                this.Items.Add(li);
            }
        }
        /// <summary>
        /// OID,   Name 
        /// </summary>
        /// <param name="dt"></param>
        public void BindWithOIDNameCol(DataTable dt)
        {
            this.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr["Name"].ToString(), dr["OID"].ToString());
                this.Items.Add(li);
            }
        }
        /// <summary>
        /// OID,   Name 
        /// </summary>
        /// <param name="dt"></param>
        public void BindWithOIDNameCol(DataTable dt, string title, string selectVal)
        {
            this.Items.Clear();
            this.Items.Add(new ListItem("-=" + title + "(全部)=-", "all"));
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr["Name"].ToString(), dr["OID"].ToString());
                this.Items.Add(li);
            }
            this.Items.Add(new ListItem("-=" + title + "(全部)=-", "all"));
            foreach (ListItem li in this.Items)
            {
                if (li.Value == selectVal)
                {
                    li.Selected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// No ,Name  bind
        /// </summary>
        /// <param name="dt"></param>
        public void BindWithNoNameCol(DataTable dt, string title)
        {
            this.Items.Clear();
            this.Items.Add(new ListItem("-=" + title + "(全部)=-", "all"));
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr["Name"].ToString().Trim(), dr["No"].ToString().Trim());
                this.Items.Add(li);
            }
            this.Items.Add(new ListItem("-=" + title + "(全部)=-", "all"));
        }
        /// <summary>
        /// No ,Name bind
        /// </summary>
        /// <param name="dt"></param>
        public void BindWithNoNameCol(DataTable dt)
        {
            this.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListItem li = new ListItem(dr["Name"].ToString().Trim(), dr["No"].ToString().Trim());
                this.Items.Add(li);
            }
        }
        public void BindWithNoNameCol(DataTable dt, string title, string selectNo)
        {
            this.Items.Clear();
            BindWithNoNameCol(dt, title);
            foreach (ListItem li in this.Items)
            {
                if (li.Value.Equals(selectNo))
                {
                    li.Selected = true;
                    break;
                }
            }
        }
        public void BindMonth(int month)
        {
            this.Items.Clear();
            int i = 0;
            string str = "";
            while (i < 12)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;
                ListItem li = new ListItem(i.ToString() + "月", str);

                if (i == month)
                    li.Selected = true;

                this.Items.Add(li);
            }
        }
        public void Bindhh(int hh)
        {
            this.Items.Clear();
            int i = 7;
            string str = "";
            while (i < 20)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;
                ListItem li = new ListItem(i.ToString() + "时", str);

                if (i == hh)
                    li.Selected = true;
                this.Items.Add(li);
            }
        }
        public void BindNumFromTo(int from, int to)
        {
            this.Items.Clear();
            int i = from;
            string str = "";
            while (i <= to)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;

                ListItem li = new ListItem(i.ToString(), str);
                this.Items.Add(li);
            }
        }
        public void Bindmm()
        {
            this.Items.Clear();
            int i = 0;
            string str = "";
            while (i < 59)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;
                ListItem li = new ListItem(i.ToString() + "分", str);

                this.Items.Add(li);
            }
        }
        public void BindQuector(int selectVal)
        {
            this.Items.Clear();

            this.Items.Add(new ListItem(":00", "00"));
            this.Items.Add(new ListItem(":15", "15"));
            this.Items.Add(new ListItem(":30", "30"));
            this.Items.Add(new ListItem(":45", "45"));

            this.SetSelectItem(selectVal);
        }
        public void BindWeek(int selectVal)
        {
            this.Items.Clear();

            this.Items.Add(new ListItem("周日", "0"));
            this.Items.Add(new ListItem("周一", "1"));
            this.Items.Add(new ListItem("周二", "2"));
            this.Items.Add(new ListItem("周三", "3"));
            this.Items.Add(new ListItem("周四", "4"));
            this.Items.Add(new ListItem("周五", "5"));
            this.Items.Add(new ListItem("周六", "6"));

            this.SetSelectItem(selectVal);
        }
        public void BindDay(int day)
        {
            this.Items.Clear();
            int i = 0;
            string str = "";

            while (i < 31)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;

                ListItem li = new ListItem(i.ToString() + "日", str);
                if (i == day)
                    li.Selected = true;
                this.Items.Add(li);
            }
        }
        /// <summary>
        /// bind month ，到当前的月份。
        /// </summary>
        public void BindMonthToThisMonth()
        {
            this.Items.Clear();
            int m = DateTime.Now.Month;
            int i = 0;
            while (i < 12)
            {
                i++;
                ListItem li = new ListItem(i.ToString() + "月", i.ToString());
                if (i == m)
                {
                    this.Items.Add(li);
                    li.Selected = true;
                    break;
                }
                this.Items.Add(li);
            }

        }
        /// <summary>
        /// bind day 。
        /// </summary>
        public void BindAppDaySelectedToday()
        {
            this.Items.Clear();
            int m = DateTime.Now.Day;
            int i = 0;
            while (i < 31)
            {
                i++;
                ListItem li = new ListItem(i.ToString() + "日", i.ToString());
                if (i == m)
                {
                    this.Items.Add(li);
                    li.Selected = true;
                    continue;
                }
                this.Items.Add(li);
            }

        }
        /// <summary>
        /// 季度
        /// </summary>
        public void BindQuarter()
        {
            this.Items.Clear();
            this.Items.Add(new ListItem("第一季度", "1"));
            this.Items.Add(new ListItem("第二季度", "2"));
            this.Items.Add(new ListItem("第三季度", "3"));
            this.Items.Add(new ListItem("第四季度", "4"));

            int month = DateTime.Now.Month;
            if (month < 4)
                this.SetSelectItem("1");
            else if (month >= 4 && month < 7)
            {
                this.SetSelectItem("2");
            }
            else if (month >= 8 && month < 10)
            {
                this.SetSelectItem("3");
            }
            else
            {
                this.SetSelectItem("4");
            }
        }
        public void BindStrs(string[] strs)
        {
            int i = -1;
            foreach (string s in strs)
            {
                i++;
                ListItem li = new ListItem(s, i.ToString());
                this.Items.Add(li);
            }
        }
        public void BindAppDay()
        {
            int i = 1;
            while (i < 31)
            {
                i++;
                ListItem li = new ListItem(i.ToString() + "日", i.ToString());
                this.Items.Add(li);
            }
        }
        public void BindAppDay(string selectedDay)
        {
            int i = 1;
            int m = int.Parse(selectedDay);
            string str = "";
            while (i < 31)
            {
                i++;

                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;

                ListItem li = new ListItem(i.ToString() + "日", str);
                if (i == m)
                {
                    this.Items.Add(li);
                    continue;
                }
                this.Items.Add(li);
            }
        }

        public void BindMonthSelectCurrentMonth()
        {
            this.Items.Clear();
            int m = DateTime.Now.Month;
            int i = 0;

            string str = "";
            while (i < 12)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;

                ListItem li = new ListItem(i.ToString() + "月", str);
                if (i == m)
                {
                    li.Selected = true;
                }
                this.Items.Add(li);
            }
        }
        /// <summary>
        /// 月份  1 ， 2， 
        /// </summary>
        /// <param name="selectM"></param>
        public void BindMonth(string selectM)
        {
            this.Items.Clear();
            int i = 0;
            string str = "";

            int m = int.Parse(selectM);
            while (i < 12)
            {
                i++;
                str = i.ToString();
                if (str.Length == 1)
                    str = "0" + str;

                ListItem li = new ListItem(i.ToString() + "月", str);
                if (i == m)
                    li.Selected = true;

                this.Items.Add(li);
            }
        }

        /// <summary>
        /// bind 近三年, 默认值是当前的年
        /// </summary>
        public void BindYearMonth(int nearM)
        {
            DateTime dt = System.DateTime.Now;
            for (int i = 0; i <= nearM; i++)
            {
                this.Items.Add(new ListItem(dt.ToString("yyyy年MM月"), dt.ToString("yyyy-MM")));
                dt = dt.AddMonths(-1);
                i++;
            }
        }
        /// <summary>
        /// bind 近三年, 默认值是当前的年
        /// </summary>
        public void BindYear()
        {
            this.Items.Clear();
            int i1 = System.DateTime.Now.Year;
            int i2 = i1 - 1;
            int i3 = i1 - 2;
            int i4 = i1 - 3;
            int i5 = i1 - 4;

            this.Items.Add(new ListItem(i1.ToString() + "年", i1.ToString()));
            this.Items.Add(new ListItem(i2.ToString() + "年", i2.ToString()));
            this.Items.Add(new ListItem(i3.ToString() + "年", i3.ToString()));
            this.Items.Add(new ListItem(i4.ToString() + "年", i4.ToString()));
            this.Items.Add(new ListItem(i5.ToString() + "年", i5.ToString()));
        }

        /// <summary>
        /// bind 近三年。
        /// </summary>
        /// <param name="selectYear">选择的年度</param>
        public void BindYear(int selectYear)
        {
            this.BindYear();
            foreach (ListItem li in this.Items)
            {
                if (li.Value.Equals(selectYear.ToString()))
                {
                    li.Selected = true;
                }
            }
        }
        /// <summary>
        /// bind  近两年的。
        /// </summary>
        public void BindYearNear2()
        {
            this.Items.Clear();
        }
        public void BindAppPrecent(int selectVal)
        {
            int i = 0;
            while (true)
            {
                this.Items.Add(new ListItem(i.ToString() + "%", i.ToString()));
                i++;
                if (i == 100)
                    break;
            }
        }
        public int SelectedItemIntVal
        {
            get
            {
                return int.Parse(this.SelectedItem.Value);
            }
        }
        public string SelectedItemStringVal
        {
            get
            {
                return this.SelectedItem == null ? null : this.SelectedItem.Value;
            }
        }

        #region bind 实体。

        public void BindEntities(Entities cateEns, string cateKey, string cateText, Entities subEns, string subKey, string subText, string refKey)
        {
            this.Items.Clear();
            foreach (Entity en in cateEns)
            {
                ListItem li = new ListItem("===" + en.GetValStringByKey(cateText) + "=====", en.GetValStringByKey(cateKey));
                li.Attributes["background-color"] = "Green";
                li.Attributes["color"] = "Green";

                this.Items.Add(li);
                foreach (Entity suben in subEns)
                {
                    if (suben.GetValStringByKey(refKey) == en.GetValStringByKey(cateKey))
                    {
                        this.Items.Add(new ListItem("|-" + suben.GetValStringByKey(subText), suben.GetValStringByKey(subKey)));
                    }
                }
            }
        }

        /// <summary>
        /// 绑定生成一个有树结构的下拉菜单
        /// </summary>
        /// <param name="dtNodeSets">菜单记录数据所在的表</param>
        /// <param name="strParentColumn">表中用于标记父记录的字段</param>
        /// <param name="strRootValue">第一层记录的父记录值(通常设计为0或者-1或者Null)用来表示没有父记录</param>
        /// <param name="strIndexColumn">索引字段，也就是放在DropDownList的Value里面的字段</param>
        /// <param name="strTextColumn">显示文本字段，也就是放在DropDownList的Text里面的字段</param>
        /// <param name="drpBind">需要绑定的DropDownList</param>
        /// <param name="i">用来控制缩入量的值，请输入-1</param> 
        public static void MakeTree(DataTable dtNodeSets, string strParentColumn, string strRootValue, string strIndexColumn, string strTextColumn, DropDownList drpBind, int i)
        {
            //每向下一层，多一个缩入单位 
            i++;

            DataView dvNodeSets = new DataView(dtNodeSets);

            //edited by liuxc,2015-6-18,增加父级字段是否字符串类型判断，是字符串时，RowFilter中=需要加单引号将值包起来
            var isString = dtNodeSets.Columns[strParentColumn].DataType == typeof(String);
            dvNodeSets.RowFilter = strParentColumn + "=" + (isString ? string.Format("'{0}'", strRootValue ?? string.Empty) : strRootValue);

            string strPading = ""; //缩入字符 

            //通过i来控制缩入字符的长度，我这里设定的是一个全角的空格 
            for (int j = 0; j < i; j++)
                strPading += "　";//如果要增加缩入的长度，改成两个全角的空格就可以了 

            foreach (DataRowView drv in dvNodeSets)
            {
                //防止死循环，一旦已经加入过的项，则不允许再加入
                if (drpBind.Items.FindByValue(drv[strIndexColumn].ToString()) != null)
                    continue;

                ListItem li = new ListItem(strPading + "├" + drv[strTextColumn].ToString(), drv[strIndexColumn].ToString());
                drpBind.Items.Add(li);
                MakeTree(dtNodeSets, strParentColumn, drv[strIndexColumn].ToString(), strIndexColumn, strTextColumn, drpBind, i);
            }
            //递归结束，要回到上一层，所以缩入量减少一个单位 
            i--;
        }

        public void BindEntities(Entities ens, string refKey, string refText, bool isShowSelect=false)
        {
            this.Items.Clear();

            if (ens.Count == 0)
                ens.RetrieveAll();

            EntitiesTree treeEns = ens as EntitiesTree;
            if (treeEns != null)
            {
                DataTable dt = ens.ToDataTableField();
                MakeTree(dt, "ParentNo", "0", "No", "Name", this, -1);
                return;
            }

            EntitiesTree treeSimpEns = ens as EntitiesTree;
            if (treeSimpEns != null)
            {
                DataTable dt = ens.ToDataTableField();
                MakeTree(dt, "ParentNo", "0", "No", "Name", this, -1);
                return;
            }

            //为了避免IsBlank的判断出错增加.. 2016.12.28 by zhoupeng.
            if (isShowSelect==true)
               this.Items.Add(new ListItem("请选择", "" ));

            foreach (Entity en in ens)
            {
                this.Items.Add(new ListItem(en.GetValStringByKey(refText), en.GetValStringByKey(refKey)));
            }
        }
        public void BindEntities(EntitiesOIDName ens)
        {
            this.Items.Clear();
            foreach (EntityOIDName en in ens)
            {
                this.Items.Add(new ListItem(en.Name, en.OID.ToString()));
            }
        }
        public void BindEntities(EntitiesNoName ens, AddAllLocation where)
        {
            this.Items.Clear();
            if (where == AddAllLocation.Top || where == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("全部", "all"));

            foreach (EntityNoName en in ens)
            {
                this.Items.Add(new ListItem(en.Name, en.No));
            }

            if (where == AddAllLocation.End || where == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("全部", "all"));
        }
        public void BindEntities(EntitiesOIDName ens, AddAllLocation where)
        {
            this.Items.Clear();
            if (where == AddAllLocation.Top || where == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("全部", "all"));

            foreach (EntityOIDName en in ens)
                this.Items.Add(new ListItem(en.Name, en.OID.ToString()));

            if (where == AddAllLocation.End || where == AddAllLocation.TopAndEnd)
                this.Items.Add(new ListItem("全部", "all"));
        }
        public void BindEntities(EntitiesNoName ens)
        {
            this.Items.Clear();
            foreach (EntityNoName en in ens)
            {
                this.Items.Add(new ListItem(en.Name, en.No));
            }
        }
        public void BindEntitiesNoName(EntitiesNoName ens, bool isShowKey)
        {
            this.Items.Clear();
            foreach (EntityNoName en in ens)
            {
                ListItem li = new ListItem();
                if (isShowKey)
                {
                    li.Value = en.No;
                    li.Text = en.No + " " + en.Name;
                }
                else
                {
                    li.Value = en.No;
                    li.Text = en.Name;
                }
                this.Items.Add(li);
            }
        }
        public void BindEntitiesNoNameWithSelectAll(EntitiesNoName ens, bool IsShowKey)
        {
            this.Items.Clear();
            this.Items.Add(new ListItem("-=请选择=-", "all"));

            foreach (EntityNoName en in ens)
            {
                if (IsShowKey)
                    this.Items.Add(new ListItem(en.No + en.Name, en.No));
                else
                    this.Items.Add(new ListItem(en.Name, en.No));
            }
        }
        /// <summary>
        /// 绑定entitiesNo,Name.
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="selecVal">返回是否选中.</param>
        public bool BindEntitiesNoName(EntitiesNoName ens, string selecVal)
        {
            this.Items.Clear();
            bool isHave = false;
            foreach (EntityNoName en in ens)
            {
                if (en.No == selecVal)
                {
                    isHave = true;
                    ListItem li=  new ListItem(en.Name, en.No);
                    li.Selected = true;
                    this.Items.Add(li);
                }
                else
                {
                    this.Items.Add(new ListItem(en.Name, en.No));

                }
            }
            return isHave;
        }
        #endregion

        #region app
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ens"></param>
        public void BindEntitiesOIDName(EntitiesOIDName ens)
        {
            this.Items.Clear();
            foreach (EntityOIDName en in ens)
            {
                this.Items.Add(new ListItem(en.Name, en.OID.ToString()));
            }
        }
        public void BindEntitiesOIDName(EntitiesOIDName ens, int seleOID)
        {
            this.Items.Clear();
            this.BindEntitiesOIDName(ens);
            foreach (ListItem li in this.Items)
            {
                if (li.Value == seleOID.ToString())
                    li.Selected = true;
            }
        }

        public void BindEntitiesNoName(EntitiesNoName ens)
        {
            this.Items.Clear();
            foreach (EntityNoName en in ens)
            {
                this.Items.Add(new ListItem(en.Name, en.No));
            }
        }



        public void BindAppYesOrNo(int selectedVal)
        {
            this.Items.Clear();

            this.Items.Add(new ListItem("是", "1"));
            this.Items.Add(new ListItem("否", "0"));

            foreach (ListItem li in this.Items)
            {
                if (li.Value.Equals(selectedVal.ToString()))
                {
                    li.Selected = true;
                    break;
                }
            }
        }



        public void SetSelectItem(int val)
        {
            this.SetSelectItem(val.ToString());
        }

        public static bool SetSelectItem(DropDownList ddl, string val)
        {
            try
            {
                foreach (ListItem li in ddl.Items)
                {
                    li.Selected = false;
                }

                foreach (ListItem li in ddl.Items)
                {

                    if (li.Value == val)
                    {
                        li.Selected = true;
                        return true;
                    }
                    else
                    {
                        li.Selected = false;
                    }
                }

            }
            catch
            {

            }
            return false;
        }
        public void SetSelectItem(string val, Attr attr)
        {
            if (attr.UIBindKey == "BP.Port.Depts")
            {
                if (val.Contains(WebUser.FK_Dept) == false)
                    return;

                this.Items.Clear();
                BP.Port.Dept detps = new BP.Port.Dept(val);

                ListItem li1 = new ListItem();
                li1.Text = detps.GetValStrByKey(attr.UIRefKeyText);
                li1.Value = detps.GetValStrByKey(attr.UIRefKeyValue);
                this.Items.Add(li1);
                return;
            }

            if (attr.UIBindKey == "BP.Port.Units")
            {
                //if (val.Contains(WebUser.FK_Unit) == false)
                //    return;

                //this.Items.Clear();
                //BP.Port.Unit units = new BP.Port.Unit(val);

                //ListItem li1 = new ListItem();
                //li1.Text = units.GetValStrByKey(attr.UIRefKeyText);
                //li1.Value = units.GetValStrByKey(attr.UIRefKeyValue);
                //this.Items.Add(li1);
                return;
            }

            //this.SetSelectItem(val);
            //return;

            if (this.SetSelectItem(val))
                return;

            return;

            //                Entity en = attr.HisFKEn; // ClassFactory.GetEn(attr.UIBindKey);
            //                en.PKVal = val;
            //                en.Retrieve();


            //#warning edit: 2008-06-01  en.RetrieveFromDBSources();


            //            ListItem li = new ListItem();
            //            li.Text = en.GetValStrByKey(attr.UIRefKeyText);
            //            li.Value = en.GetValStrByKey(attr.UIRefKeyValue);

            //            if (this.Items.Contains(li))
            //            {
            //                this.SetSelectItem(val);
            //                return;
            //            }



            //            ListItem liall = this.GetItemByText("请用更多...");
            //            ListItem myall = this.GetItemByVal("all");
            //            if (myall != null)
            //            {
            //                this.Items.Clear();
            //                this.Items.Add(li);
            //                this.Items.Add(myall);

            //                // this.Items.Remove(liall);
            //            }

            //            //  this.Items.Add(li);
            //            this.SetSelectItem(val);
        }
        /// <summary>
        /// 设置选择的值
        /// </summary>
        /// <param name="val"></param>
        public bool SetSelectItem(string val)
        {
            if (val == null)
                return false;

            foreach (ListItem li in this.Items)
                li.Selected = false;

            foreach (ListItem li in this.Items)
            {
                if (li.Value == val)
                {
                    li.Selected = true;
                    return true;
                }
                else
                {
                    li.Selected = false;
                }
            }
            return false;
        }
        public ListItem GetItemByVal(string val)
        {
            foreach (ListItem li in this.Items)
            {
                if (li.Value == val)
                    return li;
            }
            return null;
        }
        public ListItem GetItemByText(string text)
        {
            foreach (ListItem li in this.Items)
            {
                if (li.Text == text)
                    return li;
            }
            return null;
        }
        public void ReomveItem(string val)
        {
            //int  i = -1 ;
            for (int ii = 0; ii < this.Items.Count; ii++)
            {
                if (this.Items[ii].Value == val)
                {
                    this.Items.RemoveAt(ii);
                    break;
                }
            }
        }
        /// <summary>
        ///违规次数
        /// </summary>
        public void BindAppWGCS(int selected)
        {
            this.Items.Clear();
            for (int i = 0; i <= 12; i++)
            {
                ListItem li = new ListItem(i.ToString(), i.ToString());
                if (i == selected)
                    li.Selected = true;
                this.Items.Add(li);
            }
        }
        /// <summary>
        /// 降序/升序
        /// </summary>
        public void BindAppDESCandASC()
        {
            this.Items.Clear();
            this.Items.Add(new ListItem("降序", "DESC"));
            this.Items.Add(new ListItem("升序", "ASC"));
        }

        #endregion

    }
}
