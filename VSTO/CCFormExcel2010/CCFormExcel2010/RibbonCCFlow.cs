using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace CCFlowExcel
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
    }
}
