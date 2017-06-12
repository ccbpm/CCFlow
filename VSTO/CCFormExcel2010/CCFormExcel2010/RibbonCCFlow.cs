using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace CCFormExcel2010
{
	public partial class RibbonCCFlow
	{
		private void RibbonCCFlow_Load(object sender, RibbonUIEventArgs e)
		{

		}

		private void btnSaveFrm_Click(object sender, RibbonControlEventArgs e)
		{
			//手动保存Excel文件
			Globals.ThisAddIn.Application.ActiveWorkbook.Save();
		}

		private void button1_Click(object sender, RibbonControlEventArgs e)
		{
			var range = Globals.ThisAddIn.Application.ActiveCell;
			string strMsg = "ActiveCell";
			strMsg += GetRangeInf(range);
			range = Globals.ThisAddIn.Application.Selection;
			strMsg += "\rSelection";
			strMsg += GetRangeInf(range);
			MessageBox.Show(strMsg);
		}

		private string GetRangeInf(Excel.Range range)
		{
			string r = string.Empty;
			r += "\r\tRange.Value2=" + range.Value2;
			r += "\r\tRange.Address=" + range.Address;
			r += "\r\tRange.AddressLocal=" + range.AddressLocal;
			r += "\r\tRange.get_Address()=" + range.get_Address();
			r += "\r\tRange.Count=" + range.Count;
			r += "\r\tRange.Cells.Count=" + range.Cells.Count;
			r += "\r\tRange.Row=" + range.Row;
			r += "\r\tRange.Rows.Count=" + range.Rows.Count;
			r += "\r\tRange.Column=" + range.Column;
			r += "\r\tRange.Columns.Count=" + range.Columns.Count;
			r += "\r\tRange.MergeCells=" + range.MergeCells;
			try
			{
				r += "\r\tRange.MergeArea.Count=" + range.MergeArea.Count;
			}
			catch (Exception exp) { }
			try
			{
				r += "\r\tRange.Name.Name=" + range.Name.Name;
			}
			catch (Exception exp) { }
			return r;
		}

		private void GetValByName_TextChanged(object sender, RibbonControlEventArgs e)
		{
			try
			{
				MessageBox.Show(Globals.ThisAddIn.Application.Names.Item(GetValByName.Text).RefersToRange.Value2);
			}
			catch (Exception exp) { }
		}

		private void GetValByAddr_TextChanged(object sender, RibbonControlEventArgs e)
		{
			try
			{
				MessageBox.Show(Globals.ThisAddIn.Application.get_Range(GetValByAddr.Text).Value2);
			}
			catch (Exception exp) { }
		}

		private void CurrentSelectionName_Click(object sender, RibbonControlEventArgs e)
		{
			try
			{
				//如果Selection==ActiveCell
				Excel.Range activeCell = Globals.ThisAddIn.Application.ActiveCell;
				Excel.Range selection = Globals.ThisAddIn.Application.Selection;
				if (activeCell.MergeCells
					&& activeCell.MergeArea.Rows.Count == selection.Rows.Count
					&& activeCell.MergeArea.Columns.Count == selection.Columns.Count)
				{
					MessageBox.Show(activeCell.Name.NameLocal);
				}
				else
				{
					MessageBox.Show(selection.Name.NameLocal);
				}
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
				//try
				//{
				//    Excel.Range range = Globals.ThisAddIn.Application.ActiveCell;
				//    Excel.Name name = range.Name;
				//    MessageBox.Show(name.NameLocal);
				//}
				//catch (Exception exp1)
				//{
				//    MessageBox.Show(exp1.Message);
				//}
			}
		}
	}
}
