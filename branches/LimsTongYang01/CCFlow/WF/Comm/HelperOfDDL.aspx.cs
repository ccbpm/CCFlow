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
using BP.DA;
using BP.Web.Controls;
using BP.Sys;
using BP;

namespace CCFlow.Web.Comm.UI
{
	/// <summary>
	/// UIEn1ToM 的摘要说明。
	/// </summary>
	public partial class HelperOfDDL : System.Web.UI.Page
	{

		#region 属性.		
		public AttrOfOneVSM AttrOfOneVSM 
		{
			get
			{
				Entity en = ClassFactory.GetEn(this.EnsName) ;

				foreach(AttrOfOneVSM attr in en.EnMap.AttrsOfOneVSM)
				{
					if (attr.EnsOfMM.ToString()==this.AttrKey)
					{
						return attr;
					}
				}
				throw new Exception("错误没有找到属性． "); 
			}
		}
		/// <summary>
		/// 一的工作类
		/// </summary>
		public string EnsName
		{
			get
			{			 
				return this.Request.QueryString["EnsName"]  ; 
			}
		}
		public string AttrKey
		{
			get
			{
				return this.Request.QueryString["AttrKey"]  ; 
			}
		}
		public   string PK
		{
			get
			{
				if (ViewState["PK"]==null)
				{
					if (this.Request.QueryString["PK"]!=null)
					{
						ViewState["PK"]=this.Request.QueryString["PK"];
					}
					else
					{
						Entity mainEn=BP.En.ClassFactory.GetEn( this.EnsName ) ;
						ViewState["PK"]=this.Request.QueryString[mainEn.PK];
					}
				}
				return (string)ViewState["PK"];
			}
		}
		#endregion	

