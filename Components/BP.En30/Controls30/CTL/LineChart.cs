using System;
using System.Web.UI.WebControls; 
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Web.UI;
using System.Collections;
using System.Data;
using BP.En;
using BP.DA;
using System.ComponentModel;


namespace  BP.Web.Controls
{
	/// <summary>
	/// LineChart的摘要说明。
	/// </summary>
	public class LineChart : System.Web.UI.WebControls.Image
	{
		public LineChart()
		{ 
			this.PreRender += new System.EventHandler(this.LineChartPreRender);
			//ScaleX = ImageWidth ; 
			//ScaleY = ImageHeight ;
			b = new Bitmap( ImageWidth ,  ImageHeight ) ;
			g = Graphics.FromImage (b) ;			
		}
		private void LineChartPreRender( object sender, System.EventArgs e )
		{
			if (this.ImageUrl==null || this.ImageUrl=="") 
				this.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+"/images/sys/LineChart.gif";
			//this.BorderStyle=System.Web.UI.WebControls.BorderStyle.Double;
			this.BorderColor=Color.Black;

		}

		public Bitmap b ;
		public string Title = "BP 数据图表" ;
		public ArrayList chartValues = new ArrayList() ;
		public float Xorigin =0 , Yorigin = 0 ;
		public bool IsShowLab=false;

		private DataTable _Table ;

		/// <summary>
		/// 这个数据表存放了点的座标
		/// </summary>
		public DataTable Table 
		{
			get
			{
				return this._Table;
			}
			set
			{
				this._Table = value;
				foreach(DataRow dr in this._Table.Rows)
				{
					this.AddEntity(float.Parse(dr["Cash"].ToString()),dr["KJND"].ToString()+dr["KJNY"].ToString()) ;
				}
			}
		}
		//public float ScaleX=1000 ; 
		/// <summary>
		/// Y最大的 金额
		/// </summary>
		public float YMaxCash=500000;
		//public float Xdivs=2;
		/// <summary>
		/// 要在y 组增长的点数。
		/// </summary>
		public float YPontNum=20;
		/// <summary>
		/// 像素宽度
		/// </summary>
		public int ImageWidth=800;
		/// <summary>
		/// 像素高度
		/// </summary>
		public int ImageHeight=600;
		//	    public int posX=0;
		//		public int posY=0;
		private Graphics g ;
		//private Page p ;

