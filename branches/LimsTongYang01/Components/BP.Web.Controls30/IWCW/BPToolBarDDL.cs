using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Drawing;
using System.Data;
using BP.En;
using Microsoft.Web.UI.WebControls;
using BP.Web ; 
using BP.Port;
using BP.DA;
using BP.Sys;

namespace BP.Web.Controls
{
	 
	/// <summary>
	/// toolbarddl
	/// </summary>
	public class ToolbarDDL : Microsoft.Web.UI.WebControls.ToolbarDropDownList
	{
		public void AddItem(string text, string val)
		{
			this.Items.Add( new ListItem(text,val ) );
		}

		#region remove op
		/// <summary>
		///  
		/// </summary>
		/// <param name="val">要移出的key.</param>
		public void RemoveItemByKey(string val)
		{
			foreach(ListItem li in this.Items)
			{
				if (li.Value==val)
				{
					this.Items.Remove(li);
					return;					
				}
			}
		}
		#endregion

		/// <summary>
		/// 设置选择的
		/// </summary>
		/// <param name="index"></param>
		public void SetSelectItemByIndex(int index)
		{
			foreach(ListItem li in this.Items)
			{
				li.Selected=false;
			}
			int i=0 ;
			foreach(ListItem li in this.Items)
			{

				if (i==index)
				{
					li.Selected=true;
				}
				i++; 
			}
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
        public void SetSelectItem(string val, Attr attr)
        {
            if (attr.UIBindKey == "BP.Port.Depts")
            {
                if (val.Contains(WebUser.FK_Dept) == false)
                    return;

                this.Items.Clear();
                BP.Port.Dept zsjg = new BP.Port.Dept(val);
                ListItem li1 = new ListItem();
                li1.Text = zsjg.GetValStrByKey(attr.UIRefKeyText);
                li1.Value = zsjg.GetValStrByKey(attr.UIRefKeyValue);
                this.Items.Add(li1);
                return;
            }
            if (attr.UIBindKey == "BP.Port.Units")
            {
                //if (val.Contains(WebUser.FK_Unit) == false)
                //    return;

                //this.Items.Clear();
                //BP.Port.Unit zsjg = new BP.Port.Unit(val);
                //ListItem li1 = new ListItem();
                //li1.Text = zsjg.GetValStrByKey(attr.UIRefKeyText);
                //li1.Value = zsjg.GetValStrByKey(attr.UIRefKeyValue);
                //this.Items.Add(li1);
                return;
            }


            if (this.SetSelectItem(val))
                return;

            Entity en = attr.HisFKEn; // ClassFactory.GetEn(attr.UIBindKey);
            en.PKVal = val;

            #warning edit: 2008-06-01  en.RetrieveFromDBSources();

            en.Retrieve();

            ListItem li = new ListItem();
            li.Text = en.GetValStrByKey(attr.UIRefKeyText);
            li.Value = en.GetValStrByKey(attr.UIRefKeyValue);

            if (this.Items.Contains(li))
            {
                this.SetSelectItem(val);
                return;
            }

            ListItem liall = this.GetItemByText("请用更多...");
            ListItem myall = this.GetItemByVal("all");
            if (myall != null)
            {
                this.Items.Clear();
                this.Items.Add(li);
                this.Items.Add(myall);

                // this.Items.Remove(liall);
            }

          //  this.Items.Add(li);
            this.SetSelectItem(val);
        }
        /// <summary>
        /// 设置选择项目
        /// </summary>
        /// <param name="val"></param>
        /// <returns>是否有选择的项目</returns>
		public bool SetSelectItem(string val)
		{
            foreach (ListItem li in this.Items)
            {
                if (li.Value != val)
                    continue;

                li.Selected = true;
                return true;
            }
            return false;
		}
		/// <summary>
		/// set a item to selected.
		/// </summary>
		/// <param name="val">It's will select val</param>
		public void SetSelectItem(int val)
		{
			this.SetSelectItem(val.ToString() ) ;
		}
		/// <summary>
		/// currect selected value  turn to int . 
		/// </summary>
		public int SelectedItemIntVal
		{
			get
			{
				return int.Parse(this.SelectedItem.Value);
			}
		}
		/// <summary>
		/// currect selected string val
		/// </summary>
		public string SelectedItemStringVal
		{
			get
			{
				try
				{
					return this.SelectedItem.Value;
				}
				catch
				{
					return null;
				}
			}
		}
		/// <summary>
		/// show type
		/// </summary>
		private DDLShowType _ShowType=DDLShowType.None;
		/// <summary>
		/// show type
		/// </summary>
		public DDLShowType SelfShowType
		{
			get
			{
				return _ShowType;				
			}
			set
			{
				this._ShowType=value;
			}
		}
		/// <summary>
		/// 编号
		/// </summary>
		public string SelfNo
		{
			get
			{
				return (string)ViewState["SelfNo"];
			}
			set
			{
				ViewState["SelfNo"]=value;
			}
		}
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
				ViewState["EnsKey"]=value;
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
				ViewState["SelfEnsRefKeyText"]=value;
			}
		}

