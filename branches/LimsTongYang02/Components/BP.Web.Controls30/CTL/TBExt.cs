using System;
using System.Web.UI.WebControls;
using System.Drawing;

namespace BP.Web.Controls
{
//	public enum TBExtType
//	{
//		/// <summary>
//		/// 正常的
//		/// </summary>
//		Normal,	 
//		/// <summary>
//		/// 用户定义
//		/// </summary>
//		Custom,
//		/// <summary>
//		/// 纳税人
//		/// </summary>
//		TaxpayerKey,
//		/// <summary>
//		/// 房地产建设单位
//		/// </summary>
//		FDCUnit,
//		/// <summary>
//		/// 建筑项目建巩单位
//		/// </summary>
//		FDCTaxpayer,
//		/// <summary>
//		/// 建筑项目
//		/// </summary>
//        FDCBulidPrjNo,		
//		/// <summary>
//		/// 房地产项目
//		/// </summary>
//		FDCRealtyPrjNo
//	} 
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	public class TBExt:System.Web.UI.WebControls.TextBox
	{
		public TBExt()
		{
			this.PreRender += new System.EventHandler(this.TBPreRender);
		}
		private void TBPreRender( object sender, System.EventArgs e )
		{
			#region 
			#endregion 
		}
		
		private string  _DataHelpKey="";
		public string DataHelpKey 
		{
			get
			{
			   return _DataHelpKey;				
			}
			set
			{
				this._DataHelpKey=value;
			}
		}
		public object TextExt
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text=value.ToString();
			}
		}
		public int TextExtInt
		{
			get
			{
				return int.Parse(this.Text);
			}
			set
			{
				this.Text=value.ToString();
			}
		}
		public float TextExtFloat
		{
			get
			{
				return float.Parse(this.Text);
			}
			set
			{
				this.Text=value.ToString();
			}
		}

		public decimal TextExtDecimal
		{
			get
			{
				return decimal.Parse(this.Text);
			}
			set
			{
				this.Text=value.ToString();
			}
		} 

	}
	
}
