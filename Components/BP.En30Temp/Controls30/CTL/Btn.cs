using System;
using System.Web.UI.WebControls;
using System.Drawing;
using System.ComponentModel;

namespace  BP.Web.Controls
{
	/// <summary>
	/// GenerButton 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.Button))]
	public class Btn : System.Web.UI.WebControls.Button
	{
		private BtnType _ShowType=BtnType.Normal;
		[Category("自定义"),Description("按钮显示类型（为了实现全局统一管理）")]
		public BtnType ShowType
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
		/// 提示信息。
		/// </summary>
		public string  Hit
		{
			get
			{ 
				return ViewState["_Hit"].ToString();
			}
			set
			{
				 ViewState["_Hit"] =value;
			}
		}
		/// <summary>
		/// Btn
		/// </summary>
		/// <param name="btntype">btntype</param>
		public Btn(BtnType btntype)
		{
			this.ShowType =btntype; 
			//this.PreRender += new System.EventHandler(this.BtnPreRender);
		}
		/// <summary>
		/// Btn
		/// </summary>
        public Btn()
        {
            //this.Attributes["class"] = "Btn";
            // this.PreRender += new System.EventHandler(this.BtnPreRender);
        }
		private void BtnPreRender( object sender, System.EventArgs e )
		{
			//this.Attributes["onclick"] +="javascript:showRuning();";
//			if (this.Hit!=null)
//				this.Attributes["onclick"] = "javascript: return confirm('是否继续？'); ";
			switch (this.ShowType )
			{
				case BtnType.ConfirmHit :
					if (this.Text==null || this.Text=="")
						this.Text="确认(A)";
					if (this.AccessKey==null) 
						this.AccessKey="a";
					 
					this.Attributes["onclick"] = " return confirm('"+this.Hit+"');";				
					break;
			 
				case BtnType.Refurbish :
					if (this.Text==null || this.Text=="") 			 
						this.Text="刷新(R)";
					if (this.AccessKey==null) 	
						this.AccessKey="r";
					break;
				case BtnType.Back :
					if (this.Text==null || this.Text=="") 			 
						this.Text="返回(B)";
					if (this.AccessKey==null) 	
						this.AccessKey="b";
					break;
				case BtnType.Edit :
					if (this.Text==null || this.Text=="") 			 
						this.Text="修改(E)";
					if (this.AccessKey==null) 	
						this.AccessKey="e";
					break;
				case BtnType.Close :
					if (this.Text==null || this.Text=="") 			 
						this.Text="关闭(Q)";
					if (this.AccessKey==null) 	
						this.AccessKey="q";

					this.Attributes["onclick"] += " window.close(); return false";
					
					break;
				case BtnType.Cancel :
					if (this.Text==null || this.Text=="") 			 
						this.Text="取消(C)";
					if (this.AccessKey==null) 	
						this.AccessKey="c";
					break;				　
				case BtnType.Confirm :
					if (this.Text==null || this.Text=="")
						this.Text="确定(O)";
					if (this.AccessKey==null)
						this.AccessKey="o";
					break;
				case BtnType.Search :
					if (this.Text==null || this.Text=="") 			 
						this.Text="查找(F)";
					if (this.AccessKey==null)
						this.AccessKey="f";
					break;
				case BtnType.New :
					if (this.Text==null || this.Text=="") 			 
						this.Text="新建(N)";
					if (this.AccessKey==null) 
						this.AccessKey="n";
					break;
				case BtnType.SaveAndNew :
					if (this.Text==null || this.Text=="") 			 
						this.Text="保存并新建(R)";
					if (this.AccessKey==null) 
						this.AccessKey="n";
					break;
				case BtnType.Delete :
					if (this.Text==null || this.Text=="") 			 
						this.Text= "删除(D)";
					if (this.AccessKey==null)
						this.AccessKey="c";
					if (this.Hit==null)
						this.Attributes["onclick"] += " return confirm('此操作要执行删除，是否继续？');";
					else
						this.Attributes["onclick"] += " return confirm('此操作要执行删除　["+this.Hit+"]，是否继续？');";
					break;
				case BtnType.Export :
					if (this.Text==null || this.Text=="") 			 
						this.Text="导出(G)";
					if (this.AccessKey==null) 	
						this.AccessKey="g";
					break;
				case BtnType.Insert :
					if (this.Text==null || this.Text=="") 			 
						this.Text="插入(I)";
					if (this.AccessKey==null) 	
						this.AccessKey="i";
					break ;
				case BtnType.Print :
					if (this.Text==null || this.Text=="") 			 
						this.Text="打印(P)";
					if (this.AccessKey==null) 	
						this.AccessKey="p";

					if (this.Hit==null)
						this.Attributes["onclick"] += " return confirm('此操作要执行打印，是否继续？');";
					else
						this.Attributes["onclick"] += " return confirm('此操作要执行打印　["+this.Hit+"]，是否继续？');";
					break ;
				case BtnType.Save :
					if (this.Text==null || this.Text=="") 			 
						this.Text="保存(S)";
					if (this.AccessKey==null)
						this.AccessKey="s";
					break;
				case BtnType.View:
					if (this.Text==null || this.Text=="") 			 
						this.Text="浏览(V)";
					if (this.AccessKey==null) 	
						this.AccessKey="v";
					break;
				case BtnType.Add:
					if (this.Text==null || this.Text=="") 			 
						this.Text="增加(A)";
					if (this.AccessKey==null) 	
						this.AccessKey="a";
					break;
				case BtnType.SelectAll:
					if (this.Text==null || this.Text=="") 			 
						this.Text="全选择(A)";
					if (this.AccessKey==null) 	
						this.AccessKey="a";
					break;
				case BtnType.SelectNone:
					if (this.Text==null || this.Text=="") 			 
						this.Text="全不选(N)";
					if (this.AccessKey==null) 	
						this.AccessKey="n";
					break;
				case BtnType.Reomve:
					if (this.Text==null || this.Text=="") 			 
						this.Text="移除(M)";
					if (this.AccessKey==null) 	
						this.AccessKey="m";

					if (this.Hit==null)
						this.Attributes["onclick"] = " return confirm('此操作要执行移除，是否继续？');";
					else
						this.Attributes["onclick"] = " return confirm('此操作要执行移除　["+this.Hit+"]，是否继续？');";

					break;
				default:
					if (this.Text==null || this.Text=="")
						this.Text="确定(O)";
					if (this.AccessKey==null)
						this.AccessKey="o";
					break; 
			} 
			 
		}

		 
	}
}