		public bool IsLine
		{
			get
			{
				try
				{
					return (bool)ViewState["IsLine"];
				}
				catch
				{
					return false;
				}
			}
			set
			{
				ViewState["IsLine"]=value;
			}
		}

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                //this.GenerLabel(this.Label1,"数据快速选择");
                Entities ens = BP.En.ClassFactory.GetEns(this.Request.QueryString["EnsName"]);
                Entity en = ens.GetNewEntity; // = BP.En.ClassFactory.GetEns(this.Request.QueryString["EnsName"] );
                Map map = en.EnMap;
                foreach (Attr attr in map.Attrs)
                {
                    /* map */
                    if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.Enum)
                        this.DropDownList1.Items.Add(new ListItem(attr.Desc, attr.Key));
                }
                this.DropDownList1.Items.Add(new ListItem("无", "None"));
            }

            try
            {

                this.SetDataV2();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("枚举操作") || ex.Message.Contains("集合已修改"))
                {
                    System.Threading.Thread.Sleep(3000);
                    this.Response.Redirect(this.Request.RawUrl);
                    return;
                }
            }
        }
		public string RefKey
		{
			get
			{
				return this.Request.QueryString["RefKey"];
			}
		}
		public string RefText
		{
			get
			{
				return this.Request.QueryString["RefText"];
			}
		}
        public void SetDataV2()
        {
            this.UCSys1.Clear();

            Entities ens = BP.En.ClassFactory.GetEns(this.Request.QueryString["EnsName"]);
            ens.RetrieveAll();

            Entity en = ens.GetNewEntity;
            string space = "";
            if (this.DropDownList1.SelectedValue == "None")
            {
                bool isGrade = ens.IsGradeEntities;
                if (isGrade)
                {
                    this.UCSys1.Add("<a name='top' ></a>");
                    int num = ens.GetCountByKey("Grade", 2);
                    if (num > 1)
                    {
                        int i = 0;
                        this.UCSys1.AddTable("width='100%'");
                        this.UCSys1.AddTR();
                        this.UCSys1.AddTDTitle("序号");
                        this.UCSys1.AddTDTitle("<img src='/WF/Comm/Images/Home.gif' border=0 />数据选择导航");
                        this.UCSys1.AddTREnd();
                        foreach (Entity myen in ens)
                        {
                            if (myen.GetValIntByKey("Grade") != 2)
                                continue;

                            i++;
                            this.UCSys1.AddTR();
                            this.UCSys1.AddTDIdx(i);
                            this.UCSys1.AddTD("<a href='#ID" + myen.GetValStringByKey(this.RefKey) + "' >&nbsp;&nbsp;" + myen.GetValStringByKey(this.RefKey) + "&nbsp;&nbsp;" + myen.GetValStringByKey(this.RefText) + "</a>");
                            this.UCSys1.AddTREnd();
                        }
                        this.UCSys1.AddTableEnd();
                    }
                }

                this.UCSys1.AddTable();
                this.UCSys1.AddTR();
                this.UCSys1.AddTDTitle("IDX");
                this.UCSys1.AddTDTitle("");
                this.UCSys1.AddTREnd();

                bool is1 = false;

                int idx = 0;
                foreach (Entity myen in ens)
                {
                    idx++;
                    is1 = this.UCSys1.AddTR(is1);
                    this.UCSys1.AddTDIdx(idx);
                    RadioBtn rb = new RadioBtn();
                    rb.GroupName = "s";
                    if (isGrade)
                    {
                        int grade = myen.GetValIntByKey("Grade");
                        space = "";
                        space = space.PadLeft(grade - 1, '-');
                        space = space.Replace("-", "&nbsp;&nbsp;&nbsp;");
                        //    this.UCSys1.AddTD(space);
                        switch (grade)
                        {
                            case 2:
                                rb.Text = "<a href='#top' name='ID" + myen.GetValStringByKey(this.RefKey) + "' ></a><b><font color=green>" + myen.GetValStringByKey(this.RefKey) + myen.GetValStringByKey(this.RefText) + "</font></b>";
                                break;
                            case 3:
                                rb.Text = "<b>" + myen.GetValStringByKey(this.RefKey) + myen.GetValStringByKey(this.RefText) + "</b>";
                                break;
                            default:
                                rb.Text = myen.GetValStringByKey(this.RefKey) + myen.GetValStringByKey(this.RefText);
                                break;
                        }
                    }
                    else
                    {
                        rb.Text = myen.GetValStringByKey(this.RefText);
                    }
                    rb.ID = "RB_" + myen.GetValStringByKey(this.RefKey);

                    string clientscript = "window.returnValue = '" + myen.GetValStringByKey(this.RefKey) + "';window.close();";
                    rb.Attributes["onclick"] = clientscript;
                    //this.UCSys1.Add(rb);
                    //this.UCSys1.AddBR();
                    this.UCSys1.AddTD(rb);
                    this.UCSys1.AddTREnd();
                }
                this.UCSys1.AddTableEnd();
                return;
            }

            string key = this.DropDownList1.SelectedValue;
            Attr attr = en.EnMap.GetAttrByKey(key);
            if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
            {
                SysEnums ses = new SysEnums(attr.Key);
                this.UCSys1.AddTable(); //("<TABLE border=1 >"); 
                foreach (SysEnum se in ses)
                {
                    this.UCSys1.Add("<TR><TD class='Toolbar'>");
                    this.UCSys1.Add(se.Lab);
                    this.UCSys1.Add("</TD></TR>");
                    this.UCSys1.Add("<TR><TD>");

                    #region add dtl
                    this.UCSys1.AddTable("width='100%'");
                    int i = -1;
                    foreach (Entity myen in ens)
                    {
                        if (myen.GetValIntByKey(attr.Key) != se.IntKey)
                            continue;

                        i++;
                        if (i == 3)
                            i = 0;
                        if (i == 0)
                            this.UCSys1.Add("<TR>");

                        RadioBtn rb = new RadioBtn();
                        rb.GroupName = "dsfsd";
                        rb.Text = myen.GetValStringByKey(this.RefText);
                        rb.ID = "RB_" + myen.GetValStringByKey(this.RefKey);

                        string clientscript = "window.returnValue = '" + myen.GetValStringByKey(this.RefKey) + "';window.close();";
                        // rb.Attributes["ondblclick"] = clientscript;
                        rb.Attributes["onclick"] = clientscript;

                        this.UCSys1.AddTD(rb);

                        if (i == 2)
                            this.UCSys1.Add("</TR>");
                    }
                    this.UCSys1.Add("</TABLE>");
                    #endregion add dtl.

                    this.UCSys1.Add("</TD></TR>");
                }
                this.UCSys1.Add("</TABLE>");
                return;
            }

            if (attr.Key == "FK_Dept")
            {
                BP.Port.Depts Depts = new BP.Port.Depts();
                Depts.RetrieveAll();

                this.UCSys1.AddTR();
                this.UCSys1.AddTDToolbar("一级分组");
                this.UCSys1.AddTREnd();

                this.UCSys1.AddTR();
                this.UCSys1.AddTDBegin();

                this.UCSys1.AddTable();
                /* 显示导航信息 */
                int i = 0;
                //int span = 2;
                foreach (BP.Port.Dept Dept in Depts)
                {
                    if (Dept.Grade == 2 || Dept.Grade == 1)
                    {
                        i++;
                        this.UCSys1.Add("<TR>");
                        this.UCSys1.AddTDIdx(i);
                        this.UCSys1.AddTD("<a href='#ID_2" + Dept.No + "' >&nbsp;&nbsp;" + Dept.No + "&nbsp;&nbsp;" + Dept.Name + "</a><BR>");
                        this.UCSys1.Add("</TR>");
                    }
                }
                this.UCSys1.AddTableEnd();
                this.UCSys1.AddTDEnd();
                this.UCSys1.AddTREnd();


                // ===================== 
                this.UCSys1.AddTR();
                this.UCSys1.AddTDToolbar("二级分组");
                this.UCSys1.AddTREnd();

                this.UCSys1.AddTDBegin();
                this.UCSys1.AddTable();
                /* 显示导航信息 */
                // int i = 0;
                //int span = 2;
                i = 0;
                foreach (BP.Port.Dept Dept in Depts)
                {
                    i++;
                    this.UCSys1.Add("<TR>");
                    this.UCSys1.AddTDIdx(i);
                    if (Dept.Grade == 2)
                        this.UCSys1.AddTD("&nbsp;&nbsp;<a name='ID_2" + Dept.No + "' >" + Dept.No + "</A>&nbsp;&nbsp;<a href='#ID" + Dept.No + "' ><b>" + Dept.Name + "</b></a><A HREF='#top'></a><BR>");
                    else
                        this.UCSys1.AddTD("&nbsp;&nbsp;" + Dept.No + "&nbsp;&nbsp;<a href='#ID" + Dept.No + "' name='#ID_2" + Dept.No + "' >" + Dept.Name + "</a><BR>");

                    this.UCSys1.Add("</TR>");
                }
                this.UCSys1.Add("</Table>");
                this.UCSys1.Add("</TD></TR>");


                //============ 数据
                foreach (BP.Port.Dept groupen in Depts)
                {
                    this.UCSys1.Add("<TR><TD class='Toolbar' >");
                    this.UCSys1.Add("<a href='#ID_2" + groupen.No + "' name='ID" + groupen.No + "' ></a>&nbsp;&nbsp;" + groupen.GetValStringByKey(attr.UIRefKeyText));
                    this.UCSys1.Add("</TD></TR>");
                    this.UCSys1.Add("<TR><TD>");

                    #region add info .
                    this.UCSys1.AddTable();
                    i = -1;
                    foreach (Entity myen in ens)
                    {
                        if (myen.GetValStringByKey(attr.Key) != groupen.GetValStringByKey(attr.UIRefKeyValue))
                            continue;

                        i++;
                        if (i == 3)
                            i = 0;

                        if (i == 0)
                            this.UCSys1.Add("<TR>");

                        RadioBtn rb = new RadioBtn();
                        rb.GroupName = "dsfsd";
                        rb.Text = myen.GetValStringByKey(this.RefText);
                        rb.ID = "RB_" + myen.GetValStringByKey(this.RefKey);

                        string clientscript = "window.returnValue = '" + myen.GetValStringByKey(this.RefKey) + "';window.close();";
                        // rb.Attributes["ondblclick"] = clientscript;
                        rb.Attributes["onclick"] = clientscript;

                        this.UCSys1.AddTD(rb);

                        if (i == 2)
                            this.UCSys1.Add("</TR>");
                    }
                    this.UCSys1.Add("</Table>");
                    #endregion add info .

                    this.UCSys1.Add("</TD></TR>");
                }
                this.UCSys1.Add("</TABLE>");
            }
            else
            {
                Entities groupens = ClassFactory.GetEns(attr.UIBindKey);
                groupens.RetrieveAll();

                this.UCSys1.AddTable(); //("<TABLE border=1 >"); 
                if (groupens.Count > 19)
                {
                    this.UCSys1.Add("<TR><TD class='Toolbar' ><img src='../Images/Home.gif' border=0 />数据选择导航&nbsp;&nbsp;&nbsp;<font size='2'>提示:点分组连接就可到达分组数据</font></TD></TR>");

                    this.UCSys1.Add("<TR><TD>");
                    this.UCSys1.AddTable();
                    /* 显示导航信息 */
                    int i = 0;
                    //int span = 2;
                    foreach (Entity groupen in groupens)
                    {
                        i++;
                        this.UCSys1.AddTR();
                        this.UCSys1.AddTDIdx(i);
                        this.UCSys1.AddTD("<a href='#ID" + groupen.GetValStringByKey(attr.UIRefKeyValue) + "' >&nbsp;&nbsp;" + groupen.GetValStringByKey(attr.UIRefKeyValue) + "&nbsp;&nbsp;" + groupen.GetValStringByKey(attr.UIRefKeyText) + "</a><BR>");
                        this.UCSys1.AddTREnd();
                    }
                    this.UCSys1.Add("</Table>");
                    this.UCSys1.Add("</TD></TR>");
                }

                foreach (Entity groupen in groupens)
                {
                    this.UCSys1.Add("<TR><TD class='Toolbar' >");
                    this.UCSys1.Add("<a href='#top' name='ID" + groupen.GetValStringByKey(attr.UIRefKeyValue) + "' ></a>&nbsp;&nbsp;" + groupen.GetValStringByKey(attr.UIRefKeyText));
                    this.UCSys1.Add("</TD></TR>");

                    this.UCSys1.Add("<TR><TD>");

                    #region add info .
                    this.UCSys1.AddTable();
                    int i = -1;
                    foreach (Entity myen in ens)
                    {
                        if (myen.GetValStringByKey(attr.Key) != groupen.GetValStringByKey(attr.UIRefKeyValue))
                            continue;

                        i++;
                        if (i == 3)
                            i = 0;

                        if (i == 0)
                            this.UCSys1.AddTR();

                        RadioBtn rb = new RadioBtn();
                        rb.GroupName = "dsfsd";
                        rb.Text = myen.GetValStringByKey(this.RefText);
                        rb.ID = "RB_" + myen.GetValStringByKey(this.RefKey);

                        string clientscript = "window.returnValue = '" + myen.GetValStringByKey(this.RefKey) + "';window.close();";
                        // rb.Attributes["ondblclick"] = clientscript;
                        rb.Attributes["onclick"] = clientscript;

                        this.UCSys1.AddTD(rb);

                        if (i == 2)
                            this.UCSys1.AddTREnd();
                    }

                    this.UCSys1.AddTableEnd();
                    #endregion add info .

                    this.UCSys1.Add("</TD></TR>");
                }
                this.UCSys1.AddTableEnd();
            }
        }
		 
		 

		#region 操作 
		public void EditMEns()
		{
			//this.WinOpen(this.Request.ApplicationPath+"/Comm/UIEns.aspx?EnsName="+this.AttrOfOneVSM.EnsOfM.ToString());
		}
		public void Save()
		{

			AttrOfOneVSM attr = this.AttrOfOneVSM ;			 
			Entities ensOfMM = attr.EnsOfMM;
			QueryObject qo = new QueryObject(ensOfMM);
			qo.AddWhere(attr.AttrOfOneInMM,this.PK);
			qo.DoQuery();
			ensOfMM.Delete();  // 删除以前保存得数据。

			AttrOfOneVSM attrOM = this.AttrOfOneVSM;
			Entities ensOfM = attrOM.EnsOfM;
			ensOfM.RetrieveAll();
			foreach(Entity en in ensOfM)
			{
				string pk = en.GetValStringByKey( attr.AttrOfMValue ); 
				CheckBox cb = (CheckBox)this.UCSys1.FindControl("CB_"+ pk );
				if (cb.Checked==false)
					continue;

				Entity en1 =ensOfMM.GetNewEntity;
				en1.SetValByKey(attr.AttrOfOneInMM,this.PK);
				en1.SetValByKey(attr.AttrOfMInMM,  pk  );
				en1.Insert();
			}

            Entity enP = BP.En.ClassFactory.GetEn(this.Request.QueryString["EnsName"]);
			if (enP.EnMap.EnType!=EnType.View)
			{
				enP.SetValByKey(enP.PK, this.PK) ;// =this.PK;
				enP.Retrieve(); //查询。
				enP.Update(); // 执行更新，处理写在 父实体 的业务逻辑。
			}
		}
		#endregion 

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


        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            Entities ens = BP.En.ClassFactory.GetEns(this.Request.QueryString["EnsName"]);
            ens.RetrieveAll();
            foreach (Entity en in ens)
            {
                RadioBtn rb = (RadioBtn)this.UCSys1.FindControl("RB_" + en.GetValStringByKey(this.RefKey));
                if (rb.Checked == false)
                    continue;

                string val=en.GetValStringByKey(this.RefKey);
                string ddl=this.Request.QueryString["DDLID"];

                if (ddl != null)
                {
                    /*     */
                    //  ddl = ddl.Replace("DDL_");
                    string mainEns=this.Request.QueryString["MainEns"];

                    BP.Sys.UserRegedit ur = new UserRegedit(BP.Web.WebUser.No, mainEns + "_SearchAttrs");
                    string cfgval = ur.Vals;
                    int idx = cfgval.IndexOf(ddl + "=");
                    string start = cfgval.Substring(0, idx);

                    string end = cfgval.Substring(idx);
                    end = end.Substring(end.IndexOf("@"));

                    ur.Vals = start + val + end;
                    ur.Update();
                }
                 

                string clientscript = "<script language='javascript'> window.returnValue = '" + val + "'; window.close(); </script>";
                this.Page.Response.Write(clientscript);
                return;
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetDataV2();
        }
}
}
