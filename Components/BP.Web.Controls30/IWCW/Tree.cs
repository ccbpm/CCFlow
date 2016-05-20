using System;
using System.Data;
using Microsoft.Web.UI.WebControls;
using System.Web.UI.WebControls;
using BP.En;
using System.ComponentModel;
using BP.DA;
using Microsoft.Web.UI.WebControls.Design;

namespace BP.Web.Controls
{
	/// <summary>
	/// BPGrid 的摘要说明。
	/// </summary>
    public class Tree : Microsoft.Web.UI.WebControls.TreeView
    {
        /// <summary>
        /// 把选择的节点，都设置为sql的模式。
        /// </summary>
        /// <returns>in sql</returns>
        public string ToStringOfSQLModel()
        {
            string pk = "";
            foreach (Node nd in this.Nodes)
            {
                if (nd.Checked == false)
                    continue;

                pk += "'" + nd.SelfNo + "',";
            }
            if (pk == "")
                return "''";

            pk = pk.Substring(0, pk.Length - 1);
            return pk;
        }
        /// <summary>
        /// 设置全部选择
        /// </summary>
        /// <param name="isChecked">isChecked</param>
        public void SetChecked(bool isChecked)
        {
            foreach (Microsoft.Web.UI.WebControls.TreeNode nd in this.Nodes)
            {
                SetChecked(isChecked, nd);
            }
        }
        public void SetChecked(bool isChecked, Microsoft.Web.UI.WebControls.TreeNode mytn)
        {
            foreach (Microsoft.Web.UI.WebControls.TreeNode tn in mytn.Nodes)
            {
                tn.Checked = isChecked;
                SetChecked(isChecked, tn);
            }
        }

        public void SetSelectedNode(string selectedNo)
        {

        }

        #region 与实体Bind
        /// <summary>
        /// EntitiesNoName
        /// </summary>
        /// <param name="ens"></param>
        public void BindEntities(Entities ens, bool isCheckBox, bool isShowKey, string refVal, string refText)
        {
            this.Nodes.Clear();
            foreach (Entity en in ens)
            {
                Node nd1 = new Node(en.GetValStringByKey(refVal), en.GetValStringByKey(refText));
                nd1.CheckBox = isCheckBox;
                if (isShowKey == false)
                {
                    nd1.Text = en.GetValStringByKey(refText);
                }

                this.Nodes.Add(nd1);
                continue;
            }
        }
        /// <summary>
        /// EntitiesNoName
        /// </summary>
        /// <param name="ens"></param>
        public void BindEntities(EntitiesNoName ens, bool isCheckBox, bool isShowKey)
        {
            this.Nodes.Clear();
            foreach (EntityNoName en in ens)
            {
                Node nd1 = new Node(en.No, en.Name);
                nd1.CheckBox = isCheckBox;
                if (isShowKey == false)
                {
                    nd1.Text = en.Name;
                }
                this.Nodes.Add(nd1);
                continue;
            }
        }
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="ens"></param>
        public void BindDeptsType(EntitiesNoName ens)
        {
            this.Nodes.Clear();
            this.BindDeptsType(ens, 2);
            this.BindDeptsType(ens, 4);
            this.BindDeptsType(ens, 6);
            this.BindDeptsType(ens, 8);
            this.BindDeptsType(ens, 10);
        }
        private void BindDeptsType(EntitiesNoName ens, int len)
        {
            foreach (EntityNoName en in ens)
            {
                if (en.No.Length != len)
                    continue;


                Microsoft.Web.UI.WebControls.TreeNode tn = new Microsoft.Web.UI.WebControls.TreeNode();
                tn.Text = en.Name;
                tn.ID = "TN" + en.No;
                if (len == 2)
                    this.Nodes.Add(tn);


                Microsoft.Web.UI.WebControls.TreeNode ptn = this.GetNodeByID("TN" + en.No.Substring(0, en.No.Length - 2));
                if (ptn != null)
                {
                    ptn.Nodes.Add(tn);
                }
                else
                    this.Nodes.Add(tn);
            }
        }
        #endregion

        #region 构造方法
        public Tree()
        {
            //this.InitTree();
            //this.CssClass="Tree"+WebUser.Style;		

            //this.ExpandedImageUrl=altopen.gif
            //this.ExpandedImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath + "images/System/altopen.gif";
            //this.SystemImagesPath=System.Web.HttpContext.Current.Request.ApplicationPath+"/images/TreeImages/";
            //this.ImageUrl
            //this.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath + "images/System/altclose.gif";
            //this.SelectedImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath + "images/System/06.gif";
            //this.ExpandedImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath + "images/sys/openFload.ico" ;
            //this.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath + "images/sys/arror.ico";

        }
        private void InitTree()
        {
            //this.SystemImagesPath=System.Web.HttpContext.Current.Request.ApplicationPath+"/images/TreeImages/";
            this.CssClass = "Tree" + WebUser.Style;
        }
        #endregion

