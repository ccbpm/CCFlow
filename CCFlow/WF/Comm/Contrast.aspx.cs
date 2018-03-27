using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.Sys.XML;
using BP;
using BP.DA;


namespace CCFlow.Web.Comm
{
	/// <summary>
	/// 功能用于对比分析：
	/// 它用于一个 纬度（比如年月） 比另外一个纬度 多多少。
	/// para : EnsName
	/// para : ContrastKey 如果是空，就会按照FK_NY 计算。
	/// para : ContrastObj 
	/// </summary>
	public partial class Contrast : BP.Web.WebPage
	{
        /// <summary>
        /// 是否只读？
        /// </summary>
        public bool IsReadonly
        {
            get
            {
                string i = this.Request.QueryString["IsReadonly"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }
		public bool IsShowSum
		{
			get
			{
				string i= this.Request.QueryString["IsShowSum"];
				if (i=="1")
					return true;
				else
					return false;

			}
		}
		public string ContrastLab
		{
			get
			{
				return ViewState["ContrastLab"].ToString();
			}
			set
			{
				ViewState["ContrastLab"]=value;
			}
		}
		public DDL DDL_Page
		{
			get
			{
				return this.ToolBar1.GetDDLByKey("DDL_Page");
			}
		}
		public Label Lab_Result
		{
			get
			{
                return new Label();
				//return  this.ToolBar1.GetLabByKey("Lab_Result");
			}
		}
		/// <summary>
		/// 分析的项目
		/// </summary>
		public string ContrastKey
		{
			get
			{
				return this.DDL_ContrastKey.SelectedItemStringVal;
			}
		}
		/// <summary>
		/// 要分析的对象
		/// </summary>
        public string ContrastObj
        {
            get
            {
                string s = this.Request.QueryString["ContrastObj"];
                if (s == null)
                    return "FK_NY";

                //throw new Exception("ContrastObj  this.Request.QueryString[ContrastObj] 您没有设定它。");
                return s;
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            UAC uac = this.HisEn.HisUAC;
            if (uac.IsView == false)
                throw new Exception("您没有查看[" + this.HisEn.EnDesc + "]数据的权限.");

            if (this.IsPostBack == false)
            {
                BP.Sys.Contrast c = new BP.Sys.Contrast();
                c.MyPK = this.EnsName + WebUser.No;
                if (c.RetrieveFromDBSources() == 0)
                    c.ContrastKey = this.Request.QueryString["ContrastKey"];
                try
                {
                    #region 设置tool bar 1 的contral

                    #region 判断权限
                    if (uac.IsView == false)
                        throw new Exception("@对不起，您没有查看的权限！");
                    #endregion

                    // 比较对象。
                    Map map = this.HisEn.EnMap;
                    string NoShowCont = "@FK_Dot@FK_Rate@";
                    //  string NoShowCont = SystemConfig.GetConfigXmlEns("NoShowContrast", this.EnsName);
                    foreach (Attr attr in map.Attrs)
                    {
                        if (attr.MyFieldType == FieldType.FK
                            || attr.MyFieldType == FieldType.PKFK)
                        {
                            if (NoShowCont.Contains(attr.Key))
                                continue;
                            this.DDL_ContrastKey.Items.Add(new ListItem(attr.Desc, attr.Key));
                        }
                    }

                    this.DDL_ContrastKey.SetSelectItem(c.ContrastKey);
                    foreach (Attr attr in map.Attrs)
                    {
                        if (attr.IsPK)
                            continue;

                        if (attr.UIContralType == UIContralType.TB == false)
                            continue;

                        if (attr.MyFieldType == FieldType.FK)
                            continue;

                        if (attr.MyFieldType == FieldType.Enum)
                            continue;

                        if (attr.MyFieldType == FieldType.PKEnum)
                            continue;

                        if (attr.Key == "OID" || attr.Key == "WorkID" || attr.Key == "MID")
                            continue;

                        switch (attr.MyDataType)
                        {
                            case DataType.AppDouble:
                            case DataType.AppFloat:
                            case DataType.AppInt:
                            case DataType.AppMoney:
                                this.DDL_GroupField.Items.Add(new ListItem(attr.Desc, attr.Key));
                                break;
                            default:
                                break;
                        }
                    }

                    this.DDL_GroupField.SetSelectItem(this.ContrastObj);


                    this.DDL_GroupWay.Items.Add(new ListItem("求和", "0"));
                    this.DDL_GroupWay.Items.Add(new ListItem("求平均", "1"));

                    this.DDL_OrderWay.Items.Add(new ListItem("降序", "0"));
                    this.DDL_OrderWay.Items.Add(new ListItem("升序", "1"));

                    if (this.Request.QueryString["OperateCol"] != null)
                    {
                        string[] strs = this.Request.QueryString["OperateCol"].Split('_');
                        this.DDL_GroupField.SetSelectItem(strs[0]);
                        this.DDL_GroupWay.SetSelectItem(strs[1]);
                    }
                    #endregion

                    this.ToolBar1.InitByMapV2(this.HisEn.EnMap,1);
                    this.BindContrastKey(map);
                    InitState(c);
                    this.SetDGData();
                }
                catch (Exception ex)
                {
                    this.HisEns.DoDBCheck(DBCheckLevel.High);
                    throw new Exception("@装载出现错误:" + ex.Message);
                }
            }

            //this.ToolBar1.ButtonClick += new System.EventHandler(this.ToolBar1_ButtonClick);
            string lab = SystemConfig.GetConfigXmlEns("Contrast", this.EnsName);
            if (lab == null)
                lab = this.HisEn.EnMap.EnDesc;

            this.Label2.Text = this.ContrastLab + "1";
            this.Label3.Text = this.ContrastLab + "2";

            this.Label1.Text = this.GenerCaption(lab);
            //  this.Label1.Controls.Add(this.GenerLabel("<img src='../Img/Btn/DataGroup.gif' border=0  />" + lab));
            this.DDL_ContrastKey.AutoPostBack = true;
            this.DDL_ContrastKey.SelectedIndexChanged += new EventHandler(DDL_ContrastKey_SelectedIndexChanged);
            this.BindDDLMore();
        }
        public void InitState(BP.Sys.Contrast c)
        {
            if (c.KeyVal1 == "" || c.KeyVal1 == null)
                return;

            this.DDL_ContrastKey.SetSelectItem(c.ContrastKey);
            this.DDL_M1.SetSelectItem(c.KeyVal1);
            this.DDL_M2.SetSelectItem(c.KeyVal2);
            this.DDL_Key.SetSelectItem(c.SortBy);
            this.DDL_GroupField.SetSelectItem(c.KeyOfNum);
            this.DDL_GroupWay.SetSelectItem(c.GroupWay);
            this.DDL_OrderWay.SetSelectItem(c.OrderWay);
        }
        public void BindDDLMore()
        {
            Attr attr = this.HisEn.EnMap.GetAttrByKey(this.ContrastKey);
            string srip = "";
            string path = this.Request.ApplicationPath;
            this.UCBtn1.Clear();
            this.UCBtn2.Clear();

            if (attr.IsEnum )
                return;

            srip = "javascript:HalperOfDDL('" + path + "/','" + attr.UIBindKey + "','" + attr.UIRefKeyValue + "','" + attr.UIRefKeyText + "','" + this.DDL_M1.ClientID.ToString() + "');";
            this.UCBtn1.Add("<input class=Btn type='button' value='...' onclick=\"" + srip + "\"  name='b" + this.DDL_M1.ID + "'  ></td>");

            srip = "javascript:HalperOfDDL('" + path + "/','" + attr.UIBindKey + "','" + attr.UIRefKeyValue + "','" + attr.UIRefKeyText + "','" + this.DDL_M2.ClientID.ToString() + "');";
            this.UCBtn2.Add("<input class=Btn type='button' value='...' onclick=\"" + srip + "\"  name='b" + this.DDL_M2.ID + "'  ></td>");

        }
		public void BindContrastKey(Map map)
		{
            switch (this.ContrastKey)
            {
                case "FK_NY":
                    BP.Pub.NYs nys = new BP.Pub.NYs();
                    nys.RetrieveAll();
                    this.DDL_M1.BindEntities(nys);
                    DateTime dt = DateTime.Now.AddMonths(-1);
                    this.DDL_M1.SetSelectItem(dt.ToString("yyyy-MM"));
                    this.DDL_M2.BindEntities(nys);
                    this.DDL_M2.SetSelectItem(DateTime.Now.ToString("yyyy-MM"));
                    this.ContrastLab = "月份";
                    break;
                default:
                    Attr attr = map.GetAttrByKey(this.ContrastKey);
                    this.ContrastLab = attr.Desc;
                    Entities ens = attr.HisFKEns; //DA.ClassFactory.GetEns(attr.UIBindKey);
                    ens.RetrieveAll();

                    this.DDL_M1.BindEntities(ens, attr.UIRefKeyValue, attr.UIRefKeyText, false);
                    this.DDL_M2.BindEntities(ens, attr.UIRefKeyValue, attr.UIRefKeyText, false);

                    this.BindDDLMore();
                  

                    this.DDL_M2.SetSelectItem(attr.DefaultVal.ToString());

                    this.Label2.Text = attr.Desc + "1";
                    this.Label3.Text = attr.Desc + "2";

                    break;
            }
			this.DDL_Key.Items.Clear();

			string reAttrs=this.Request.QueryString["Attrs"];
			foreach(Attr attr in map.Attrs)
			{
				if (attr.MyFieldType==FieldType.PKEnum )
					continue;

				if (attr.MyFieldType==FieldType.Enum )
					continue;

				if (attr.Key==this.ContrastKey )
					continue;

				if (attr.UIContralType==UIContralType.DDL )
				{
					ListItem li = new ListItem(attr.Desc,attr.Key);
					if (reAttrs!=null)
					{
						if ( reAttrs.IndexOf(attr.Key)!=-1)
							li.Selected=true;
					}
					this.DDL_Key.Items.Add( li );
				}
			}

			if (this.DDL_Key.Items.Count==0)
				throw new Exception (map.EnDesc+"没有外键条件，不适合做分组查询。");

		}


		#region 方法 
		
        public void SetDGData()
        {
            Map map = this.HisEn.EnMap;

            #region 检查布局 条件 是否合理。
            try
            {
                // 判断选择的两个对象是否相同。
                if (this.DDL_M2.SelectedItemStringVal == this.DDL_M1.SelectedItemStringVal)
                    throw new Exception("对比分析的两个对象[" + this.ContrastLab + "1]与[" + this.ContrastLab + "2]不能相同，这样不符合业务逻辑。");

                // 取出来隶属属性相同的全局配置。
                GlobalKeyVals xmls = new GlobalKeyVals();
                xmls.RetrieveBy(GlobalKeyValAttr.Key, GlobalKeyValList.Subjection);
                foreach (GlobalKeyVal xml in xmls)
                {
                    if (xml.Val.Contains("@" + this.ContrastKey + "@") == false)
                        continue;

                    /*
                       如果当前的比较对象，包含在全局的系列配置中，
                       就把查询条件中的所有与此有关系的都要设置为全部。
                     */
                    string[] myattrs = xml.Val.Split('@');
                    foreach (string s in myattrs)
                    {
                        if (s == null || s == "")
                            continue;

                        // 把隶属与条件都设置为查询全部。
                        if (this.ToolBar1.FindControl("DDL_" + s) !=null)
                        {
                            if (s == "FK_Dept")
                                this.ToolBar1.GetDDLByKey("DDL_FK_Dept").SelectedIndex = 0;
                            else
                                this.ToolBar1.GetDDLByKey("DDL_" + s).SetSelectItem("all");
                        }
                    }

                    if (xml.Val.Contains("@" + this.DDL_Key.SelectedItemStringVal + "@"))
                    {
                        /* 分析项目与分组条件不能同选择。*/
                        throw new Exception("您不能选择[" + map.GetAttrByKey(this.DDL_Key.SelectedItemStringVal).Desc + "]做为分类条件。这样不符合业务逻辑，因为它与[" + map.GetAttrByKey(this.ContrastKey).Desc + "]之间有隶属关系。");
                    }
                }

                if (this.ToolBar1.IsExitsContral("DDL_" + this.ContrastKey))
                {
                    /*如果在查询条件里面有，就自动设置它为全部的查询条件，因为比较对象的明细已经确定了。*/
                    if (this.ContrastKey == "FK_Dept")
                    {
                        /* 设置为最大的查询条件 */
                        this.ToolBar1.GetDDLByKey("DDL_FK_Dept").SelectedIndex = 0;
                    }
                    else
                    {
                        this.ToolBar1.GetDDLByKey("DDL_" + this.ContrastKey).SetSelectItem("all");
                    }
                }

                //if (this.ToolBar1.IsExitsContral("DDL_" + this.DDL_GroupField.SelectedItemStringVal))
                //{
                //    /*如果在查询条件里面有。*/
                //    this.ToolBar1.GetDDLByKey("DDL_" + this.DDL_GroupField.SelectedItemStringVal).SetSelectItem("all");
                //}
            }
            catch (Exception ex)
            {
                this.UCSys1.AddMsgOfWarning("在检查对比条件时间出现如下问题：", ex.Message);
                return;
            }
            #endregion

            this.Label2.Text = this.ContrastLab + "1";
            this.Label3.Text = this.ContrastLab + "2";

            try
            {
                Attrs attrs = new Attrs();
                attrs.Add(map.GetAttrByKey(this.DDL_Key.SelectedItemStringVal));

                Entities ens = this.HisEns;
              //  QueryObject qo = this.ToolBar1.InitTableByEnsV2(ens, ens.GetNewEntity, 10000, 1);
               
                QueryObject qo =new QueryObject();
                //= this.ToolBar1.InitQueryObjectByEns(ens,
                  //  map.IsShowSearchKey,map.DTSearchWay, map.DTSearchKey, map.Attrs, map.AttrsOfSearch, map.AttrsOfSearch);

                Attr attr = new Attr();
                attr = map.GetAttrByKey(this.DDL_GroupField.SelectedItemStringVal);
                qo.addAnd();
                if (this.ContrastKey == "FK_Dept")
                    qo.AddWhereDept(this.DDL_M1.SelectedItemStringVal);
                else
                    qo.AddWhere(this.ContrastKey, this.DDL_M1.SelectedItemStringVal);

                //第1个时间段
                DataTable dt1 = qo.DoGroupReturnTable(this.HisEn, attrs, attr, (GroupWay)this.DDL_GroupWay.SelectedItemIntVal, (OrderWay)this.DDL_OrderWay.SelectedItemIntVal);

                //第2个时间段
                Attrs attrs2 = new Attrs();
                attrs2.Add(map.GetAttrByKey(this.DDL_Key.SelectedItemStringVal));

                Entities ens2 = this.HisEns;
                //QueryObject qo2 = this.ToolBar1.InitTableByEnsV2(ens2, ens2.GetNewEntity, 10000, 1);
                QueryObject qo2 = new QueryObject();

                //qo2.addAnd();

                if (this.ContrastKey == "FK_Dept")
                    qo2.AddWhereDept(this.DDL_M2.SelectedItemStringVal);
                else
                    qo2.AddWhere(this.ContrastKey, this.DDL_M2.SelectedItemStringVal);

                DataTable dt2 = qo2.DoGroupReturnTable(this.HisEn, attrs2, attr, (GroupWay)this.DDL_GroupWay.SelectedItemIntVal, (OrderWay)this.DDL_OrderWay.SelectedItemIntVal);


                #region 生成要url条件
                string url = "ContrastDtl.aspx?EnsName=" + this.EnsName;
                //Map map = ens.GetNewEntity;
                foreach (Attr attrS in map.SearchAttrs)
                {
                    if (attrS.MyFieldType == FieldType.RefText)
                        continue;

                    if (this.ContrastKey == attrS.Key)
                        continue;

                    string s = this.ToolBar1.GetDDLByKey("DDL_" + attrS.Key).SelectedItemStringVal;
                    if (s == "all")
                        continue;

                    //ToolbarDDL ddl = (ToolbarDDL)ctl;
                    url += "&" + attrS.Key + "=" + s;
                }
                #endregion


                this.SaveState();
                this.Bind(dt1, dt2, url);
            }
            catch (Exception ex)
            {
                this.ResponseWriteRedMsg(ex);
            }
        }
        /// <summary>
        /// 保存当前的状态
        /// </summary>
        public void SaveState()
        {
            BP.Sys.Contrast c = new BP.Sys.Contrast();
            c.MyPK = this.EnsName + BP.Web.WebUser.No;

            c.ContrastKey = this.DDL_ContrastKey.SelectedItemStringVal;
            c.KeyVal1 = this.DDL_M1.SelectedItemStringVal;
            c.KeyVal2 = this.DDL_M2.SelectedItemStringVal;

            c.SortBy = this.DDL_Key.SelectedItemStringVal;
            c.KeyOfNum = this.DDL_GroupField.SelectedItemStringVal;


            c.GroupWay = this.DDL_GroupWay.SelectedItemIntVal;
            c.OrderWay = this.DDL_OrderWay.SelectedItemStringVal;

            int i=c.Update();
            if (i == 0)
                c.Insert();
        }
		public void Bind(DataTable dt1, DataTable dt2, string url )
		{
			string key=this.DDL_Key.SelectedItemStringVal;
			Attr attr= this.HisEn.EnMap.GetAttrByKey(key);
			Entities ensOfGroup = BP.En.ClassFactory.GetEns(attr.UIBindKey);
			ensOfGroup.RetrieveAll();

			string str="";
			str+="<Table   style='border-collapse: collapse' bordercolor='#111111'  >";
			str+="<TR  >";
			str+="  <TD warp=false class='Title' nowrap >序</TD>";
			str+="  <TD warp=false class='Title' nowrap >"+this.DDL_Key.SelectedItem.Text+"</TD>";
			str+="  <TD warp=false class='Title' nowrap >"+this.DDL_M1.SelectedItem.Text+"</TD>";
			str+="  <TD warp=false class='Title' nowrap >"+this.DDL_M2.SelectedItem.Text+"</TD>";
			str+="  <TD warp=false class='Title' nowrap >降低值</TD>";
			str+="  <TD warp=false class='Title' nowrap >降低比例%</TD>";
			str+="</TR>";

			int idx=0;
			foreach(Entity en in ensOfGroup)
			{
				bool isHave=false;
				foreach(DataRow dr in dt1.Rows)
				{
					string kv=dr[0].ToString();
					if (en.GetValStringByKey(attr.UIRefKeyValue)==kv)
					{
						isHave=true;
						break;
					}
				}
				if (isHave==false)
					continue;  // 不存在这个值 就continue;

				idx++;
				str+="<TR onmouseover='TROver(this)' onmouseout='TROut(this)' >";
				str+="  <TD warp=false class='Idx' nowrap >"+idx.ToString() +"</TD>";
				str+="  <TD warp=false class='TD' nowrap >"+en.GetValStringByKey(attr.UIRefKeyText) +"</TD>";
				int num1=0;
				int num2=0;
				foreach(DataRow dr1 in dt1.Rows)
				{
					string kv=dr1[0].ToString();  //循环到值 。
					if (en.GetValStringByKey(attr.UIRefKeyValue)!=kv)
						continue;

					try
					{
						num1= int.Parse( dr1[2].ToString() ); 
					}
					catch
					{

						num1= int.Parse(  decimal.Parse( dr1[2].ToString() ).ToString("0.00") ) ; 

					}
					num2= 0 ; 
					str+="  <TD warp=false class='TDNum' nowrap ><a href=\"javascript:WinOpen('"+url+"&"+this.DDL_Key.SelectedItemStringVal+"="+dr1[0].ToString()+"&"+this.ContrastKey+"="+this.DDL_M1.SelectedItemStringVal+"')\"  >"+dr1[2].ToString()+"</a></TD>"; // 时间段1值.
					isHave=false;
					foreach(DataRow dr2 in dt2.Rows)
					{
						num2=int.Parse(dr2[2].ToString()) ;
						if ( dr2[0].ToString() ==en.GetValStringByKey(attr.UIRefKeyValue) )
						{
							isHave=true;
							int cz=num1-num2;
							str+="  <TD warp=false class='TDNum' nowrap ><a href=\"javascript:WinOpen('"+url+"&"+this.DDL_Key.SelectedItemStringVal+"="+dr2[0].ToString()+"&"+this.ContrastKey+"="+this.DDL_M2.SelectedItemStringVal+"')\"  >"+num2.ToString()+"</a></TD>"; // 时间段1值.
							str+="  <TD warp=false class='TDNum' nowrap >"+cz+"</TD>"; // 降低数.
							break;
						}
					}
					if (isHave==false)
					{
						num2=0;
						str+="  <TD warp=false class='TDNum' nowrap >0</TD>"; // 时间段1值.
						str+="  <TD warp=false class='TDNum' nowrap >"+num1+"</TD>"; // 降低数.
					}
				}
				if (num1==0)
					str+="  <TD warp=false class='TDNum' nowrap >0.00</TD>";
				else
				{
					decimal fz= decimal.Parse(num1.ToString())  - decimal.Parse(num2.ToString()) ; 
					decimal fm= decimal.Parse(num1.ToString());
					decimal rate =  fz/fm *100 ;
					str+="  <TD warp=false class='TDNum' nowrap >"+rate.ToString("0.00") +"</TD>";
				}
				str+="</TR>";
			}
			str+="</Table>";
			this.UCSys1.Add(str);
		}

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);			
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{   

		}
		#endregion
	 
		
		#endregion
        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            Btn btn = (Btn)sender;
            switch (btn.ID)
            {
                case NamesOfBtn.Save:
                    if (btn.ID == "Btn_Save")
                    {
                        GroupEnsTemplates rts = new GroupEnsTemplates();
                        GroupEnsTemplate rt = new GroupEnsTemplate();
                        rt.EnsName = this.EnsName;
                        //rt.Name=""
                        string name = "";
                        //	string opercol="";
                        string attrs = "";
                        foreach (ListItem li in DDL_Key.Items)
                        {
                            if (li.Selected)
                            {
                                attrs += "@" + li.Value;
                                name += li.Text + "_";
                            }
                        }
                        name = this.HisEn.EnDesc + name.Substring(0, name.Length - 1);
                        if (rt.Search(WebUser.No, this.EnsName, attrs) >= 1)
                        {
                            this.InvokeEnManager(rts.ToString(), rt.OID.ToString(), true);
                            return;
                        }
                        rt.Name = name;
                        rt.Attrs = attrs;
                        rt.OperateCol = this.DDL_GroupField.SelectedItemStringVal + "@" + this.DDL_GroupWay.SelectedItemStringVal;
                        rt.Rec = WebUser.No;
                        rt.EnName = this.EnsName;
                        rt.EnName = this.HisEn.EnMap.EnDesc;
                        rt.Save();
                        this.InvokeEnManager(rts.ToString(), rt.OID.ToString(), true);
                        //	this.ResponseWriteBlueMsg("当前的模板已经加入了自定义报表队列，点击这里<a href');\"编辑自定义报表</a>");
                    }
                    break;
                case NamesOfBtn.Help:
                    this.Helper();
                    break;
                default:
                    break;
            }
            this.SetDGData();
        }

        private void DDL_Key_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            return;
            //this.SetDGData();
        }
        private void DDL_ContrastKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindContrastKey(this.HisEn.EnMap);
            this.UCSys1.Clear();
            this.UCSys1.AddMsgOfInfo(  "操作提示：", "&nbsp;&nbsp;&nbsp;&nbsp;请您设置好查询比较条件，之后点工具栏上的查找按钮。");
        }
	}
}