		struct DataPoint 
		{
			public float x ;
			public float y ;
			public bool valid ;
			public float Cash;
			public string KJNY;
			 
		}		 
		/// <summary>
		/// 加入一个点
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void AddPoint( float x , float y, string kjny, float val ) 
		{
			DataPoint myPoint;
			myPoint.x = x ;
			myPoint.y = y ;
			myPoint.valid = true ;
			myPoint.KJNY=kjny;
			myPoint.Cash=val;
			chartValues.Add( myPoint ) ;
		}
		/// <summary>
		/// 增加一个实体
		/// </summary>
		/// <param name="val">一般是金额</param>
		/// <param name="kjny">一般是会计年月 format yyyym</param>
		public void AddEntity(float val , string kjny )
		{
			 
			if ( val > this.YMaxCash ) 
				throw new Exception("@Y 座标设置的最大值["+this.YMaxCash.ToString()+"]低于显示的数额["+val.ToString()+"]。");
			float y= val/this.YMaxCash * this.ChartWidth-this.ChartInset;
		}
		/// <summary>
		/// 定义的边框两边的宽度。
		/// </summary>
		public int ChartInset
		{
			get
			{
				return 50;
			}
		}
		/// <summary>
		/// 宽度
		/// </summary>
		public int ChartWidth 
		{
			get
			{
				return ImageWidth - ( 2 * ChartInset );
			}
		}
		/// <summary>
		/// 高度
		/// </summary>
		public int ChartHeight 
		{
			get
			{
				return ImageHeight - ( 2 * ChartInset );
			}
		}
		/// <summary>
		/// 字体
		/// </summary>
		public Font axesFont
		{
			get
			{
				return new Font ( "arial" , 10);
			}
		}
		public Pen RedPen
		{
			get
			{
				return new Pen(Color.Red, 3 ) ;
			}
		}
		public Pen BlackPen
		{
			get
			{
				return new Pen(Color.Black, 3 ) ;
			}
		}
		public Pen GradePen
		{
			get
			{
				//float i =float.Parse( "1");
				return new Pen(Color.DarkBlue,1 ) ;
			}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public Brush BlackBrush 
		{
			get
			{
				return new SolidBrush ( Color.Black );	
			}
		} 
		public bool IsShowGirde=true;
		/// <summary>
		/// 是否显示gride . 
		/// </summary>
		public void Draw() 
		{
			int i ;
			float x , y , x0 , y0 ; 	 

			//首先要创建图片的大小
			//p.Response.ContentType = "image/jpeg" ;
			//p.Response.ContentType = "image/jpeg" ;
			g.FillRectangle ( new SolidBrush ( Color.White ) , 0 , 0 , ImageWidth  , ImageHeight  ) ;
			 
			///画一个矩形 
			g.DrawRectangle ( new Pen( Color.Black , 1) , ChartInset , ChartInset , ChartWidth , ChartHeight );
			//写出图片上面的图片内容文字 
			Font fon= new Font( "宋体" , 14 );
			g.DrawString( Title , fon , BlackBrush , ImageWidth/4, 10 );	

			#region 沿Y坐标写入Y标签	
			 
			for (  i = 0 ; i <= YPontNum ; i++ )
			{
				x = ChartInset ;
				y = ChartHeight + ChartInset - ( i * ChartHeight / YPontNum ) ;
				string myLabel = ( Yorigin + ( YMaxCash * i / YPontNum ) ).ToString ( ) ;
				g.DrawString(myLabel , axesFont , BlackBrush , 5 , y - 6 ) ;
				g.DrawLine ( BlackPen , x + 2 , y , x - 2 , y ) ;  /// 让其显示一个 dot 在图上。 有两个基本很近的点组合成的。
				if (IsShowGirde && i > 0 )
				{
					g.DrawLine ( this.GradePen , ChartInset , y   , ChartInset +  ChartWidth  ,  y) ;
				}
			}
			g.RotateTransform ( 180 ) ;
			g.TranslateTransform ( 0 , - ChartHeight ) ;
			g.TranslateTransform ( - ChartInset , ChartInset ) ;
			g.ScaleTransform ( - 1 , 1);
			/// 如果要显示gride .
			#endregion

			#region 画出图表中的数据 
			DataPoint prevPoint = new DataPoint();
			prevPoint.valid = false ;
			foreach ( DataPoint myPoint in chartValues ) 
			{
				if ( prevPoint.valid == true ) 
				{
//					Xorigin = 2;
//					Yorigin=2;

//					x0 = ChartWidth * ( prevPoint.x - Xorigin ) /ImageWidth;
//					y0 = ChartHeight*( prevPoint.y - Yorigin )/ImageHeight;
//
//					x = ChartWidth * ( myPoint.x - Xorigin ) / ImageWidth ;
//					y = ChartHeight * ( myPoint.y - Yorigin ) / ImageHeight ;



										x0 =  prevPoint.x ;
										y0 =  prevPoint.y ;					
										x =   myPoint.x ;
										y =  myPoint.y ;

					
					g.DrawLine ( RedPen , x0 , y0 , x , y );
					//g.FillEllipse(BlackBrush , x0 - 2 , y0 - 2, 2 , 2 ) ;
					//g.FillEllipse( BlackBrush , x - 2 , y - 2 , 2 , 2 );

					g.FillEllipse(BlackBrush , x0-2, y0-2, 4, 4 ) ;
					g.FillEllipse( BlackBrush , x-2 , y-2, 4 , 4);

					//g.FillEllipse( BlackBrush , x0  , y0  , 1 , 1 ) ;
					//g.FillEllipse( BlackBrush , x  , y , 1 , 1 );

					if (this.IsShowLab)
					{
						g.DrawString( myPoint.KJNY+";"+myPoint.Cash.ToString(), new Font("宋体",10), BlackBrush , x,y  );	
						//g.DrawString( "abc", new Font("宋体",10), BlackBrush , x,y  );	
						//g.RotateTransform ( 180 ) ;
						//g.TranslateTransform ( 0 , - ImageHeight ) ;
						//g.TranslateTransform ( - ChartInset , ChartInset ) ;
						//g.ScaleTransform ( - 1 , 1);
					}
				}
				prevPoint = myPoint ;
			}
			#endregion	

            #region 最后以图片形式来浏览
			string fileName=DBAccess.GenerOID().ToString()+".Jpeg";
			b.Save( ExportFilePath+ fileName, ImageFormat.Jpeg);
			this.ImageUrl=System.Web.HttpContext.Current.Request.ApplicationPath+"/Temp/" + fileName;
			#endregion
		}
	
		/// <summary>
		/// 导出文件的路径
		/// </summary>
		protected string ExportFilePath
		{
			get
			{
				return this.Page.Request.PhysicalApplicationPath + "Temp\\";
			}
		}
		 
		/*
		LineChart() 
		{
			g.Dispose();
			b.Dispose();
		}*/
		 
	
	}
}
