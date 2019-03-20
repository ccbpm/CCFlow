using System;
using System.Web.UI.WebControls;
using System.Drawing;
using System.ComponentModel;


namespace  BP.Web.Controls
{
	/// <summary>
	/// GenerButton 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.ImageButton))]
	public class ImageBtn : System.Web.UI.WebControls.ImageButton 
	{
		public enum ImageBtnType
		{
			Normal,			 
			Confirm,
			Save,
			Search,
		    Cancel,
			Delete,
			Update,
			Insert,
			Edit,
			New,
			View,
			Close,
			Export,
			Print,
			Add,
			Reomve
		}		
		private ImageBtnType _ShowType=ImageBtnType.Normal;
		public ImageBtnType ShowType
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
		private string _Hit=null;
		/// <summary>
		/// 提示信息。
		/// </summary>
		public string  Hit
		{
			get
			{ 
				return _Hit;
			}
			set
			{
				this._Hit=value;
			}
		}
		public ImageBtn()
		{	
			this.PreRender += new System.EventHandler(this.LinkBtnPreRender);
		}
		private void LinkBtnPreRender( object sender, System.EventArgs e )
		{
			if (this.Hit!=null)
				this.Attributes["onclick"] = "javascript: return confirm('是否继续？'); ";

			switch (this.ShowType )
			{
				case ImageBtnType.Edit :
					this.ImageUrl="修改";
					 
					if (this.AccessKey==null)
						this.AccessKey="e";
					break;
				case ImageBtnType.Close :
					this.ImageUrl="关闭";
					 
					if (this.AccessKey==null) 	
						this.AccessKey="q";					 
					break;
				case ImageBtnType.Cancel :
					this.ImageUrl="取消";
					 
					if (this.AccessKey==null) 	
						this.AccessKey="c";
					break;				　
				case ImageBtnType.Confirm :
					this.ImageUrl="确定";
					 
					if (this.AccessKey==null)
						this.AccessKey="o";
				    break;
				case ImageBtnType.Search :
					this.ImageUrl="查找";
					 
					if (this.AccessKey==null)
						this.AccessKey="f";
					break;
				case ImageBtnType.New :
					this.ImageUrl="新建";
					 
					if (this.AccessKey==null) 
						this.AccessKey="n";
					break;
				case ImageBtnType.Delete :
					this.ImageUrl="删除";
					 
					if (this.AccessKey==null)
						this.AccessKey="c";
					if (this.Hit==null)
					    this.Attributes["onclick"] = " return confirm('此操作要执行删除，是否继续？');";
					else
						this.Attributes["onclick"] = " return confirm('此操作要执行删除　["+this.Hit+"]，是否继续？');";

					break;
				case ImageBtnType.Export :
					this.ImageUrl="导出";
					 
					if (this.AccessKey==null) 	
						this.AccessKey="g";
					break;
				case ImageBtnType.Insert :
					 
					this.ImageUrl="插入";
					 
					if (this.AccessKey==null) 	
						this.AccessKey="i";
					break ;
				case ImageBtnType.Print :
					this.ImageUrl="ssss";
					if (this.AccessKey==null) 	
						this.AccessKey="p";

					if (this.Hit==null)
						this.Attributes["onclick"] = " return confirm('此操作要执行打印，是否继续？');";
					else
						this.Attributes["onclick"] = " return confirm('此操作要执行打印　["+this.Hit+"]，是否继续？');";
					break ;
				case ImageBtnType.Save :
					this.ImageUrl="ssss";
					if (this.AccessKey==null)
						this.AccessKey="s";
					break;
				case ImageBtnType.View:
					this.ImageUrl="ssss";
					if (this.AccessKey==null) 	
						this.AccessKey="v";
					break;
				case ImageBtnType.Add:
					this.ImageUrl="ssss";
					if (this.AccessKey==null) 	
						this.AccessKey="a";
					break;
				case ImageBtnType.Reomve:
					this.ImageUrl="移除";
					 

					if (this.Hit==null)
						this.Attributes["onclick"] = " return confirm('此操作要执行移除，是否继续？');";
					else
						this.Attributes["onclick"] = " return confirm('此操作要执行移除　["+this.Hit+"]，是否继续？');";
					break;
				default:
					 this.ImageUrl="确定";
					if (this.AccessKey==null)
						this.AccessKey="o";
					break; 
			}		 
			
			 
		}	
	 
		
		 
	}
}