		#region 构造
		/// <summary>
		/// ddl
		/// </summary>
		/// <param name="id">id</param>
		public ToolbarDDL(string id)
		{
			this.ID = id;
			this.CssClass="DDL"+WebUser.Style;
		}
		/// <summary>
		/// toolbar ddl
		/// </summary>
		/// <param name="attr"></param>
        public ToolbarDDL(Attr attr,string defSelectVal)
        {
            this.ID = "DDL_" + attr.Key;
            this.SelfShowType = attr.UIDDLShowType;
            this.SelfBindKey = attr.UIBindKey;
            this.SelfEnsRefKey = attr.UIRefKeyValue;
            this.SelfEnsRefKeyText = attr.UIRefKeyText;
            this.SelfDefaultVal = defSelectVal;

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
		/// <summary>
		/// toolbarddl
		/// </summary>
		public ToolbarDDL()
		{
			//this.CssClass="ToolbarDDL"+WebUser.Style;
            this.Font.Size = new FontUnit(12);
		}
		/// <summary>
		/// tb int
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TBInit(object sender, System.EventArgs e)
		{
		}
		#endregion 

		#region 公共方法
		/// <summary>
		/// bind with sys enum.
		/// </summary>
		public void SelfBindSysEnum()
		{
			string sql = "SELECT  IntKey , Lab  FROM Sys_Enum WHERE EnumKey='"+this.SelfBindKey.Trim()+"' ";
			DataTable dt = DBAccess.RunSQLReturnTable(sql);
			if (dt.Rows.Count==0)
				throw new Exception("@没有预制枚举类型:"+this.SelfBindKey);
			foreach(DataRow dr in dt.Rows)
			{
				if (this.SelfIsShowVal)
					this.Items.Add(new ListItem(dr[0].ToString()+" "+dr[1].ToString(),dr[0].ToString() ));		
				else
					this.Items.Add(new ListItem( dr[1].ToString(),dr[0].ToString() ));	
			}
		}
		/// <summary>
		/// bind with a key
		/// </summary>
		/// <param name="key">key</param>
		public void SelfBind(string key)
		{
			this.SelfBindKey=key;
			this.Items.Clear();
			this.SelfBind();
		}
		/// <summary>
		/// bind.
		/// </summary>
		public void SelfBind()
		{
			if (this.SelfAddAllLocation==AddAllLocation.Top || this.SelfAddAllLocation==AddAllLocation.TopAndEnd )
				this.Items.Add(new ListItem("-=全部=-","all"));

			switch ( this.SelfShowType )
			{				 
				case DDLShowType.Boolean:
					this.Items.Add(new ListItem("是","1" )) ;
					this.Items.Add(new ListItem("否","0" )) ;					 
					break;
				case DDLShowType.Gender:
					this.Items.Add(new ListItem("男","1" )) ;
					this.Items.Add(new ListItem("女","0" )) ;
					break;
//				case DDLShowType.Year:
//					this.BindYear();
//					break;
//				case DDLShowType.Month:
//					this.BindMonth();
//					break;
//				case DDLShowType.Quarter:
//					this.BindQuarter();
//					break;
//				case DDLShowType.Day:
//					this.BindDay();
//					break;
				case DDLShowType.Self: /// 枚举类型
					SelfBindSysEnum();						
					break;
				case DDLShowType.BindTable : /// 于Table Bind .
					this.SelfBindTable();
					break;
				case DDLShowType.Ens : /// 于实体。
					this.SelfBindEns();
					break;
				 
			
			}

			if (this.SelfAddAllLocation==AddAllLocation.TopAndEnd || this.SelfAddAllLocation==AddAllLocation.End )
				this.Items.Add(new ListItem("-=全部=-","all"));

			if (this.SelfDefaultVal!=null)
				this.SetSelectItem(this.SelfDefaultVal) ;					
		}
		/// <summary>
		/// bind with month.
		/// </summary>
		public void BindMonth()
		{
			this.Items.Clear();
			int i = 0 ;
			string str="";
			while ( i < 12)
			{
				i++;
				str=i.ToString();
				if (str.Length==1)
					str="0"+str;

				ListItem li =new ListItem( i+"月", str );
				this.Items.Add(li);
			}
			this.SetSelectItem(  DateTime.Now.ToString("MM")  );
		}
		/// <summary>
		/// 
		/// </summary>
		public void BindQuarter01_04_07_10()
		{
			this.Items.Clear();
			this.Items.Add( new ListItem("第一季度","01"));
			this.Items.Add( new ListItem("第二季度","04"));
			this.Items.Add( new ListItem("第三季度","07"));
			this.Items.Add( new ListItem("第四季度","10"));

			this.SetSelectItem(DataType.GetJDByMM(DateTime.Now.ToString("MM") )) ; 
 
		}
		/// <summary>
		/// 季度
		/// </summary>
		public void BindQuarter()
		{
			this.Items.Clear();
			this.Items.Add( new ListItem("第一季度","1"));
			this.Items.Add( new ListItem("第二季度","2"));
			this.Items.Add( new ListItem("第三季度","3"));
			this.Items.Add( new ListItem("第四季度","4"));

			int month = DateTime.Now.Month ;
			if (month < 4)
				this.SetSelectItem("1");
			else if (month >=4 && month < 7)
			{
				this.SetSelectItem("2");
			}
			else if (month >=8 && month < 10)
			{
				this.SetSelectItem("3");
			}
			else
			{
				this.SetSelectItem("4");
			}			 
		}
		/// <summary>
		/// bind with days/
		/// </summary>
		public void BindDay()
		{
			this.Items.Clear();
			int i = 0 ;
			string str="";
			while ( i < 31)
			{
				i++;
				str=i.ToString();
				if (str.Length==1)
					str="0"+str;

				ListItem li =new ListItem( i.ToString()+"日", str ); 
				this.Items.Add(li);
			}
			this.SetSelectItem(DateTime.Now.ToString("dd"));
		}
		/// <summary>
		/// bind 近三年, 默认值是当前的年
		/// </summary>
		public void BindYear()
		{			 
			this.Items.Clear();
			int i = System.DateTime.Now.Year ;
			int i2 = i-1 ; 
			int i3 = i-2 ;
			this.Items.Add(new ListItem(i.ToString()+"年", i.ToString()) );
			this.Items.Add(new ListItem(i2.ToString()+"年", i2.ToString()) );
			this.Items.Add(new ListItem(i3.ToString()+"年", i3.ToString()) );
		}
		#endregion

		#region 处理BindKey
		/// <summary>
		/// -==全部==- 显示位置。
		/// </summary>		
		private AddAllLocation _SelfAddAllLocation=AddAllLocation.None ;
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
				_SelfAddAllLocation=value;
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
				ViewState["SelfBindKey"]=value;
			}
		}
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
				ViewState["SelfDefaultVal"]=value;
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
				ViewState["SelfIsShowVal"]=value;
			}
		}
		/// <summary>
		/// 用到了DDL 于 Ens 定义的帮定
		/// </summary>
		private void SelfBindEns()
		{
			if (this.SelfBindKey=="")
				throw new Exception("@没有设定它的Key.");

			Entities ens =ClassFactory.GetEns(this.SelfBindKey);
			ens.RetrieveAll();

			this.BindEntities(ens,this.SelfEnsRefKey,this.SelfEnsRefKeyText,false);
		}
		/// <summary>
		/// DDLDataHelp 用到了DDL自定义的帮定。
		/// </summary>
		private void SelfBindTable()
		{
			if (this.SelfBindKey=="")
				return ;

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
			*/
		}
		/// <summary>
		/// TBPreRender
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e"></param>
        private void TBPreRender(object sender, System.EventArgs e)
        {
            this.CssClass = "DDL" + WebUser.Style;

            // this.Attributes.Add("selcecChange", "alert('hello')");
            //  this.at
            //    htw.AddAttribute("OnCheckChange", this.jsOfOnselectMore);
            //    base.WriteItemAttributes(htw);
            // base.
            //this.
        }
		#endregion

		/// <summary>
		/// BindSysEnum
		/// </summary>
		/// <param name="key">Key</param>
		/// <param name="isShowKey">isShowKey</param>
		/// <param name="alllocal">alllocal</param>
		public void BindSysEnum(string key,bool isShowKey,AddAllLocation alllocal)
		{
			Sys.SysEnums ens = new BP.Sys.SysEnums(key);
			this.Items.Clear();
			if (alllocal==AddAllLocation.TopAndEnd || alllocal==AddAllLocation.Top)
				this.Items.Add( new ListItem(">>","all"));
			foreach(SysEnum en in ens)
			{
				if (this.SelfIsShowVal)
				{
					this.Items.Add(new ListItem(en.IntKey+" "+en.Lab, en.IntKey.ToString() ));	
				}
				else
				{
					ListItem li = new ListItem( en.Lab, en.IntKey.ToString() );
                    //li.Attributes.CssStyle.Add("style","color:"+en.Style);
                    //li.Attributes.Add("color",en.Style);
                    //li.Attributes.Add("style","color:"+en.Style);
					//li.Attributes.A.CssStyle.Add("color",en.Style);
					this.Items.Add( li );
				}
			}

			if (alllocal==AddAllLocation.TopAndEnd || alllocal==AddAllLocation.End)
				this.Items.Add( new ListItem("-=全部=-","all"));
			//this.BindEntities(ens.ToEntitiesNoName(),isShowKey,alllocal);
		}
		/// <summary>
		/// BindEntities
		/// </summary>
		/// <param name="ens">EntitiesNoName</param>
		/// <param name="isShowKey">是否显示编码</param>
		/// <param name="local">all位置</param>
		public void BindEntities(EntitiesNoName ens, bool isShowKey, AddAllLocation alllocal)
		{
            
			BindEntities(ens,"No","Name",isShowKey,true,alllocal);
		}
		/// <summary>
		/// BindEntities
		/// </summary>
		/// <param name="ens">EntitiesNoName</param>
		/// <param name="isShowKey">是否显示编码</param>
		/// <param name="local">all位置</param>
		public void BindEntities(EntitiesOID ens, bool isShowKey, AddAllLocation alllocal)
		{
			BindEntities(ens,"OID","Name",isShowKey,true,alllocal);
		}
		/// <summary>
		/// BindEntities
		/// </summary>
		/// <param name="ens">ens</param>
		/// <param name="refkey">refkey</param>
		/// <param name="reftext">reftext</param>
		/// <param name="isClearItems">is clear items.</param> 
		public void BindEntities(Entities ens, string refkey, string reftext,bool isClearItems )
		{
			BindEntities(ens,refkey,reftext,false,isClearItems,AddAllLocation.None) ; 
		}
		/// <summary>
		/// BindEntities
		/// </summary>
		/// <param name="ens">ens</param>
		/// <param name="refkey">refkey</param>
		/// <param name="reftext">reftext</param>
		/// <param name="isShowKey">is show key</param>
		/// <param name="isClearItems">is clear item</param>
		/// <param name="where">where the -=all=- location.</param>
        public void BindEntities(Entities ens, string refkey, string reftext, bool isShowKey, bool isClearItems, AddAllLocation where)
        {
            string val = SystemConfig.GetConfigXmlEns("ShowTextLen", this.SelfBindKey);
            int len = 0;
            if (val != null)
                len = int.Parse(val);

            if (isClearItems)
                this.Items.Clear();
            if (where == AddAllLocation.Top || where == AddAllLocation.TopAndEnd)
            {
                ListItem li = new ListItem("-=全部=-", "all");
                this.Items.Add(li);
               
            }

            if (SystemConfig.MaxDDLNum <= ens.Count)
            {
                ListItem  liMore = new ListItem();
                liMore.Text = "请用更多...";
                liMore.Value = "all";
                this.Items.Add(liMore);

                BP.Web.Controls.BPToolBar blb = (BP.Web.Controls.BPToolBar)this.Parent;
                blb.AddLab("<a href=\"javascript:onDDLSelectedMore('" + this.ID + "', '" + this.EnsName + "', '" + ens.ToString() + "', 'No','Name')\" >...</a>");

                return;
            }


           /// #warning 处理多选择的问题

            int maxNum = SystemConfig.MaxDDLNum ;
            string text = "";
            string key = "";
            int i = 0;
            foreach (Entity en in ens)
            {
                text = en.GetValStringByKey(reftext);
                key = en.GetValStringByKey(refkey);

                ListItem li = new ListItem();
                if (len > 0)
                {
                    if (text.Length > len)
                    {
                        li.Attributes["title"] = text;

                        text = text.Substring(0, len);
                    }
                }

                if (isShowKey)
                {
                    li.Text = key + " " + text;
                    li.Value = key;
                }
                else
                {
                    li.Text = text;
                    li.Value = key;
                }

                this.Items.Add(li);
                i++;




                if (i >= maxNum)
                {
                    li = new ListItem();
                    li.Text = "请用更多...";
                    li.Value = "all";
                    this.Items.Add(li);

                    BP.Web.Controls.BPToolBar blb = (BP.Web.Controls.BPToolBar)this.Parent;
                    blb.AddLab("<a href=\"javascript:onDDLSelectedMore('" + this.ID + "', '" + this.EnsName + "', '" + ens.ToString() + "', 'No','Name')\" >...</a>");


                    // this.AutoPostBack = true;
                    // this.SelectedIndexChanged += new EventHandler(ToolbarDDL_SelectedIndexChanged);

                    //li.Attributes["class"] = "sssssss";
                    //li.Attributes["bgColor"] = "Red";
                    //  writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "Red");
                    //  li.Attributes[
                    //ToolbarBtn btn = new ToolbarBtn();
                    //btn.Text="...";
                    // blb.AddBtn("Btn_"
                    //  blb.Attributes.Add("selcecChange", "alert('hello')");

                    //blb.Attributes.Add("onclick", "onDDLSelectedMore('" + blb.ClientID + "')");
                    // this.Attributes["onclick"] = "alert('hello')";
                    // li.Attributes["onclick"] = "alert('hello')";


                    //this.Items.Add(li);
                    //System.IO.TextWriter;
                    //System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(;
                    //this.AddAttributesToRender(HtmlTextWriterAttribute.o
                    // this.AddAttributesToRender(htw);

                    break;
                }
            }
            if (where == AddAllLocation.End || where == AddAllLocation.TopAndEnd)
            {
                ListItem li = new ListItem("-=全部=-", "all");
                this.Items.Add(li);
                //this.Items.
            }
        }

        public string EnsName
        {
            get
            {
                string val = this.ParentToolbar.Page.Request.QueryString["EnsName"];
                if (val == null)
                    val = this.ParentToolbar.Page.Request.QueryString["EnsName"];

                return val;
            }
        }

        void ToolbarDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedItemStringVal == "...")
            {
                PubClass.Alert("hello");
                return;
            }
            // throw new Exception("The method or operation is not implemented.");
        }

        //protected override void WriteItemAttributes(HtmlTextWriter htw)
        //{
        //    htw.AddAttribute("OnCheckChange", this.jsOfOnselectMore);
        //    base.WriteItemAttributes(htw);
        //}

        public string jsOfOnselectMore = "alert('hello')";
        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            if (jsOfOnselectMore == null)
            {
                base.AddAttributesToRender(writer);
                return;
            }

            //  Show the CompareValidator's error message as bold.   
            //  writer.AddStyleAttribute(System.Web.UI.HtmlTextWriterStyle.FontWeight, "bold");
            //  writer.AddAttribute(HtmlTextWriterAttribute.Onclick, jsOfOnselectMore);
            //  writer.AddAttribute(HtmlTextWriterAttribute.Selected, jsOfOnselectMore);

            writer.AddAttribute("OnCheckChange", jsOfOnselectMore);

            //   writer.AddAttribute("Selected", jsOfOnselectMore);
            //    writer.AddAttribute(HtmlTextWriterAttribute.Selected, jsOfOnselectMore);
            // writer.AddAttribute("Onclick", jsOfOnselectMore);
            //writer.AddAttribute("Onchange", jsOfOnselectMore);
            // string js = "alert('hello ')";
            //writer.AddAttribute("onmouseover", js);
            // Call the Base's AddAttributesToRender method.   

            base.AddAttributesToRender(writer);
        }
		/// <summary>
		/// 根据一个实体建立一个Entity 
		/// </summary>
		/// <param name="ens">ens</param>
		/// <param name="refkey">refkey</param>
		/// <param name="reftext">reftext</param>
		public ToolbarDDL(string id, Entities ens, string refkey, string reftext,bool isShowKey)
		{
			this.CssClass="DDL"+WebUser.Style ; 
			this.ID =id;
			this.BindEntities(ens,refkey,reftext,isShowKey) ; 			
		}
		/// <summary>
		/// 根据一个实体建立一个Entity 
		/// </summary>
		/// <param name="ens">ens</param>
		/// <param name="refkey">refkey</param>
		/// <param name="reftext">reftext</param>
		public ToolbarDDL(string id, Entities ens, string refkey, string reftext,bool isShowKey, AddAllLocation where)
		{
			this.CssClass="DDL"+WebUser.Style ; 
			this.ID = id;
			this.BindEntities(ens,refkey,reftext,isShowKey,true,where);
		}
	}
	  
}
