using System;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using BP.En;
 

namespace  BP.Web.Controls
{
	/// <summary>
	/// GenerButton 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.CheckBoxList))]	
	public class CBL : System.Web.UI.WebControls.CheckBoxList
	{
		public string GetSelectValues(string spt)
		{
			string val="";
			foreach(ListItem li in Items)
			{
				if (li.Selected)
					val+=li.Value+spt;
			}
			if (val=="")
				return "";
			return val.Substring(0,val.Length-1);
		}
		public string SelectedValsToSQLInStr
		{
			get
			{
				string yearIn="";
				foreach(ListItem li in Items)
				{
					if (li.Selected)
						yearIn+="'"+li.Value+"',";
				}
				return yearIn.Substring(0,yearIn.Length-1);
			}
		}


		#region 与实体的操作。
		/// <summary>
		/// 与BindEntities
		/// </summary>
		/// <param name="ens">EntitiesNoName</param>
		public void BindEntities(En.EntitiesNoName ens,bool isShowKey)
		{
			this.Items.Clear();
			foreach(EntityNoName en in ens)
			{
				if (isShowKey)
				{
					this.Items.Add(new ListItem(en.No+" "+en.Name,en.No));
				}
				else
				{
					this.Items.Add(new ListItem( en.Name,en.No));
				}
			}
		}
	 
	 
		/// <summary>
		/// 选择的
		/// </summary>
		/// <param name="ens"></param>
		public void BindSelected(EntitiesNoName ens)
		{
			 this.BindSelected(ens.ToDataTableField());
		}
		#endregion


		/// <summary>
		/// check Bill list
		/// </summary>
		public CBL()
		{
			//this.BorderStyle=BorderStyle.Groove;
			//this.CssClass="CBL"+WebUser.Style;
			return ;
		}
		/// <summary>
		/// OID, No , Name 
		/// </summary>
		public void BindByTableOIDNoName(DataTable dt)
		{
			foreach (DataRow dr in dt.Rows)
			{
				this.Items.Add(new ListItem(dr["No"].ToString()+ "  " +dr["Name"].ToString(),dr["OID"].ToString() ));
			}
		}
		/// <summary>
		///  No , Name 
		/// </summary>
		public void BindByTableNoName(DataTable dt)
		{
			foreach (DataRow dr in dt.Rows)
			{
				this.Items.Add(new ListItem(dr["No"].ToString()+ "  " +dr["Name"].ToString(),dr["No"].ToString() ));
			}
		}
		/// <summary>
		/// sssDS
		/// </summary>
		/// <param name="dt"></param>
		public void BindSelected(DataTable dt )
		{
			foreach( ListItem li in this.Items)
			{
				li.Selected=false;
			}
		
			foreach(ListItem li in this.Items)
			{
				foreach(DataRow dr in dt.Rows)				
				{
					if (dr["No"].ToString().Equals(li.Value))
					{
						li.Selected=true;
					}
				}
			}
		}
		/// <summary>
		/// Bind选择的
		/// </summary>
		/// <param name="ens">EntitiesOIDName</param>
		public void BindSelected(EntitiesOIDName  ens )
		{
			foreach( ListItem li in this.Items)
			{
				li.Selected=false;
			}
		
			foreach( ListItem li in this.Items)
			{
				foreach(EntityOIDName en in ens )				
				{
					if (en.OID.ToString()==li.Value )
					{
						li.Selected=true;
					}
				}
			}
		}
		/// <summary>
		/// OID
		/// </summary>
		/// <returns></returns>
		public DataTable SaveSelectedItemValToTableOID()
		{
			DataTable dt =new DataTable();
			dt.Columns.Add(new DataColumn("OID",typeof(int)) );
			foreach(ListItem li in this.Items)
			{
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["OID"]=li.Value;
					dt.Rows.Add(dr);
				}
			}
			return dt;
		}
		/// <summary>
		/// No
		/// </summary>
		/// <returns></returns>
		public DataTable SaveSelectedItemValToTableNo()
		{
			DataTable dt =new DataTable();
			dt.Columns.Add(new DataColumn("No",typeof(string)) );
			foreach(ListItem li in this.Items)
			{
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					dr["No"]=li.Value;
					dt.Rows.Add(dr);
				}
			}
			return dt;
		}

		#region 方法
		/// <summary>
		/// 选择全部
		/// </summary>
		public void SelectAll()
		{
			foreach(ListItem li in this.Items)
			{
				li.Selected=true;
			}
		}
		/// <summary>
		/// 全不选择。
		/// </summary>
		public void SelectNone()
		{
			foreach(ListItem li in this.Items)
			{
				li.Selected=false;
			}
		}
		#endregion 
	}
}
