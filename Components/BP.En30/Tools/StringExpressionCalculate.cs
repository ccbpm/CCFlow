using System;
using System.Collections;
using System.Data;

namespace BP.Tools
{
	/// <summary>
	/// Class1 的摘要说明。
	/// </summary>
	public class StringExpressionCalculate
	{
		public StringExpressionCalculate()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		/// <summary>
		/// 转换到decimal.
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		public decimal TurnToDecimal(string exp)
		{
			string str =CalculateParenthesesExpression( exp ) ;
			str=str.Replace("E+","");
			return Math.Round( decimal.Parse(str), 4) ;
		}
		public float TurnToFloat(string exp)
		{
			return float.Parse( CalculateParenthesesExpression( exp ) );
		}

		//中序转换成后序表达式再计算
		// 如：23+56/(102-100)*((36-24)/(8-6))
		// 转换成：23|56|102|100|-|/|*|36|24|-|8|6|-|/|*|+"
		//以便利用栈的方式都进行计算。
        public string CalculateParenthesesExpression(string Expression)
        {
            ArrayList operatorList = new ArrayList();
            string operator1;
            string ExpressionString = "";
            string operand3;
            Expression = Expression.Replace(" ", "");
            Expression = Expression.Replace("%", "");
            Expression = Expression.Replace("％", "");

            while (Expression.Length > 0)
            {
                operand3 = "";
                //取数字处理
                if (Char.IsNumber(Expression[0]) || Expression[0] == '.')
                {
                    while (Char.IsNumber(Expression[0]) || Expression[0] == '.')
                    {
                        operand3 += Expression[0].ToString();
                        Expression = Expression.Substring(1);
                        if (Expression == "") break;


                    }
                    ExpressionString += operand3 + "|";
                }

                //取“C”处理
                if (Expression.Length > 0 && Expression[0].ToString() == "(")
                {
                    operatorList.Add("(");
                    Expression = Expression.Substring(1);
                }

                //取“)”处理
                operand3 = "";
                if (Expression.Length > 0 && Expression[0].ToString() == ")")
                {
                    do
                    {

                        if (operatorList[operatorList.Count - 1].ToString() != "(")
                        {
                            operand3 += operatorList[operatorList.Count - 1].ToString() + "|";
                            operatorList.RemoveAt(operatorList.Count - 1);
                        }
                        else
                        {
                            operatorList.RemoveAt(operatorList.Count - 1);
                            break;
                        }

                    } while (true);
                    ExpressionString += operand3;
                    Expression = Expression.Substring(1);
                }

                //取运算符号处理
                operand3 = "";
                if (Expression.Length > 0 && (Expression[0].ToString() == "*" || Expression[0].ToString() == "/" || Expression[0].ToString() == "+" || Expression[0].ToString() == "-"))
                {
                    operator1 = Expression[0].ToString();
                    if (operatorList.Count > 0)
                    {

                        if (operatorList[operatorList.Count - 1].ToString() == "(" || verifyOperatorPriority(operator1, operatorList[operatorList.Count - 1].ToString()))
                        {
                            operatorList.Add(operator1);
                        }
                        else
                        {
                            operand3 += operatorList[operatorList.Count - 1].ToString() + "|";
                            operatorList.RemoveAt(operatorList.Count - 1);
                            operatorList.Add(operator1);
                            ExpressionString += operand3;
                        }
                    }
                    else
                    {
                        operatorList.Add(operator1);
                    }
                    Expression = Expression.Substring(1);
                }
            }
            operand3 = "";
            while (operatorList.Count != 0)
            {
                operand3 += operatorList[operatorList.Count - 1].ToString() + "|";
                operatorList.RemoveAt(operatorList.Count - 1);
            }
            ExpressionString += operand3.Substring(0, operand3.Length - 1); ;
            return CalculateParenthesesExpressionEx(ExpressionString);
        }
		// 第二步:把转换成后序表达的式子计算
		//23|56|102|100|-|/|*|36|24|-|8|6|-|/|*|+"
		private string  CalculateParenthesesExpressionEx(string Expression)
		{
			//定义两个栈
			ArrayList operandList =new ArrayList();
			float operand1;
			float operand2;
			string[] operand3;
  
			Expression = Expression.Replace(" ","");
			operand3 = Expression.Split(Convert.ToChar("|"));
 
			for(int i = 0;i < operand3.Length;i++)
			{
				if(Char.IsNumber(operand3[i],0))
				{
					operandList.Add( operand3[i].ToString());
				}
				else
				{
					//两个操作数退栈和一个操作符退栈计算
					operand2 =(float)Convert.ToDouble(operandList[operandList.Count-1]);
					operandList.RemoveAt(operandList.Count-1);  
					operand1 =(float)Convert.ToDouble(operandList[operandList.Count-1]);
					operandList.RemoveAt(operandList.Count-1);
					operandList.Add(calculate(operand1,operand2,operand3[i]).ToString()) ;
				}
			}
			return operandList[0].ToString(); 
		}


		//判断两个运算符优先级别
		private bool verifyOperatorPriority(string Operator1,string Operator2)
		{
   
			if(Operator1=="*" && Operator2 =="+") 
				return true;
			else if(Operator1=="*" && Operator2 =="-") 
				return true;
			else if(Operator1=="/" && Operator2 =="+") 
				return true;
			else if(Operator1=="/" && Operator2 =="-") 
				return true;
			else
				return false;
		}

		//计算
		private float calculate(float operand1, float operand2,string operator2)
		{
			switch(operator2)
			{
				case "*":
					operand1 *=  operand2;
					break;
				case "/":
					operand1 /=  operand2;
					break;
				case "+":
					operand1 +=  operand2;
					break;
				case "-":
					operand1 -=  operand2;
					break;
				default:
					break;
			}
			return operand1;
		}

        /// <summary>
        /// 去掉文件名中的无效字符,如 \ / : * ? " < > | 
        /// </summary>
        /// <param name="fileName">待处理的文件名</param>
        /// <returns>处理后的文件名</returns>
        public static string ReplaceBadCharOfFileName(string fileName)
        {
            string str = fileName;
            str = str.Replace("\\", string.Empty);
            str = str.Replace("/", string.Empty);
            str = str.Replace(":", string.Empty);
            str = str.Replace("*", string.Empty);
            str = str.Replace("?", string.Empty);
            str = str.Replace("\"", string.Empty);
            str = str.Replace("<", string.Empty);
            str = str.Replace(">", string.Empty);
            str = str.Replace("|", string.Empty);
            str = str.Replace(" ", string.Empty);   
            //前面的替换会产生空格,最后将其一并替换掉
            return str;
        }
	}
}