        #region 基本操作。
        /// <summary>
        /// 得到选择的 SelfNo 根据分割符号。
        /// </summary>
        /// <param name="spt">分割符号</param>
        /// <returns>组成的字符</returns>
        public string GetSelfNosBySpt(string spt)
        {
            string str = "";
            foreach (Node nd in this.Nodes)
            {
                str += spt + nd.SelfNo;
            }
            return str;
        }
        public Microsoft.Web.UI.WebControls.TreeNode GetNodeByID(string id)
        {
            foreach (Microsoft.Web.UI.WebControls.TreeNode tn in this.Nodes)
            {
                if (tn.ID == id)
                    return tn;

                Microsoft.Web.UI.WebControls.TreeNode mytn = this.GetNodeByID(id, tn);
                if (mytn != null)
                    return mytn;
            }
            return null;
        }

        public Microsoft.Web.UI.WebControls.TreeNode GetNodeByID(string id, Microsoft.Web.UI.WebControls.TreeNode tn)
        {
            foreach (Microsoft.Web.UI.WebControls.TreeNode mytn in tn.Nodes)
            {
                if (mytn.ID == id)
                    return mytn;
                Microsoft.Web.UI.WebControls.TreeNode tnXXX = GetNodeByID(id, mytn);
                if (tnXXX == null)
                    continue;
                else
                    return tnXXX;
            }
            return null;
        }


        /// <summary>
        /// 得到当前选择的树结点
        /// </summary>
        /// <returns></returns>
        public Node GetCurrSelectedNode
        {
            get
            {
                string index = this.SelectedNodeIndex;
                try
                {
                    return (Node)this.GetNodeFromIndex(index);
                }
                catch
                {
                    return null;
                }
            }
        }
        public int GetCurrSelectedNodeLevel
        {
            get
            {
                if (this.GetCurrSelectedNode.Nodes.Count > 0)
                    return 1;
                else
                    return 2;
            }
        }
        #endregion

