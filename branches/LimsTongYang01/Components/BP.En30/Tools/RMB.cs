
using System;

namespace BP.Tools
{ 
	/// 
	/// 功能：字符串处理函数集 
	/// 
	public class DealString
	{ 
		#region 私有成员 
		/// 
		/// 输入字符串 
		/// 
		private string inputString=null; 
		/// 
		/// 输出字符串 
		/// 
		private string outString=null; 
		/// 
		/// 提示信息 
		/// 
		private string noteMessage=null; 
		#endregion 

		#region 公共属性 
		/// 
		/// 输入字符串 
		/// 
		public string InputString 
		{ 
			get{return inputString;} 
			set{inputString=value;} 
		} 
		/// 
		/// 输出字符串 
		/// 
		public string OutString 
		{ 
			get{return outString;} 
			set{outString=value;} 
		} 
		/// 
		/// 提示信息 
		/// 
		public string NoteMessage 
		{ 
			get{return noteMessage;} 
			set{noteMessage=value;} 
		} 
		#endregion 

		#region 构造函数 
        public DealString() 
		{ 
			// 
			// TODO: 在此处添加构造函数逻辑 
			// 
		} 
		#endregion 

		#region 公共方法 
		public void ConvertToChineseNum() 
		{ 
			string numList="零壹贰叁肆伍陆柒捌玖"; 
			string rmbList = "分角元拾佰仟万拾佰仟亿拾佰仟万"; 
			double number=0; 
			string tempOutString=null; 

			try 
			{ 
				number=double.Parse(this.inputString); 
			} 
			catch 
			{ 
				this.noteMessage="传入参数非数字！"; 
				return; 
			} 

			if(number>9999999999999.99) 
				this.noteMessage="超出范围的人民币值"; 

			//将小数转化为整数字符串 
			string tempNumberString=Convert.ToInt64(number*100).ToString(); 
			int tempNmberLength=tempNumberString.Length; 
			int i=0; 
			while( i<tempNmberLength ) 
			{ 
				int oneNumber=Int32.Parse(tempNumberString.Substring(i,1)); 
				string oneNumberChar=numList.Substring(oneNumber,1); 
				string oneNumberUnit=rmbList.Substring(tempNmberLength-i-1,1); 
				if(oneNumberChar!="零") 
					tempOutString+=oneNumberChar+oneNumberUnit; 
				else 
				{ 
					if(oneNumberUnit=="亿"||oneNumberUnit=="万"||oneNumberUnit=="元"||oneNumberUnit=="零") 
					{ 
						while (tempOutString.EndsWith("零")) 
						{ 
							tempOutString=tempOutString.Substring(0,tempOutString.Length-1); 
						} 

					}
                    if (oneNumberUnit == "亿" || (oneNumberUnit == "万" && !tempOutString.EndsWith("亿")) || oneNumberUnit == "元")
                    {
                        tempOutString += oneNumberUnit;
                    }
                    else
                    {
                        bool tempEnd = tempOutString.EndsWith("亿");
                        bool zeroEnd = tempOutString.EndsWith("零");
                        if (tempOutString.Length > 1)
                        {
                            bool zeroStart = tempOutString.Substring(tempOutString.Length - 2, 2).StartsWith("零");
                            if (!zeroEnd && (zeroStart || !tempEnd))
                                tempOutString += oneNumberChar;
                        }
                        else
                        {
                            if (!zeroEnd && !tempEnd)
                                tempOutString += oneNumberChar;
                        }
                    } 
				} 
				i+=1; 
			} 

			while (tempOutString.EndsWith("零")) 
			{ 
				tempOutString=tempOutString.Substring(0,tempOutString.Length-1); 
			} 

			while(tempOutString.EndsWith("元")) 
			{ 
				tempOutString=tempOutString+"整"; 
			} 

			this.outString=tempOutString; 


		} 
		#endregion 
	} 
} 