        #region Bind 方法。
        /// <summary>
        ///  DT col OID, No, Name .
        ///  SeleDt col OID .
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="SeleDt"></param>
        public void BindSelected(DataTable SeleDt)
        {
            foreach (DataRow dr in SeleDt.Rows)
            {
                int OID = (int)dr["OID"];
                foreach (Node nd in this.Nodes)
                {
                    if (nd.SelfOID == OID)
                    {
                        nd.Checked = true;
                    }
                }
            }
        }
        /// <summary>
        /// OID, No , Name
        /// </summary>
        /// <param name="dt"></param>
        public void BindByTable(DataTable dt)
        {
            this.BindByTable(dt, false);
        }
        /// <summary>
        /// OID, No , Name
        /// </summary>
        /// <param name="dt"></param>
        public void BindByTableNoName(DataTable dt, bool IsCheckBox)
        {
            this.Nodes.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                Node nd = new Node();
                nd.SelfNo = dr[0].ToString();
                nd.SelfName = dr[1].ToString();
                nd.Text = nd.SelfNo + nd.SelfName;
                nd.CheckBox = IsCheckBox;
                this.Nodes.Add(nd);
            }
        }
        /// <summary>
        /// OID, No , Name . 是不是 CheckBox
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="IsCheck"></param>
        public void BindByTable(DataTable dt, bool IsCheckBox)
        {
            this.Nodes.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                Node nd = new Node(dr);
                nd.CheckBox = IsCheckBox;
                this.Nodes.Add(nd);
            }
        }
        #endregion

        //		/ <summary>
        //		/ 用户功能, 修改于 2003-10-21
        //		/ </summary> 
        //		public void BindAppFunc()
        //		{
        //			FuncCates HisFuncCates = BP.Web.WebUser.HisFuncCates;
        //			Funcs HisFuncs = BP.Web.WebUser.HisFuncs;
        //			Emp en = new Emp(WebUser.No);
        //			foreach(FuncCate fc in HisFuncCates)
        //			{				
        //				Node nd = new Node(fc.No,fc.Name);
        //				nd.Expanded=true;
        //				if (en.IsShowICO)				
        //					nd.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+fc.Ico;				 
        //				
        //				this.Nodes.Add(nd);
        //				Funcs  funcs = WebUser.GetHisFuncsByCateNo(fc.No);
        //				foreach(Func f in funcs)
        //				{	 		
        //					if (f.IsShow==false) continue;				 
        //					Node n = new Node(f.No,f.Name);
        //					n.NavigateUrl=f.Url;
        //					if (en.IsShowICO)
        //						n.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+f.Ico;
        //					n.Target="mainfrm";
        //					nd.Nodes.Add( n );
        //				}	
        //			}
        //		}
        //		/// <summary>
        //		/// 用户功能
        //		/// </summary> 
        //		public void BindAppUserFunc(string userNo)
        //		{
        //			FuncCates HisFuncCates = new FuncCates();
        //			Funcs HisFuncs = BP.Web.WebUser.HisFuncs;
        //			Emp en = new Emp(userNo );
        //			foreach(FuncCate fc in HisFuncCates)
        //			{
        //				Node nd = new Node(fc.No,fc.Name);
        //				nd.Expanded=true;
        //				if (en.IsShowICO)
        //				{
        //					nd.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+fc.Ico;
        //					//nd.ExpandedImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+"/images/sys/OPENFOLD.ICO";
        //				}
        //				this.Nodes.Add(nd);
        //
        //				Funcs  funcs = WebUser.GetHisFuncsByCateNo(fc.No);
        //				foreach(Func f in funcs)
        //				{	
        //					Node n = new Node(f.No,f.Name) ;
        //					n.NavigateUrl=f.Url;
        //					if (en.IsShowICO)
        //						n.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+f.Ico;
        //					n.Target="mainfrm";
        //					nd.Nodes.Add( n );
        //				}	
        //			}
        //		}
        //		/// <summary>
        //		/// bind 纳税人的功能
        //		/// </summary>
        //		public void BindAppAllNSRFunc()
        //		{
        //			NSRFuncSorts ens = new  NSRFuncSorts();
        //			ens.QueryAll();
        //			foreach(NSRFuncSort fc in ens)
        //			{
        //				Node nd = new Node( fc.No,fc.Name);
        //				nd.Expanded=true;
        //				this.Nodes.Add(nd);
        //				NSRFuncs  funcs =fc.HisFuncs;
        //				foreach(NSRFunc f in funcs)
        //				{
        //					Node n = new Node(f.No,f.Name) ;
        //					n.NavigateUrl=System.Web.HttpContext.Current.Request.ApplicationPath +f.Url;
        //					n.Target="mainfrm";
        //					nd.Nodes.Add( n );
        //				}				
        //			}
        //		}
    }
	public class Node : Microsoft.Web.UI.WebControls.TreeNode 
	{
		private void SetText()
		{			 
			//this.Text=this.SelfNo+" "+this.SelfName;
			this.Text= this.SelfName;
			//this.ID="_"+this.SelfOID.ToString()+this.SelfNo;
		}
		/// <summary>
		/// Node
		/// </summary>
		public Node()
		{			
			this.InitNode();
		}
        public void InitNode()
        {
            this.DefaultStyle.CssText = "TreeDefaultStyle" + WebUser.Style;
            this.HoverStyle.CssText = "TreeHoverStyle" + WebUser.Style;
            this.SelectedStyle.CssText = "TreeSelectedStyle" + WebUser.Style;
            this.ExpandedImageUrl = System.Web.HttpContext.Current.Request.ApplicationPath + "Images/Pub/BillOpen.gif";
            this.ImageUrl = System.Web.HttpContext.Current.Request.ApplicationPath + "Images/Pub/Bill.gif";
        }
		public Node(int OID, string No, string Name)
		{
			SelfOID=OID;
			SelfNo=No;
			SelfName=Name;
			this.SetText();
			this.InitNode();
		}
		/// <summary>
		/// No, Name
		/// </summary>
		/// <param name="No">编码</param>
		/// <param name="Name">名称</param>
		public Node(string No, string Name)
		{
			SelfNo=No;
			SelfName=Name;
			this.SetText();
			this.InitNode();

		}
		public Node(DataRow dr)
		{
			SelfOID=(int)dr["OID"];
			SelfNo=(string)dr["No"];
			SelfName=(string)dr["Name"];
			this.SetText();

			this.InitNode();

		}		 
		/// <summary>
		/// 树结点的本级编号
		/// </summary>
		public string SelfNo
		{
			get
			{
				if (ViewState["Self_GradeNo"]==null)				
					return "";
				else			
					return (string)ViewState["Self_GradeNo"];			
			}
			set
			{
				ViewState["Self_GradeNo"] = value;
			}
		}
		/// <summary>
		/// 树结点的本级编号
		/// </summary>
		public int SelfLevel
		{
			get
			{
				if (ViewState["SelfLevel"]==null)
					return 1;
				else			
					return (int)ViewState["SelfLevel"];			
			}
			set 
			{
				ViewState["SelfLevel"] = value;
			}
		}
		/// <summary>
		/// 是不是明晰
		/// </summary>
		public bool SelfIsDtl
		{
			get
			{
				if (ViewState["SelfIsDtl"]==null)
					return true;
				else			
					return (bool)ViewState["SelfIsDtl"];			
			}
			set 
			{
				ViewState["SelfIsDtl"] = value;
			}
		}
		/// <summary>
		/// 树结点的名称
		/// </summary>
		public string SelfName
		{
			get 
			{
				object obj = ViewState["Self_Name"];
				if (obj != null)
					return(string)obj;
				return null;
			}
			set 
			{
				ViewState["Self_Name"] = value;
			}
		}
		/// <summary>
		/// 树结点的唯一标识OID
		/// </summary>
		public int SelfOID
		{
			get 
			{				
				if (ViewState["Self_OID"] == null)
					return -1;
				else
					return int.Parse(ViewState["Self_OID"].ToString()); 
			}
			set 
			{
				ViewState["Self_OID"] = value;
			}
		}
		/// <summary>
		/// 是不是目录?
		/// </summary>
		public bool SelfIsDir
		{
			get
			{
				if (ViewState["SelfIsDir"]==null)
					return true;
				else			
					return (bool)ViewState["SelfIsDir"];			
			}
			set 
			{
				ViewState["SelfIsDir"] = value;
			}
		}
		 
	}
}
